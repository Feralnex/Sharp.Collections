namespace Sharp.Collections.Extensions
{
    public static partial class ListExtensions
    {
        private class UnsafeList<TTarget>
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
            private TTarget[] _items;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
            private int _size;
            private int _version;

            public TTarget[] Items
            {
                get => _items;
                set => _items = value;
            }
            public int Count
            {
                get => _size;
                set => _size = value;
            }
            public int Version
            {
                get => _version;
                set => _version = value;
            }
        }
    }
}
