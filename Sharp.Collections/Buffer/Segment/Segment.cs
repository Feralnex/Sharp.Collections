namespace Sharp.Collections
{
    public class Segment<TItem>(int size) : Buffer<TItem>(size), ISegment<TItem>
    {
        public ISegment<TItem>? NextHead { get; set; }
        public ISegment<TItem>? NextTail { get; set; }
    }
}
