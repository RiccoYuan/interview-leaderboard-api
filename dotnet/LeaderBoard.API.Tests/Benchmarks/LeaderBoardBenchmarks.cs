using BenchmarkDotNet.Attributes;
using LeaderBoard.Core;
using Microsoft.Extensions.Logging.Abstractions;

namespace LeaderBoard.API.Tests.Benchmarks;

[MemoryDiagnoser]
public class LeaderBoardBenchmarks
{
  private LeaderBoardEngine _engine;
  private readonly Random _random = new Random();

  [GlobalSetup]
  public void Setup()
  {
    var logger = NullLogger<LeaderBoardEngine>.Instance;
    _engine = new LeaderBoardEngine(logger);

    // 预热：添加100万用户
    Parallel.For(1, 1_000_001, i =>
    {
      _engine.UpdateScoreAsync(i, _random.Next(1, 1000)).Wait();
    });
  }

  [Benchmark]
  [Arguments(1000000)]
  public async Task UpdateScore_SingleUser(int iterations)
  {
    var customerId = _random.NextInt64(1, 1_000_000);
    await _engine.UpdateScoreAsync(customerId, _random.Next(-100, 100));
  }

  [Benchmark]
  [Arguments(1, 100)]
  public async Task GetLeaderboard_TopRanking(int start, int end)
  {
    await _engine.GetCustomersByRankRangeAsync(start, end);
  }

  [Benchmark]
  public async Task GetLeaderboard_Concurrent()
  {
    var tasks = new List<Task>();
    for (int i = 0; i < 1000; i++)
    {
      tasks.Add(_engine.GetCustomersByRankRangeAsync(1, 100));
    }
    await Task.WhenAll(tasks);
  }

  [Benchmark]
  public async Task UpdateScore_Concurrent()
  {
    var tasks = new List<Task>();
    for (int i = 0; i < 1000; i++)
    {
      var customerId = _random.NextInt64(1, 1_000_000);
      tasks.Add(_engine.UpdateScoreAsync(customerId, _random.Next(-100, 100)));
    }
    await Task.WhenAll(tasks);
  }
}