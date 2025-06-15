using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Caching.Memory;
using LeaderBoard.Models;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace LeaderBoard.Core;

public class LeaderBoardEngine
{
    private readonly ConcurrentDictionary<long, CustomerNode> _customers;
    private readonly ConcurrentSkipList<CustomerNode> _rankedList;
    private readonly ConcurrentDictionary<int, long> _rankToCustomerId;
    private readonly SegmentedLockManager _segmentLocks;
    private readonly Channel<RankingUpdateTask> _updateChannel;
    private readonly IMemoryCache _hotDataCache;
    private readonly VectorizedScoreProcessor _scoreProcessor;
    private readonly PeriodicTimer _rankingUpdateTimer;
    private readonly ILogger<LeaderBoardEngine> _logger;

    public LeaderBoardEngine(ILogger<LeaderBoardEngine> logger)
    {
        _customers = new ConcurrentDictionary<long, CustomerNode>();
        _rankedList = new ConcurrentSkipList<CustomerNode>();
        _rankToCustomerId = new ConcurrentDictionary<int, long>();
        _segmentLocks = new SegmentedLockManager();
        _updateChannel = Channel.CreateUnbounded<RankingUpdateTask>(new UnboundedChannelOptions 
        { 
            SingleReader = true,
            SingleWriter = false
        });
        _hotDataCache = new MemoryCache(new MemoryCacheOptions());
        _scoreProcessor = new VectorizedScoreProcessor();
        _rankingUpdateTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));
        _logger = logger;

        _ = ProcessUpdatesAsync();
    }

    public async Task<decimal> UpdateScoreAsync(long customerId, decimal delta)
    {
        var customer = await GetOrCreateCustomerAsync(customerId);
        var newScore = UpdateScoreImmediate(customer, delta);
        
        await _updateChannel.Writer.WriteAsync(new RankingUpdateTask(customer));
        
        return newScore;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private decimal UpdateScoreImmediate(CustomerNode customer, decimal delta)
    {
        return _scoreProcessor.ProcessScore(customer.Score, delta);
    }

    private async Task<CustomerNode> GetOrCreateCustomerAsync(long customerId)
    {
        return _customers.GetOrAdd(customerId, id => new CustomerNode(id, 0));
    }

    private async Task ProcessUpdatesAsync()
    {
        var updates = new List<CustomerNode>();
        const int maxBatchSize = 2000;
        
        while (await _rankingUpdateTimer.WaitForNextTickAsync())
        {
            while (updates.Count < maxBatchSize && 
                   _updateChannel.Reader.TryRead(out var task))
            {
                if (task.Customer.RankDirty)
                    updates.Add(task.Customer);
            }
            
            if (updates.Count > 0)
                await ProcessRankingUpdatesBatchAsync(updates);
                
            updates.Clear();
        }
    }

    private async Task ProcessRankingUpdatesBatchAsync(List<CustomerNode> updates)
    {
        foreach (var customer in updates)
        {
            using var writeLock = _segmentLocks.AcquireWriteLock(customer.CustomerID);
            _rankedList.Insert(customer);
            customer.UpdateRank();
        }
    }

    public async Task<List<CustomerRankInfo>> GetCustomersByRankRangeAsync(int start, int end)
    {
        var cacheKey = $"rank_range_{start}_{end}";
        
        if (_hotDataCache.TryGetValue(cacheKey, out List<CustomerRankInfo>? cachedData))
        {
            return cachedData;
        }

        var customers = _rankedList.GetRange(start - 1, end - start + 1)
            .Select((c, i) => new CustomerRankInfo
            {
                CustomerId = c.CustomerID,
                Score = c.Score,
                Rank = start + i
            })
            .ToList();

        _hotDataCache.Set(cacheKey, customers, TimeSpan.FromSeconds(1));
        
        return customers;
    }

    public async Task<CustomerNeighborsResponse?> GetCustomerNeighborsAsync(
        long customerId, int high, int low)
    {
        if (!_customers.TryGetValue(customerId, out var customer))
            return null;

        var rank = customer.CachedRank;
        var start = Math.Max(1, rank - high);
        var end = rank + low;

        var neighbors = await GetCustomersByRankRangeAsync(start, end);
        
        return new CustomerNeighborsResponse
        {
            CustomerId = customerId,
            Rank = rank,
            Score = customer.Score,
            Neighbors = neighbors
        };
    }

    public async Task<int> GetTotalCustomersAsync()
    {
        return _customers.Count;
    }
} 