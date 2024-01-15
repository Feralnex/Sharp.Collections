namespace Sharp.Collections
{
    public class Segments<TItem> : ISegments<TItem>
    {
        public virtual ISegment<TItem> Head { get; set; }
        public virtual ISegment<TItem> Tail { get; set; }

        public Segments(ISegment<TItem> root)
        {
            Head = Tail = root;
        }

        public void AddToHead(ISegment<TItem> segment)
        {
            Tail.NextHead = segment;
            Tail = segment;
        }

        public void AddToTail(ISegment<TItem> segment)
        {
            Tail.NextTail = segment;
            Tail = segment;
        }

        public void MoveToNextHead()
        {
            ISegment<TItem> nextHead = Head.NextHead!;

            Head.NextHead = null;
            Head = nextHead;
        }

        public void MoveToNextTail()
        {
            ISegment<TItem> nextTail = Head.NextTail!;

            Head.NextTail = null;
            Head = nextTail;
        }
    }
}
