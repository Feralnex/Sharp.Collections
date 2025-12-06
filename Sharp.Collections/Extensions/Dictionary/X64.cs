using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sharp.Collections.Extensions
{
    public static partial class DictionaryExtensions
    {
        private class X64<TKey, TValue>
            where TKey : notnull
        {
            private int[]? _buckets;
            private Entry<TKey, TValue>[]? _entries;
            private ulong _fastModMultiplier;
            private int _count;
            private int _freeCount;
            private int _freeList;
            private int _version;
            private IEqualityComparer<TKey>? _comparer;
            private Dictionary<TKey, TValue>.KeyCollection? _keys;
            private Dictionary<TKey, TValue>.ValueCollection? _values;

            public int[]? Buckets
            {
                get => _buckets;
                set => _buckets = value;
            }
            public Entry<TKey, TValue>[]? Entries
            {
                get => _entries;
                set => _entries = value;
            }
            public int Count
            {
                get => _count;
                set => _count = value;
            }
            public int FreeCount
            {
                get => _freeCount;
                set => _freeCount = value;
            }
            public int FreeList
            {
                get => _freeList;
                set => _freeList = value;
            }
            public int Version
            {
                get => _version;
                set => _version = value;
            }
            public IEqualityComparer<TKey>? Comparer
            {
                get => _comparer;
                set => _comparer = value;
            }
            public Dictionary<TKey, TValue>.KeyCollection? Keys
            {
                get => _keys;
                set => _keys = value;
            }
            public Dictionary<TKey, TValue>.ValueCollection? Values
            {
                get => _values;
                set => _values = value;
            }

            public bool TryGetFastModMultiplier(out ulong fastModMultiplier)
            {
                fastModMultiplier = _fastModMultiplier;

                return true;
            }

            public bool TrySetFastModMultiplier(ulong value)
            {
                _fastModMultiplier = value;

                return true;
            }

            public static int[]? GetBuckets(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Buckets;

            public static void SetBuckets((Dictionary<TKey, TValue> Dictionary, int[]? Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Buckets = input.Value;

            public static Entry<TKey, TValue>[]? GetEntries(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Entries;

            public static void SetEntries((Dictionary<TKey, TValue> Dictionary, Entry<TKey, TValue>[]? Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Entries = input.Value;

            public static bool TryGetFastModMultiplier(Dictionary<TKey, TValue> dictionary, out ulong fastModMultiplier) => Unsafe.As<X64<TKey, TValue>>(dictionary).TryGetFastModMultiplier(out fastModMultiplier);

            public static bool TrySetFastModMultiplier((Dictionary<TKey, TValue> Dictionary, ulong Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).TrySetFastModMultiplier(input.Value);

            public static int GetCount(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Count;

            public static void SetCount((Dictionary<TKey, TValue> Dictionary, int Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Count = input.Value;

            public static int GetFreeCount(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).FreeCount;

            public static void SetFreeCount((Dictionary<TKey, TValue> Dictionary, int Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).FreeCount = input.Value;

            public static int GetFreeList(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).FreeList;

            public static void SetFreeList((Dictionary<TKey, TValue> Dictionary, int Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).FreeList = input.Value;

            public static int GetVersion(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Version;

            public static void SetVersion((Dictionary<TKey, TValue> Dictionary, int Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Version = input.Value;

            public static IEqualityComparer<TKey>? GetComparer(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Comparer;

            public static void SetComparer((Dictionary<TKey, TValue> Dictionary, IEqualityComparer<TKey>? Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Comparer = input.Value;

            public static Dictionary<TKey, TValue>.KeyCollection? GetKeys(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Keys;

            public static void SetKeys((Dictionary<TKey, TValue> Dictionary, Dictionary<TKey, TValue>.KeyCollection? Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Keys = input.Value;

            public static Dictionary<TKey, TValue>.ValueCollection? GetValues(Dictionary<TKey, TValue> dictionary) => Unsafe.As<X64<TKey, TValue>>(dictionary).Values;

            public static void SetValues((Dictionary<TKey, TValue> Dictionary, Dictionary<TKey, TValue>.ValueCollection? Value) input) => Unsafe.As<X64<TKey, TValue>>(input.Dictionary).Values = input.Value;
        }
    }
}
