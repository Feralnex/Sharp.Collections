namespace Sharp.Collections
{
    public interface ISegments<TItem>
    {
        ISegment<TItem> Head { get; }
        ISegment<TItem> Tail { get; }

        void AddToHead(ISegment<TItem> segment);
        void AddToTail(ISegment<TItem> segment);
        void MoveToNextHead();
        void MoveToNextTail();
    }
}
