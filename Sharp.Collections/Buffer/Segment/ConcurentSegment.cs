using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sharp.Collections
{
    public class ConcurrentSegment<TItem> : Segment<TItem>
    {
        private int _head;
        private int _tail;
        private bool _frozenForEnqueues;
        private readonly int _freezeOffset;
        private int _sequence;
        private readonly int[] _sequences;

        public override int Head
        {
            get => _head;
            protected set => _head = value;
        }
        public override int Tail
        {
            get => _tail;
            protected set => _tail = value;
        }
        public int Sequence => _sequence;

        public ConcurrentSegment(int size) : this(size, 0) { }

        public ConcurrentSegment(int size, int sequence) : base(size)
        {
            _sequences = new int[size];
            _frozenForEnqueues = false;
            _freezeOffset = Size * 2;
            _sequence = sequence;

            for (int index = 0; index < _sequences.Length; index++)
                _sequences[index] = index;
        }

        public void FreezeForEnqueue()
        {
            _frozenForEnqueues = true;
            Interlocked.Add(ref _tail, _freezeOffset);
        }

        public void UnfreezeForEnqueue(int nextSequence)
        {
            _frozenForEnqueues = false;
            Interlocked.Add(ref _tail, -_freezeOffset);
            Interlocked.Exchange(ref _sequence, nextSequence);
        }

        public override bool TryWrite(TItem item, bool moveTail = true)
        {
            int[] sequences = _sequences;

            while (true)
            {
                int currentTail = Volatile.Read(ref _tail);
                int index = currentTail % Size;
                int sequence = Volatile.Read(ref sequences[index]);
                int diff = sequence - currentTail;

                if (diff == 0)
                {
                    if (moveTail)
                    {
                        if (Interlocked.CompareExchange(ref _tail, currentTail + 1, currentTail) == currentTail)
                        {
                            Items[index] = item;

                            Volatile.Write(ref sequences[index], currentTail + 1);

                            return true;
                        }
                    }
                    else
                    {
                        if (Interlocked.CompareExchange(ref _tail, currentTail + _freezeOffset, currentTail) == currentTail)
                        {
                            Items[index] = item;

                            Interlocked.Add(ref _tail, -_freezeOffset);

                            return true;
                        }
                    }
                }
                else if (diff < 0)
                    return false;
            }
        }

        public override bool TryRead([MaybeNullWhen(false)] out TItem item, bool moveTail = true)
        {
            int[] sequences = _sequences;
            SpinWait spinner = default;

            while (true)
            {
                int currentHead = Volatile.Read(ref _head);
                int index = currentHead % Size;
                int sequence = Volatile.Read(ref sequences[index]);
                int diff = sequence - (currentHead + 1);

                if (diff == 0)
                {
                    if (moveTail)
                    {
                        if (Interlocked.CompareExchange(ref _head, currentHead + 1, currentHead) == currentHead)
                        {
                            item = Items[index]!;

                            if (RuntimeHelpers.IsReferenceOrContainsReferences<TItem>())
                                Items[index] = default;

                            Volatile.Write(ref sequences[index], currentHead + sequences.Length);

                            return true;
                        }
                    }
                    else
                    {
                        if (Interlocked.CompareExchange(ref _head, currentHead + _freezeOffset, currentHead) == currentHead)
                        {
                            item = Items[index]!;

                            Interlocked.Add(ref _head, -_freezeOffset);

                            return true;
                        }
                    }
                }
                else if (diff < 0)
                {
                    bool frozen = Volatile.Read(ref _frozenForEnqueues);
                    int currentTail = Volatile.Read(ref _tail);
                    if (currentTail - currentHead <= 0 || (frozen && (currentTail - _freezeOffset - currentHead <= 0)))
                    {
                        item = default;
                        return false;
                    }

                    spinner.SpinOnce(sleep1Threshold: -1);
                }
            }
        }
    }
}
