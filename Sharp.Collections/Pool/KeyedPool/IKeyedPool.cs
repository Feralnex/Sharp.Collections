using System;

namespace Sharp.Collections
{
    public interface IKeyedPool
    {
        bool IsThreadSafe { get; }

        int Count(object key);
        object Acquire(object key);
        object Acquire(object key, Func<object, object> createNewOverride);
        bool TryAcquire(object key, out object? element);
        bool TryAcquire(object key, Func<object, object> createNewOverride, out object? element);
        void Release(object key, object element);
        bool TryRelease(object key, object element);
    }

    public interface IKeyedPool<TKey, TElement> : IKeyedPool
        where TKey : notnull
        where TElement : class
    {
        int Count(TKey key);
        TElement Acquire(TKey key);
        TElement Acquire(TKey key, Func<TKey, TElement> createNewOverride);
        bool TryAcquire(TKey key, out TElement? element);
        bool TryAcquire(TKey key, Func<TKey, TElement> createNewOverride, out TElement? element);
        void Release(TKey key, TElement element);
        bool TryRelease(TKey key, TElement element);
    }
}
