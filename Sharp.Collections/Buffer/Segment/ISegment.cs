namespace Sharp.Collections
{
    public interface ISegment<TItem> : IBuffer<TItem>
    {
        ISegment<TItem>? NextHead { get; set; }
        ISegment<TItem>? NextTail { get; set; }
    }
}
