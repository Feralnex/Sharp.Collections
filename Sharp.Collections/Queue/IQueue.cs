namespace Sharp.Collections
{
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
}
