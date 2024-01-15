using System;

namespace Sharp.Collections
{
    public class Buffer<TItem> : IBuffer<TItem>
    {
        protected TItem?[] Items { get; set; }

        public virtual int Head { get; protected set; }
        public virtual int Tail { get; protected set; }
        public int Count => Tail - Head;
        public int Size => Items.Length;

        public Buffer(int size)
        {
            Head = Tail;
            Items = new TItem?[size];
        }

        public virtual void Write(TItem item, bool moveTail = true)
        {
            if (!TryWrite(item, moveTail))
                throw new IndexOutOfRangeException();
        }

        public virtual bool TryWrite(TItem item, bool moveTail = true)
        {
            if ((Tail - Size) == Head)
                return false;

            int index = Tail % Size;
            Items[index] = item;

            if (moveTail)
                Tail++;

            return true;
        }

        public virtual TItem Read(bool moveHead = true)
        {
            if (!TryRead(out TItem? item, moveHead))
                throw new InvalidOperationException();

            return item!;
        }

        public virtual bool TryRead(out TItem? item, bool moveHead = true)
        {
            item = default;

            if (Head == Tail)
                return false;

            int index = Head % Size;
            item = Items[index];

            if (moveHead)
                Head++;

            return true;
        }
    }
}
