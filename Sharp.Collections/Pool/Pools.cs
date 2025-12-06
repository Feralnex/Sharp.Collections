using CommunityToolkit.HighPerformance;
using Sharp.Collections.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sharp.Collections
{
    public static class Pools
    {
        public delegate bool TrySelector<TElement>(ReadOnlySpan<IPool<TElement>> span, out IPool<TElement>? pool) where TElement : class;

        private static Lazy<Dictionary<Type, List<IPool>>> _pools;
        private static ReaderWriterLockSlim _lockSlim;

        static Pools()
        {
            _pools = new Lazy<Dictionary<Type, List<IPool>>>(Create);
            _lockSlim = new ReaderWriterLockSlim();
        }

        private static Dictionary<Type, List<IPool>> Create()
            => new Dictionary<Type, List<IPool>>();

        public static void Add<TElement>(IPool<TElement> pool)
            where TElement : class
        {
            Type handle = typeof(TElement);

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
            {
                if (!Contains(pools, pool))
                    pools.Add(pool);
            }
            else
            {
                pools = new List<IPool>() { pool };

                _pools.Value.Add(handle, pools);
            }

            _lockSlim.ExitWriteLock();
        }

        public static bool TryAdd<TElement>(IPool<TElement> pool)
            where TElement : class
        {
            bool added = false;
            Type handle = typeof(TElement);

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
            {
                if (!Contains(pools, pool))
                {
                    pools.Add(pool);

                    added = true;
                }
            }
            else
            {
                pools = new List<IPool>() { pool };

                _pools.Value.Add(handle, pools);

                added = true;
            }

            _lockSlim.ExitWriteLock();

            return added;
        }

        public static bool Remove<TElement>(IPool<TElement> pool)
            where TElement : class
        {
            bool removed = false;
            Type handle = typeof(TElement);

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
            {
                removed = pools.Remove(pool);

                if (pools.Count == 0)
                    _pools.Value.Remove(handle);
            }

            _lockSlim.ExitWriteLock();

            return removed;
        }

        public static int RemoveAll<TElement>()
            where TElement : class
        {
            int removedCount = 0;
            Type handle = typeof(TElement);

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
            {
                removedCount = pools.Count;

                pools.Clear();
                _pools.Value.Remove(handle);
            }

            _lockSlim.ExitWriteLock();

            return removedCount;
        }

        public static int RemoveAll<TElement>(Predicate<IPool<TElement>> match)
            where TElement : class
        {
            int removedCount = 0;
            Type handle = typeof(TElement);

            _lockSlim.EnterWriteLock();

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
            {
                ref IPool<TElement> reference = ref Unsafe.As<IPool, IPool<TElement>>(ref pools.GetItems().DangerousGetReferenceAt(0));
                ReadOnlySpan<IPool<TElement>> span = MemoryMarshal.CreateSpan(ref reference, pools.Count);

                for (int index = 0; index < pools.Count; index++)
                {
                    IPool<TElement> pool = span.DangerousGetReferenceAt(index);

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

        public static int RemoveAll(Predicate<Type> typeMatch, Predicate<IPool> poolMatch)
        {
            int removedCount = 0;

            _lockSlim.EnterWriteLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Type, List<IPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Type, List<IPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Type? handle = entry.key;

                    if (typeMatch(handle!))
                    {
                        List<IPool> pools = entry.value;

                        removedCount += pools.RemoveAll(poolMatch);

                        if (pools.Count == 0)
                            _pools.Value.Remove(entry.key);
                    }
                }
            }

            _lockSlim.ExitWriteLock();

            return removedCount;
        }

        public static bool Contains<TElement>(IPool<TElement> pool)
            where TElement : class
        {
            Type handle = typeof(TElement);

            _lockSlim.EnterReadLock();

            bool containsPool = _pools.Value.TryGetValue(handle, out List<IPool>? pools)
                && Contains(pools, pool);

            _lockSlim.ExitReadLock();

            return containsPool;
        }

        private static bool Contains<TElement>(List<IPool> pools, IPool<TElement> pool)
            where TElement : class
        {
            Span<nint> pointers = pools.GetItems().AsPointers();
            nint pointer = Unsafe.As<IPool<TElement>, nint>(ref pool);
            int index = pointers.IndexOf(pointer);

            return index > -1;
        }

        public static bool Any<TElement>()
            where TElement : class
        {
            Type handle = typeof(TElement);

            _lockSlim.EnterReadLock();

            bool containsKey = _pools.Value.ContainsKey(handle);

            _lockSlim.ExitReadLock();

            return containsKey;
        }

        public static bool Any(Predicate<Type> match)
        {
            _lockSlim.EnterReadLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Type, List<IPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Type, List<IPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Type? handle = entry.key;

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

        public static IPool<TElement> GetOrAdd<TElement>(Func<IPool<TElement>> onPoolMissing)
            where TElement : class
        {
            _lockSlim.EnterReadLock();

            if (UnsafeTryGet(out IPool<TElement>? pool))
            {
                _lockSlim.ExitReadLock();
            }
            else
            {
                _lockSlim.ExitReadLock();
                _lockSlim.EnterWriteLock();

                if (!UnsafeTryGet(out pool))
                {
                    Type handle = typeof(TElement);
                    List<IPool> pools = _pools.Value.GetOrAdd(handle);
                    pool = onPoolMissing();

                    pools.Add(pool);
                }

                _lockSlim.ExitWriteLock();
            }

            return pool!;
        }

        public static IPool<TElement> GetOrAdd<TElement>(TrySelector<TElement> poolSelector, Func<IPool<TElement>> onPoolMissing)
            where TElement : class
        {
            Type handle = typeof(TElement);
            IPool<TElement>? pool = default;

            _lockSlim.EnterReadLock();

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
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
                    pools = new List<IPool>();
                    pool = onPoolMissing();

                    pools.Add(pool);
                    _pools.Value.Add(handle, pools);
                }

                _lockSlim.ExitWriteLock();
            }

            return pool!;
        }

        public static bool TryGet<TElement>(out IPool<TElement>? pool)
            where TElement : class
        {
            _lockSlim.EnterReadLock();

            bool result = UnsafeTryGet(out pool);

            _lockSlim.ExitReadLock();

            return result;
        }

        public static bool TryGet<TElement>(TrySelector<TElement> poolSelector, out IPool<TElement>? pool)
            where TElement : class
        {
            Type handle = typeof(TElement);

            pool = default;

            _lockSlim.EnterReadLock();

            bool selected = UnsafeTryGet(poolSelector, out pool);

            _lockSlim.ExitReadLock();

            return selected;
        }

        public static bool TryGet(Predicate<Type> match, out IPool? pool)
        {
            pool = default;

            _lockSlim.EnterReadLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Type, List<IPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Type, List<IPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Type handle = entry.key;

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

        public static bool TryGet(Predicate<Type> match, Predicate<IPool> poolMatch, out IPool? pool)
        {
            pool = default;

            _lockSlim.EnterReadLock();

            int count = _pools.Value.GetCount();
            DictionaryExtensions.Entry<Type, List<IPool>>[]? entries = _pools.Value.GetEntries();

            for (int index = 0; index < count; index++)
            {
                ref DictionaryExtensions.Entry<Type, List<IPool>> entry = ref entries!.DangerousGetReferenceAt(index);

                if (entry.next >= -1)
                {
                    Type handle = entry.key;

                    if (match(handle))
                    {
                        IPool[] pools = entry.value.GetItems();

                        for (int poolIndex = 0; poolIndex < entry.value.Count; poolIndex++)
                        {
                            IPool currentPool = pools.DangerousGetReferenceAt(poolIndex);

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

        public static bool UnsafeTryGet<TElement>(out IPool<TElement>? pool)
            where TElement : class
        {
            Type handle = typeof(TElement);

            pool = default;

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
            {
                pool = Unsafe.As<IPool<TElement>>(pools.GetItems().DangerousGetReferenceAt(0));

                return true;
            }

            return false;
        }

        public static bool UnsafeTryGet<TElement>(TrySelector<TElement> poolSelector, out IPool<TElement>? pool)
            where TElement : class
        {
            Type handle = typeof(TElement);
            bool selected = false;

            pool = default;

            if (_pools.Value.TryGetValue(handle, out List<IPool>? pools))
                selected = UnsafeTryGet(pools, poolSelector, out pool);

            return selected;
        }

        public static bool UnsafeTryGet<TElement>(List<IPool> pools, TrySelector<TElement> poolSelector, out IPool<TElement>? pool)
            where TElement : class
        {
            Type handle = typeof(TElement);

            ref IPool<TElement> reference = ref Unsafe.As<IPool, IPool<TElement>>(ref pools.GetItems().DangerousGetReferenceAt(0));
            ReadOnlySpan<IPool<TElement>> span = MemoryMarshal.CreateSpan(ref reference, pools.Count);

            return poolSelector(span, out pool);
        }
    }
}
