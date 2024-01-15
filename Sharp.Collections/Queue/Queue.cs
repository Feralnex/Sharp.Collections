using System;

namespace Sharp.Collections
{
    public class Queue<TItem> : IQueue<TItem>
    {
        protected static readonly byte _defaultSegmentSize = 64;

        protected ISegments<TItem> Enqueues { get; private set; }
        protected ISegments<TItem> Dequeues { get; private set; }

        public virtual int Count { get; protected set; }

        public Queue() : this(_defaultSegmentSize) { }

        public Queue(int segmentSize)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(segmentSize, 1);

            ISegment<TItem> root = InitializeRoot(segmentSize);

            Enqueues = InitializeEnqueues(root);
            Dequeues = InitializeDequeues(root);
            Count = 0;
        }

        public void Enqueue(TItem item)
        {
            if (!TryEnqueue(item))
                throw new ArgumentNullException(nameof(item));
        }

        public virtual bool TryEnqueue(TItem item)
        {
            if (item is null)
                return false;

            if (!Enqueues.Head.TryWrite(item))
            {
                MoveToNextEnqueue();

                Enqueues.Head.Write(item);
            }

            Count++;

            return true;
        }

        public TItem Dequeue()
        {
            if (!TryDequeue(out TItem? item))
                throw new InvalidOperationException();

            return item!;
        }

        public virtual bool TryDequeue(out TItem? item)
        {
            bool dequeued = Dequeues.Head.TryRead(out item);

            if (!dequeued && Dequeues.Head.NextTail is not null)
            {
                MoveToNextDequeue();

                dequeued = Dequeues.Head.TryRead(out item);
            }

            if (dequeued)
                Count--;

            return dequeued;
        }

        public TItem Peek()
        {
            if (!TryPeek(out TItem? item))
                throw new InvalidOperationException();

            return item!;
        }

        public virtual bool TryPeek(out TItem? item)
        {
            bool peeked = Dequeues.Head.TryRead(out item, false);

            if (!peeked && Dequeues.Head.NextTail is not null)
            {
                MoveToNextDequeue();

                peeked = Dequeues.Head.TryRead(out item, false);
            }

            return peeked;
        }

        protected virtual ISegment<TItem> InitializeRoot(int segmentSize)
            => new Segment<TItem>(segmentSize);

        protected virtual ISegments<TItem> InitializeEnqueues(ISegment<TItem> root)
            => new Segments<TItem>(root);

        protected virtual ISegments<TItem> InitializeDequeues(ISegment<TItem> root)
            => new Segments<TItem>(root);

        private void MoveToNextEnqueue()
        {
            ISegment<TItem>? nextHead = Enqueues.Head.NextHead;

            if (nextHead is null)
            {
                nextHead = new Segment<TItem>(Enqueues.Tail.Size);

                Enqueues.AddToHead(nextHead);
            }

            Enqueues.MoveToNextHead();
            Dequeues.AddToTail(nextHead);
        }

        private void MoveToNextDequeue()
        {
            ISegment<TItem>? oldHead = Dequeues.Head;
            
            Dequeues.MoveToNextTail();
            Enqueues.AddToHead(oldHead); ;
        }
    }
}