using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sharp.Collections
{
    public class ConcurrentKeyedPool<TKey, TElement> : KeyedPool<TKey, TElement>
        where TKey : notnull
        where TElement : class
    {
        public override bool IsThreadSafe => true;

        public ConcurrentKeyedPool() : base() { }

        public ConcurrentKeyedPool(int segmentSize) : base(segmentSize) { }

        public ConcurrentKeyedPool(Func<TKey, TElement> onCreateNew) : base(onCreateNew) { }

        public ConcurrentKeyedPool(Func<TKey, TElement> onCreateNew, int segmentSize) : base(segmentSize, onCreateNew) { }

        protected override IDictionary<TKey, IQueue<TElement>> InitializeBuckets()
            => new ConcurrentDictionary<TKey, IQueue<TElement>>();

        protected override IQueue<TElement> InitializeQueue()
            => new ConcurrentQueue<TElement>();

        protected override IQueue<TElement> InitializeQueue(int segmentSize)
            => new ConcurrentQueue<TElement>(segmentSize);
    }
}
