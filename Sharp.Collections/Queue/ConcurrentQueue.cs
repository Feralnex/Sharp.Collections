using System.Runtime.CompilerServices;
using System.Threading;

namespace Sharp.Collections
{
    public class ConcurrentQueue<TItem> : Queue<TItem>
    {
        private int _head;
        private int _tail;
        private int _count;

        public override int Count
        {
            get => _count;
            protected set => _count = value;
        }

        public ConcurrentQueue() : base() { }

        public ConcurrentQueue(int segmentSize) : base(segmentSize) { }

        public override bool TryEnqueue(TItem item)
        {
            if (item is null)
                return false;

            SpinWait spinner = default;

            while (true)
            {
                ConcurrentSegment<TItem> currentSegment = Unsafe.As<ConcurrentSegment<TItem>>(Enqueues.Head);
                int currentTail = Volatile.Read(ref _tail);
                int sequence = currentSegment.Sequence;
                int diff = sequence - currentTail;

                if (diff == 0)
                {
                    if (!currentSegment.TryWrite(item))
                        MoveToNextEnqueue(spinner, currentSegment, currentTail);
                    else
                    {
                        Interlocked.Increment(ref _count);

                        return true;
                    }
                }
                else if (diff < 0)
                    spinner.SpinOnce(sleep1Threshold: 5);
            }
        }

        public override bool TryDequeue(out TItem? item)
        {
            SpinWait spinner = default;

            while (true)
            {
                ConcurrentSegment<TItem> currentSegment = Unsafe.As<ConcurrentSegment<TItem>>(Dequeues.Head);
                int currentHead = Volatile.Read(ref _head);
                int sequence = currentSegment.Sequence;
                int diff = sequence - currentHead;

                if (diff == 0)
                {
                    if (!currentSegment.TryRead(out item))
                    {
                        if (!TryMoveToNextDequeue(spinner, currentSegment, currentHead))
                            return false;
                    }
                    else
                    {
                        Interlocked.Decrement(ref _count);

                        return true;
                    }
                }
                else if (diff < 0)
                    spinner.SpinOnce(sleep1Threshold: 5);
            }
        }

        public override bool TryPeek(out TItem? item)
        {
            SpinWait spinner = default;

            while (true)
            {
                ConcurrentSegment<TItem> currentSegment = Unsafe.As<ConcurrentSegment<TItem>>(Dequeues.Head);
                int currentHead = Volatile.Read(ref _head);
                int sequence = currentSegment.Sequence;
                int diff = sequence - currentHead;

                if (diff == 0)
                {
                    if (!currentSegment.TryRead(out item, false))
                    {
                        if (!TryMoveToNextDequeue(spinner, currentSegment, currentHead))
                            return false;
                    }
                    else
                        return true;
                }
                else if (diff < 0)
                    spinner.SpinOnce(sleep1Threshold: 5);
            }
        }

        protected override ISegment<TItem> InitializeRoot(int segmentSize)
            => new ConcurrentSegment<TItem>(segmentSize);

        private void MoveToNextEnqueue(SpinWait spinner, ConcurrentSegment<TItem> currentSegment, int currentTail)
        {
            int nextSequence = currentTail + 1;

            if (Interlocked.CompareExchange(ref _tail, nextSequence, currentTail) == currentTail)
            {
                ConcurrentSegment<TItem>? nextTail = Unsafe.As<ConcurrentSegment<TItem>>(currentSegment.NextTail);

                currentSegment.FreezeForEnqueue();

                if (nextTail is null)
                {
                    nextTail = new ConcurrentSegment<TItem>(Enqueues.Tail.Size, currentTail);
                    nextTail.FreezeForEnqueue();

                    Enqueues.AddToTail(nextTail);
                }

                nextTail = Unsafe.As<ConcurrentSegment<TItem>>(currentSegment.NextTail);
                Enqueues.MoveToNextTail();
                Dequeues.AddToHead(nextTail!);
                nextTail!.UnfreezeForEnqueue(nextSequence);
            }
            else
                spinner.SpinOnce(sleep1Threshold: 5);
        }

        private bool TryMoveToNextDequeue(SpinWait spinner, ConcurrentSegment<TItem> currentSegment, int currentHead)
        {
            int nextSequence = currentHead + 1;

            if (Interlocked.CompareExchange(ref _head, nextSequence, currentHead) == currentHead)
            {
                ConcurrentSegment<TItem>? nextHead = Unsafe.As<ConcurrentSegment<TItem>>(currentSegment.NextHead);

                if (nextHead is null)
                {
                    _head--;

                    return false;
                }

                Dequeues.MoveToNextHead();
                Enqueues.AddToTail(currentSegment);                
            }
            else
                spinner.SpinOnce(sleep1Threshold: 5);

            return true;
        }
    }
}