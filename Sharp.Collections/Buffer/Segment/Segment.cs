using System;

namespace Sharp.Collections
{
    public class Segment<TItem> : Buffer<TItem>, ISegment<TItem>
    {
        public ISegment<TItem>? NextHead { get; set; }
        public ISegment<TItem>? NextTail { get; set; }

        public Segment() : base(Defaults.SegmentSize) { }

        public Segment(int size) : base(size) { }
    }
}
