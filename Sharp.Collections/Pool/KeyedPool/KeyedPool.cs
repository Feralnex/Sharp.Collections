using Sharp.Collections.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sharp.Collections
{
    public class KeyedPool<TKey, TElement> : IKeyedPool<TKey, TElement>
        where TKey : notnull
        where TElement : class
    {
        private static Reference<Func<TElement>> _creator;

        private readonly Reference<Func<TKey, TElement>> _createNew;

        protected IDictionary<TKey, IQueue<TElement>> Buckets { get; }
        protected Value<int> SegmentSize { get; }

        public virtual bool IsThreadSafe => false;

        public KeyedPool()
        {
            _createNew = new Reference<Func<TKey, TElement>>();
            Buckets = InitializeBuckets();
            SegmentSize = new Value<int>();
        }

        public KeyedPool(int segmentSize) : this()
        {
            SegmentSize.Set(segmentSize);
        }

        public KeyedPool(Func<TKey, TElement> onCreateNew) : this()
            => _createNew.Set(onCreateNew);

        public KeyedPool(int segmentSize, Func<TKey, TElement> onCreateNew) : this(segmentSize)
            => _createNew.Set(onCreateNew);

        static KeyedPool()
        {
            ConstructorInfo? constructorInfo = typeof(TElement).GetConstructor(Type.EmptyTypes);

            _creator = new Reference<Func<TElement>>();

            if (constructorInfo is not null)
            {
                Func<TElement> creator = Expression
                    .Lambda<Func<TElement>>(Expression.New(constructorInfo))
                    .Compile();

                _creator.Set(creator);
            }
        }

        public int Count(TKey key)
        {
            if (Buckets.TryGetValue(key, out IQueue<TElement>? queue))
                return queue.Count;

            return 0;
        }

        public TElement Acquire(TKey key)
        {
            IQueue<TElement> queue = Buckets.GetOrAdd(key, OnQueueMissing);

            if (queue.TryDequeue(out TElement? element))
                return element!;
            else if (_createNew.TryGet(out Func<TKey, TElement>? createNew))
                return createNew!(key);
            else if (_creator.TryGet(out Func<TElement>? creator))
                return creator!();

            throw new InvalidOperationException();
        }

        public TElement Acquire(TKey key, Func<TKey, TElement> createNewOverride)
        {
            IQueue<TElement> queue = Buckets.GetOrAdd(key, OnQueueMissing);

            if (!queue.TryDequeue(out TElement? element))
                element = createNewOverride(key);

            return element!;
        }

        public bool TryAcquire(TKey key, out TElement? element)
        {
            bool acquired = false;
            IQueue<TElement> queue = Buckets.GetOrAdd(key, OnQueueMissing);

            if (queue.TryDequeue(out element))
            {
                acquired = true;
            }
            else if (_createNew.TryGet(out Func<TKey, TElement>? createNew))
            {
                element = createNew!(key);
                acquired = true;
            }
            else if (_creator.TryGet(out Func<TElement>? creator))
            {
                element = creator!();
                acquired = true;
            }

            return acquired;
        }

        public bool TryAcquire(TKey key, Func<TKey, TElement> createNewOverride, out TElement? element)
        {
            IQueue<TElement> queue = Buckets.GetOrAdd(key, OnQueueMissing);

            if (!queue.TryDequeue(out TElement? acquiredElement))
                acquiredElement = createNewOverride(key);

            element = acquiredElement;

            return true;
        }

        public void Release(TKey key, TElement element)
        {
            IQueue<TElement> queue = Buckets.GetOrAdd(key, OnQueueMissing);

            queue.Enqueue(element);
        }

        public bool TryRelease(TKey key, TElement element)
        {
            IQueue<TElement> queue = Buckets.GetOrAdd(key, OnQueueMissing);

            return queue.TryEnqueue(element);
        }

        protected virtual IDictionary<TKey, IQueue<TElement>> InitializeBuckets()
            => new Dictionary<TKey, IQueue<TElement>>();

        protected virtual IQueue<TElement> InitializeQueue()
            => new Queue<TElement>();

        protected virtual IQueue<TElement> InitializeQueue(int segmentSize)
            => new Queue<TElement>(segmentSize);

        private IQueue<TElement> OnQueueMissing()
            => SegmentSize.Match(OnSomeSegmentSize, OnNoneSegmentSize);

        private IQueue<TElement> OnSomeSegmentSize()
            => InitializeQueue(SegmentSize.Target);

        private IQueue<TElement> OnNoneSegmentSize()
            => InitializeQueue();

        #region IKeyedPool

        int IKeyedPool.Count(object key)
        {
            if (key is not TKey typedKey)
                throw new InvalidOperationException();

            IQueue<TElement> queue = Buckets.GetOrAdd(typedKey, OnQueueMissing);

            return 0;
        }

        object IKeyedPool.Acquire(object key)
        {
            if (key is not TKey typedKey)
                throw new InvalidOperationException();

            return Acquire(typedKey);
        }

        object IKeyedPool.Acquire(object key, Func<object, object> createNewOverride)
        {
            if (key is not TKey typedKey)
                throw new InvalidOperationException();

            if (createNewOverride is not Func<TKey, TElement> createNew)
                throw new InvalidOperationException();

            return Acquire(typedKey, createNew);
        }

        bool IKeyedPool.TryAcquire(object key, out object? element)
        {
            element = default;

            if (key is not TKey typedKey)
                return false;

            if (!TryAcquire(typedKey, out TElement? acquiredElement))
                return false;

            element = acquiredElement;

            return true;
        }

        bool IKeyedPool.TryAcquire(object key, Func<object, object> createNewOverride, out object? element)
        {
            element = default;

            if (key is not TKey typedKey)
                return false;

            IQueue<TElement> queue = Buckets.GetOrAdd(typedKey, OnQueueMissing);

            if (!queue.TryDequeue(out TElement? acquiredElement))
            {
                if (createNewOverride is not Func<TKey, TElement> createNew)
                    return false;

                acquiredElement = createNew(typedKey);
            }

            element = acquiredElement;

            return true;
        }

        void IKeyedPool.Release(object key, object element)
        {
            if (key is not TKey typedKey)
                throw new InvalidOperationException();

            if (element is not TElement)
                throw new InvalidOperationException();

            TElement elementToRelease = Unsafe.As<TElement>(element);

            Release(typedKey, elementToRelease);
        }

        bool IKeyedPool.TryRelease(object key, object element)
        {
            if (key is not TKey typedKey)
                return false;

            if (element is not TElement)
                return false;

            TElement elementToRelease = Unsafe.As<TElement>(element);

            return TryRelease(typedKey, elementToRelease);
        }

        #endregion IKeyedPool
    }
}
