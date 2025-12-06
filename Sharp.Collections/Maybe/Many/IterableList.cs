using CommunityToolkit.HighPerformance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sharp.Collections
{
    public abstract partial class Selection<TTarget>
    {
        protected partial class Many : Maybe<TTarget>
        {
            private class IterableList : IList<TTarget>
            {
                public event Action<int> Added
                {
                    add => _onAddedListeners.Add(value);
                    remove => _onAddedListeners.Remove(value);
                }
                public event Action<int> Removed
                {
                    add => _onRemovedListeners.Add(value);
                    remove => _onRemovedListeners.Remove(value);
                }

                private List<TTarget> _list;
                private List<Action<int>> _onAddedListeners;
                private List<Action<int>> _onRemovedListeners;

                public List<TTarget> Source => _list;
                public int Count => _list.Count;

                public bool IsReadOnly => false;

                public TTarget this[int index]
                {
                    get => _list[index];
                    set => _list[index] = value;
                }

                public IterableList()
                {
                    _list = new List<TTarget>();
                    _onAddedListeners = new List<Action<int>>();
                    _onRemovedListeners = new List<Action<int>>();
                }

                public IterableList(int capacity)
                {
                    _list = new List<TTarget>(capacity);
                    _onAddedListeners = new List<Action<int>>();
                    _onRemovedListeners = new List<Action<int>>();
                }

                public IterableList(List<TTarget> list)
                {
                    _list = list;
                    _onAddedListeners = new List<Action<int>>();
                    _onRemovedListeners = new List<Action<int>>();
                }

                public IterableList(IEnumerable<TTarget> enumerable)
                {
                    _list = new List<TTarget>(enumerable);
                    _onAddedListeners = new List<Action<int>>();
                    _onRemovedListeners = new List<Action<int>>();
                }

                public int IndexOf(TTarget item)
                    => _list.IndexOf(item);

                public void Add(TTarget item)
                {
                    _list.Add(item);

                    Span<Action<int>> listeners = CollectionsMarshal.AsSpan(_onAddedListeners);

                    for (int listenerIndex = 0; listenerIndex < listeners.Length; listenerIndex++)
                        listeners.DangerousGetReferenceAt(listenerIndex)(_list.Count - 1);
                }

                public void Insert(int index, TTarget item)
                {
                    if (index >= 0 && index < _list.Count)
                    {
                        _list.Insert(index, item);

                        Span<Action<int>> listeners = CollectionsMarshal.AsSpan(_onAddedListeners);

                        for (int listenerIndex = 0; listenerIndex < listeners.Length; listenerIndex++)
                            listeners.DangerousGetReferenceAt(listenerIndex)(index);
                    }
                }

                public bool Remove(TTarget item)
                {
                    int index = IndexOf(item);

                    if (index >= 0)
                    {
                        RemoveAt(index);

                        Span<Action<int>> listeners = CollectionsMarshal.AsSpan(_onRemovedListeners);

                        for (int listenerIndex = 0; listenerIndex < listeners.Length; listenerIndex++)
                            listeners.DangerousGetReferenceAt(listenerIndex)(index);

                        return true;
                    }

                    return false;
                }

                public void RemoveAt(int index)
                {
                    if (index >= 0 && index < _list.Count)
                    {
                        _list.RemoveAt(index);

                        Span<Action<int>> listeners = CollectionsMarshal.AsSpan(_onRemovedListeners);

                        for (int listenerIndex = 0; listenerIndex < listeners.Length; listenerIndex++)
                            listeners.DangerousGetReferenceAt(listenerIndex)(index);
                    }
                }

                public void Clear()
                {
                    int count = Count;

                    _list.Clear();

                    if (count > 0)
                    {
                        Span<Action<int>> listeners = CollectionsMarshal.AsSpan(_onRemovedListeners);

                        for (int listenerIndex = 0; listenerIndex < listeners.Length; listenerIndex++)
                            for (int index = 0; index < count; index++)
                                listeners.DangerousGetReferenceAt(listenerIndex)(index);
                    }
                }

                public bool Contains(TTarget item)
                    => _list.Contains(item);

                public void CopyTo(TTarget[] array, int arrayIndex)
                    => _list.CopyTo(array, arrayIndex);

                public Span<TTarget> AsSpan()
                    => CollectionsMarshal.AsSpan(_list);

                public ReadOnlySpan<TTarget> AsReadOnlySpan()
                    => CollectionsMarshal.AsSpan(_list);

                public IEnumerator<TTarget> GetEnumerator()
                    => _list.GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator()
                    => _list.GetEnumerator();
            }
        }
    }
}
