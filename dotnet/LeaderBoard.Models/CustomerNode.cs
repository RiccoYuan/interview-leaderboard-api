using System.Runtime.InteropServices;

namespace LeaderBoard.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct CustomerNode : IComparable<CustomerNode>
{
    public readonly long CustomerID;
    public readonly decimal Score;
    
    private readonly AtomicInt32 _cachedRank;
    private readonly AtomicBoolean _rankDirty;
    private readonly AtomicDateTime _lastUpdate;
    
    public CustomerNode(long customerId, decimal score)
    {
        CustomerID = customerId;
        Score = score;
        _cachedRank = new AtomicInt32(0);
        _rankDirty = new AtomicBoolean(true);
        _lastUpdate = new AtomicDateTime(DateTime.UtcNow);
    }
    
    public int CachedRank => _cachedRank.Value;
    public bool RankDirty => _rankDirty.Value;
    public DateTime LastUpdate => _lastUpdate.Value;
    
    public void UpdateRank()
    {
        _rankDirty.Value = false;
        _lastUpdate.Value = DateTime.UtcNow;
    }
    
    public int CompareTo(CustomerNode other)
    {
        var scoreComparison = Score.CompareTo(other.Score);
        if (scoreComparison != 0)
            return -scoreComparison; // 降序排列
            
        return CustomerID.CompareTo(other.CustomerID);
    }
}

public class AtomicInt32
{
    private int _value;
    
    public AtomicInt32(int value)
    {
        _value = value;
    }
    
    public int Value
    {
        get => Interlocked.CompareExchange(ref _value, 0, 0);
        set => Interlocked.Exchange(ref _value, value);
    }
}

public class AtomicBoolean
{
    private int _value;
    
    public AtomicBoolean(bool value)
    {
        _value = value ? 1 : 0;
    }
    
    public bool Value
    {
        get => Interlocked.CompareExchange(ref _value, 0, 0) == 1;
        set => Interlocked.Exchange(ref _value, value ? 1 : 0);
    }
}

public class AtomicDateTime
{
    private long _value;
    
    public AtomicDateTime(DateTime value)
    {
        _value = value.ToBinary();
    }
    
    public DateTime Value
    {
        get => DateTime.FromBinary(Interlocked.Read(ref _value));
        set => Interlocked.Exchange(ref _value, value.ToBinary());
    }
} 