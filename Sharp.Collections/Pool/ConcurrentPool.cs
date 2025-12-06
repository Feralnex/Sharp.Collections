using System;

namespace Sharp.Collections
{
    public class ConcurrentPool<TElement> : Pool<TElement>
        where TElement : class
    {
        public override bool IsThreadSafe => true;

        public ConcurrentPool() : base() { }

        public ConcurrentPool(int segmentSize) : base(segmentSize) { }

        public ConcurrentPool(Func<TElement> onCreateNew) : base(onCreateNew) { }

        public ConcurrentPool(Func<TElement> onCreateNew, int segmentSize) : base(segmentSize, onCreateNew) { }

        protected override IQueue<TElement> InitializeQueue()
            => new ConcurrentQueue<TElement>();

        protected override IQueue<TElement> InitializeQueue(int segmentSize)
            => new ConcurrentQueue<TElement>(segmentSize);
    }
}
