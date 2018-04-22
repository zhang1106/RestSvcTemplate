using System;
using System.Collections.Generic;

namespace AiDollar.ApiGateway.Cache
{
    public interface ICache<in TKey, TValue>
    {
        void Add(TKey key, TValue value);
        void Replace(TKey key, TValue value);
        TValue Get(TKey key);
        IDisposable AcquireReadAccess();
        IDisposable AcquireWriteAccess();
        IReadOnlyList<TValue> GetItems();
    }
}