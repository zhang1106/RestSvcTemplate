using System;
using System.Collections.Generic;
using System.Linq;
using AiDollar.Infrastructure.Threading;

namespace AiDollar.ApiGateway.Cache
{
    public class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly FastReaderWriterLock _rwLock =  new FastReaderWriterLock();
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public IDisposable AcquireReadAccess()
        {
            return _rwLock.EnterRead();
        }

        public IDisposable AcquireWriteAccess()
        {
            return _rwLock.EnterWrite();
        }

        public void Add(TKey key, TValue value)
        {
            _cache.Add(key, value);
        }

        public void Replace(TKey key, TValue value)
        {
            _cache[key] = value;
        }

        public TValue Get(TKey key)
        {
            TValue val;
            if (!_cache.TryGetValue(key, out val))
            {
                val = default(TValue);
            }

            return val;
        }

        public IReadOnlyList<TValue> GetItems()
        {
            return _cache.Values.ToArray();
        }
    }
}
