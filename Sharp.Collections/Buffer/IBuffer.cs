namespace Sharp.Collections
{
    public interface IBuffer<TItem>
    {
        int Head { get; }
        int Tail { get; }
        int Count { get; }
        int Size { get; }

        void Write(TItem item, bool moveTail = true);
        bool TryWrite(TItem item, bool moveTail = true);
        TItem Read(bool moveHead = true);
        bool TryRead(out TItem? item, bool moveHead = true);
    }
}
