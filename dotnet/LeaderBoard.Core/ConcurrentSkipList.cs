using System.Collections.Concurrent;

namespace LeaderBoard.Core;

public class ConcurrentSkipList<T> where T : IComparable<T>
{
    private readonly int _maxLevel;
    private readonly Random _random;
    private volatile SkipListNode<T> _head;
    private readonly ConcurrentDictionary<T, SkipListNode<T>> _nodeMap;
    
    public ConcurrentSkipList(int maxLevel = 32)
    {
        _maxLevel = maxLevel;
        _random = new Random();
        _head = new SkipListNode<T>(default!, maxLevel);
        _nodeMap = new ConcurrentDictionary<T, SkipListNode<T>>();
        
        for (int i = 0; i < maxLevel; i++)
        {
            _head.Forward[i] = null;
        }
    }
    
    public bool Insert(T item)
    {
        var newLevel = GenerateRandomLevel();
        var newNode = new SkipListNode<T>(item, newLevel);
        var update = new SkipListNode<T>[_maxLevel];
        
        var current = _head;
        for (int i = _maxLevel - 1; i >= 0; i--)
        {
            while (current.Forward[i] != null && 
                   current.Forward[i].Data.CompareTo(item) < 0)
            {
                current = current.Forward[i];
            }
            update[i] = current;
        }
        
        for (int i = 0; i < newLevel; i++)
        {
            newNode.Forward[i] = update[i].Forward[i];
            update[i].Forward[i] = newNode;
        }
        
        return _nodeMap.TryAdd(item, newNode);
    }
    
    public IEnumerable<T> GetRange(int start, int count)
    {
        var current = _head.Forward[0];
        var index = 0;
        
        while (current != null && index < start)
        {
            current = current.Forward[0];
            index++;
        }
        
        var result = new List<T>(count);
        while (current != null && result.Count < count)
        {
            result.Add(current.Data);
            current = current.Forward[0];
        }
        
        return result;
    }
    
    private int GenerateRandomLevel()
    {
        var level = 1;
        while (_random.NextDouble() < 0.5 && level < _maxLevel)
        {
            level++;
        }
        return level;
    }
}

public class SkipListNode<T>
{
    public T Data { get; }
    public SkipListNode<T>?[] Forward { get; }
    
    public SkipListNode(T data, int level)
    {
        Data = data;
        Forward = new SkipListNode<T>?[level];
    }
} 