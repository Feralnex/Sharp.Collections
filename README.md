# About
Sharp.Collections is a .Net 8.0 library that contains implementations of custom collections.
- `Buffer<TItem>`
- `Segment<TItem>`
- `ConcurrentSegment<TItem>`
- `Segments<TItem>`
- `Queue<TItem>`
- `ConcurrentQueue<TItem>`

## Buffer\<TItem\> : IBuffer\<TItem\>
A simple fixed-size collection that writes elements at the Tail position and reads elements at the Head position.

```cs
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
```

## Segment\<TItem\> : Buffer\<TItem\>, ISegment\<TItem\>
An extended buffer with the NextHead and NextTail properties.

```cs
public interface ISegment<TItem> : IBuffer<TItem>
{
    ISegment<TItem>? NextHead { get; set; }
    ISegment<TItem>? NextTail { get; set; }
}
```

## ConcurrentSegment\<TItem\> : Segment\<TItem\>
A segment adapted to a concurrent environment.

## Segments\<TItem\> : ISegments<\TItem\>
A collection of two unidirectional branches of stored segments used in a custom queue implementation.

```cs
public interface ISegments<TItem>
{
    ISegment<TItem> Head { get; }
    ISegment<TItem> Tail { get; }

    void AddToHead(ISegment<TItem> segment);
    void AddToTail(ISegment<TItem> segment);
    void MoveToNextHead();
    void MoveToNextTail();
}
```

## Queue\<TItem\> : IQueue\<TItem\>
A custom queue implementation focusing on more efficient use of allocated memory. It consists of segments that are reused or new ones are allocated in case of memory shortage. To summarize, it is a queue implementation that reuses allocated memory without deallocating it - it only grows and never shrinks.

```cs
public interface IQueue<TItem>
{
    int Count { get; }

    void Enqueue(TItem item);
    bool TryEnqueue(TItem item);
    TItem Dequeue();
    bool TryDequeue(out TItem? item);
    TItem Peek();
    bool TryPeek(out TItem? item);
}
```

## ConcurrentQueue\<TItem\> : Queue\<TItem\>
A queue adapted to a concurrent environment.