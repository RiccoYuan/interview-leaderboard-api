namespace LeaderBoard.Core;

public class SegmentedLockManager
{
    private readonly ReaderWriterLockSlim[] _locks;
    private readonly int _segmentCount;
    
    public SegmentedLockManager()
    {
        _segmentCount = Environment.ProcessorCount * 4;
        _locks = new ReaderWriterLockSlim[_segmentCount];
        
        for (int i = 0; i < _segmentCount; i++)
        {
            _locks[i] = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }
    }
    
    public IDisposable AcquireReadLock(long customerId)
    {
        var lockIndex = Math.Abs(customerId.GetHashCode()) % _segmentCount;
        return new ReadLockScope(_locks[lockIndex]);
    }
    
    public IDisposable AcquireWriteLock(long customerId)
    {
        var lockIndex = Math.Abs(customerId.GetHashCode()) % _segmentCount;
        return new WriteLockScope(_locks[lockIndex]);
    }
    
    private class ReadLockScope : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock;
        
        public ReadLockScope(ReaderWriterLockSlim @lock)
        {
            _lock = @lock;
            _lock.EnterReadLock();
        }
        
        public void Dispose()
        {
            _lock.ExitReadLock();
        }
    }
    
    private class WriteLockScope : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock;
        
        public WriteLockScope(ReaderWriterLockSlim @lock)
        {
            _lock = @lock;
            _lock.EnterWriteLock();
        }
        
        public void Dispose()
        {
            _lock.ExitWriteLock();
        }
    }
} 