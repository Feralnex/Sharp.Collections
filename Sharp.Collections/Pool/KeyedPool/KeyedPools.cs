using CommunityToolkit.HighPerformance;
using Sharp.Collections.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sharp.Collections
{
    public static class KeyedPools
    {
        public delegate bool TrySelector<TKey, TElement>(ReadOnlySpan<IKeyedPool<TKey, TElement>> span, out IKeyedPool<TKey, TElement>? pool)
            where TKey : notnull
            where TElement : class;

        private static readonly Lazy<Dictionary<Handle, List<IKeyedPool>>> _pools;
        private static readonly ReaderWriterLockSlim _lockSlim;

        // Composite key for dictionary
        public sealed record Handle(Type KeyType, Type ElementType);

        static KeyedPools()
        {
            _pools = new Lazy<Dictionary<Handle, List<IKeyedPool>>>(Create);
            _lockSlim = new ReaderWriterLockSlim();
        }

        private static Dictionary<Handle, List<IKeyedPool>> Create()
            => new Dictionary<Handle, List<IKeyedPool>>();

        private static Handle GetHandle<TKey, TElement>()
            where TKey : notnull
            where TElement : class
            => new Handle(typeof(TKey), typeof(TElement));

        public static void Add<TKey, TElement>(IKeyedPool<TKey, TElement> pool)
            where TKey : notnull
            where TElement : class
        {
            Handle handle = new Handle(typeof(TKey), typeof(TElement));

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                if (!Contains(pools, pool))
                    pools.Add(pool);
            }
            else
            {
                pools = new List<IKeyedPool>() { pool };

                _pools.Value.Add(handle, pools);
            }

            _lockSlim.ExitWriteLock();
        }

        public static bool TryAdd<TKey, TElement>(IKeyedPool<TKey, TElement> pool)
            where TKey : notnull
            where TElement : class
        {
            bool added = false;
            Handle handle = GetHandle<TKey, TElement>();

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                if (!Contains(pools, pool))
                {
                    pools.Add(pool);

                    added = true;
                }
            }
            else
            {
                pools = new List<IKeyedPool>() { pool };

                _pools.Value.Add(handle, pools);

                added = true;
            }

            _lockSlim.ExitWriteLock();

            return added;
        }

        public static bool Remove<TKey, TElement>(IKeyedPool<TKey, TElement> pool)
            where TKey : notnull
            where TElement : class
        {
            bool removed = false;
            Handle handle = GetHandle<TKey, TElement>();

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                removed = pools.Remove(pool);

                if (pools.Count == 0)
                    _pools.Value.Remove(handle);
            }

            _lockSlim.ExitWriteLock();

            return removed;
        }

        public static int RemoveAll<TKey, TElement>()
            where TKey : notnull
            where TElement : class
        {
            int removedCount = 0;
            Handle handle = GetHandle<TKey, TElement>();

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                removedCount = pools.Count;

                pools.Clear();
                _pools.Value.Remove(handle);
            }

            _lockSlim.ExitWriteLock();

            return removedCount;
        }

        public static int RemoveAll<TKey, TElement>(Predicate<IKeyedPool<TKey, TElement>> match)
            where TKey : notnull
            where TElement : class
        {
            int removedCount = 0;
            Handle handle = GetHandle<TKey, TElement>();

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                ref IKeyedPool<TKey, TElement> reference = ref Unsafe.As<IKeyedPool, IKeyedPool<TKey, TElement>>(ref pools.GetItems().DangerousGetReferenceAt(0));
                ReadOnlySpan<IKeyedPool<TKey, TElement>> span = MemoryMarshal.CreateSpan(ref reference, pools.Count);

                for (int index = 0; index < pools.Count; index++)
                {
                    IKeyedPool<TKey, TElement> pool = span.DangerousGetReferenceAt(index);

                    if (match(pool))
                    {
                        pools.Remove(pool);

                        removedCount++;
                    }
                }

                if (pools.Count == 0)
                    _pools.Value.Remove(handle);
            }

            _lockSlim.ExitWriteLock();

            return removedCount;
        }

        public static int RemoveAll(Predicate<Handle> typeMatch, Predicate<IKeyedPool> poolMatch)
        {
            int removedCount = 0;

            _lockSlim.EnterWriteLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Handle, List<IKeyedPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Handle, List<IKeyedPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Handle? handle = entry.key;

                    if (typeMatch(handle!))
                    {
                        List<IKeyedPool> pools = entry.value;

                        removedCount += pools.RemoveAll(poolMatch);

                        if (pools.Count == 0)
                            _pools.Value.Remove(entry.key);
                    }
                }
            }

            _lockSlim.ExitWriteLock();

            return removedCount;
        }

        public static bool Contains<TKey, TElement>(IKeyedPool<TKey, TElement> pool)
            where TKey : notnull
            where TElement : class
        {
            Handle handle = GetHandle<TKey, TElement>();

            _lockSlim.EnterReadLock();

            bool result = _pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools)
                && pools.Contains(pool);

            _lockSlim.ExitReadLock();

            return result;
        }

        private static bool Contains<TKey, TElement>(List<IKeyedPool> pools, IKeyedPool<TKey, TElement> pool)
            where TKey : notnull
            where TElement : class
        {
            Span<IntPtr> pointers = pools.GetItems().AsPointers();
            IntPtr pointer = Unsafe.As<IKeyedPool<TKey, TElement>, IntPtr>(ref pool);
            int index = pointers.IndexOf(pointer);

            return index > -1;
        }

        public static bool Any<TKey, TElement>()
            where TKey : notnull
            where TElement : class
        {
            Handle handle = GetHandle<TKey, TElement>();

            _lockSlim.EnterReadLock();

            bool result = _pools.Value.ContainsKey(handle);

            _lockSlim.ExitReadLock();

            return result;
        }

        public static bool Any(Predicate<Handle> match)
        {
            _lockSlim.EnterReadLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Handle, List<IKeyedPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Handle, List<IKeyedPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Handle? handle = entry.key;

                    if (match(handle!))
                    {
                        _lockSlim.ExitReadLock();

                        return true;
                    }
                }
            }

            _lockSlim.ExitReadLock();

            return false;
        }

        public static IKeyedPool<TKey, TElement> GetOrAdd<TKey, TElement>(Func<IKeyedPool<TKey, TElement>> onPoolMissing)
            where TKey : notnull
            where TElement : class
        {
            _lockSlim.EnterReadLock();

            if (UnsafeTryGet(out IKeyedPool<TKey, TElement>? pool))
            {
                _lockSlim.ExitReadLock();
            }
            else
            {
                _lockSlim.ExitReadLock();
                _lockSlim.EnterWriteLock();

                if (!UnsafeTryGet(out pool))
                {
                    Handle handle = GetHandle<TKey, TElement>();
                    List<IKeyedPool> pools = _pools.Value.GetOrAdd(handle);
                    pool = onPoolMissing();

                    pools.Add(pool);
                }

                _lockSlim.ExitWriteLock();
            }

            return pool!;
        }

        public static IKeyedPool<TKey, TElement> GetOrAdd<TKey, TElement>(TrySelector<TKey, TElement> poolSelector, Func<IKeyedPool<TKey, TElement>> onPoolMissing)
            where TKey : notnull
            where TElement : class
        {
            Handle handle = GetHandle<TKey, TElement>();
            IKeyedPool<TKey, TElement>? pool = default;

            _lockSlim.EnterReadLock();

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                bool selected = UnsafeTryGet(pools, poolSelector, out pool);

                _lockSlim.ExitReadLock();

                if (!selected)
                {
                    _lockSlim.EnterWriteLock();

                    selected = UnsafeTryGet(pools, poolSelector, out pool);

                    if (!selected)
                    {
                        pool = onPoolMissing();

                        pools.Add(pool);
                    }

                    _lockSlim.ExitWriteLock();
                }
            }
            else
            {
                _lockSlim.ExitReadLock();
                _lockSlim.EnterWriteLock();

                if (_pools.Value.TryGetValue(handle, out pools))
                {
                    bool selected = UnsafeTryGet(pools, poolSelector, out pool);

                    if (!selected)
                    {
                        pool = onPoolMissing();

                        pools.Add(pool);
                    }
                }
                else
                {
                    pools = new List<IKeyedPool>();
                    pool = onPoolMissing();

                    pools.Add(pool);
                    _pools.Value.Add(handle, pools);
                }

                _lockSlim.ExitWriteLock();
            }

            return pool!;
        }

        public static bool TryGet<TKey, TElement>(out IKeyedPool<TKey, TElement>? pool)
            where TKey : notnull
            where TElement : class
        {
            _lockSlim.EnterReadLock();

            bool result = UnsafeTryGet(out pool);

            _lockSlim.ExitReadLock();

            return result;
        }

        public static bool TryGet<TKey, TElement>(TrySelector<TKey, TElement> poolSelector, out IKeyedPool<TKey, TElement>? pool)
            where TKey : notnull
            where TElement : class
        {
            Handle handle = GetHandle<TKey, TElement>();

            pool = default;

            _lockSlim.EnterReadLock();

            bool selected = UnsafeTryGet(poolSelector, out pool);

            _lockSlim.ExitReadLock();

            return selected;
        }

        public static bool TryGet(Predicate<Handle> match, out IKeyedPool? pool)
        {
            pool = default;

            _lockSlim.EnterReadLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Handle, List<IKeyedPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Handle, List<IKeyedPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Handle handle = entry.key;

                    if (match(handle))
                    {
                        pool = entry.value.GetItems().DangerousGetReferenceAt(0);

                        _lockSlim.ExitReadLock();

                        return true;
                    }
                }
            }

            _lockSlim.ExitReadLock();

            return false;
        }

        public static bool TryGet(Predicate<Handle> match, Predicate<IKeyedPool> poolMatch, out IKeyedPool? pool)
        {
            pool = default;

            _lockSlim.EnterReadLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Handle, List<IKeyedPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Handle, List<IKeyedPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Handle handle = entry.key;

                    if (match(handle))
                    {
                        IKeyedPool[] pools = entry.value.GetItems();

                        for (int poolIndex = 0; poolIndex < entry.value.Count; poolIndex++)
                        {
                            IKeyedPool currentPool = pools.DangerousGetReferenceAt(poolIndex);

                            if (poolMatch(currentPool))
                            {
                                pool = currentPool;

                                _lockSlim.ExitReadLock();

                                return true;
                            }
                        }
                    }
                }
            }

            _lockSlim.ExitReadLock();

            return false;
        }

        public static bool UnsafeTryGet<TKey, TElement>(out IKeyedPool<TKey, TElement>? pool)
            where TKey : notnull
            where TElement : class
        {
            Handle handle = GetHandle<TKey, TElement>();

            pool = default;

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
            {
                pool = Unsafe.As<IKeyedPool<TKey, TElement>>(pools.GetItems().DangerousGetReferenceAt(0));

                return true;
            }

            return false;
        }

        public static bool UnsafeTryGet<TKey, TElement>(TrySelector<TKey, TElement> poolSelector, out IKeyedPool<TKey, TElement>? pool)
            where TKey : notnull
            where TElement : class
        {
            Handle handle = GetHandle<TKey, TElement>();
            bool selected = false;

            pool = default;

            if (_pools.Value.TryGetValue(handle, out List<IKeyedPool>? pools))
                selected = UnsafeTryGet(pools, poolSelector, out pool);

            return selected;
        }

        public static bool UnsafeTryGet<TKey, TElement>(List<IKeyedPool> pools, TrySelector<TKey, TElement> poolSelector, out IKeyedPool<TKey, TElement>? pool)
            where TKey : notnull
            where TElement : class
        {
            ref IKeyedPool<TKey, TElement> reference = ref Unsafe.As<IKeyedPool, IKeyedPool<TKey, TElement>>(ref pools.GetItems().DangerousGetReferenceAt(0));
            ReadOnlySpan<IKeyedPool<TKey, TElement>> span = MemoryMarshal.CreateSpan(ref reference, pools.Count);

            return poolSelector(span, out pool);
        }
    }
}
