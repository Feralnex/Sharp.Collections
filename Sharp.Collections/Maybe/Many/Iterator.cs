namespace Sharp.Collections
{
    public abstract partial class Selection<TTarget>
    {
        protected partial class Many : Maybe<TTarget>
        {
            private class Iterator
            {
                public int Index { get; set; }
                public int Length { get; set; }

                public void Subscribe(IterableList list)
                {
                    list.Added += OnAdded;
                    list.Removed += OnRemoved;
                }

                public void Unsubscribe(IterableList list)
                {
                    list.Added -= OnAdded;
                    list.Removed -= OnRemoved;
                }

                protected virtual void OnAdded(int index)
                {
                    if (index <= Index)
                        Index++;

                    Length++;
                }

                protected virtual void OnRemoved(int index)
                {
                    if (index <= Index)
                        Index--;

                    Length--;
                }
            }
        }
    }
}
