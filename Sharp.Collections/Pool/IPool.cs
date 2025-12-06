using System;

namespace Sharp.Collections
{
    public interface IPool
    {
        bool IsThreadSafe { get; }
        int Count { get; }

        object? Acquire();
        object Acquire(Func<object> createNewOverride);
        bool TryAcquire(out object? element);
        bool TryAcquire(out object? element, Func<object> createNewOverride);
        void Release(object element);
        bool TryRelease(object element);
    }

    public interface IPool<TElement> : IPool
        where TElement : class
    {
        new TElement? Acquire();
        TElement Acquire(Func<TElement> createNewOverride);
        bool TryAcquire(out TElement? element);
        bool TryAcquire(out TElement? element, Func<TElement> createNewOverride);
        void Release(TElement element);
        bool TryRelease(TElement element);
    }
}
