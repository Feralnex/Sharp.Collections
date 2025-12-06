using Sharp.Extensions;
using System;
using System.Collections.Generic;

namespace Sharp.Collections.Extensions
{
    public static partial class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out TValue? value))
            {
                value = new TValue();
                dictionary.Add(key, value);
            }

            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> onMissingKey)
            where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out TValue? value))
            {
                value = onMissingKey();
                dictionary.Add(key, value);
            }

            return value;
        }

        public static int[]? GetBuckets<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetBuckets, X86<TKey, TValue>.GetBuckets, dictionary);

        public static void SetBuckets<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int[]? value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetBuckets, X86<TKey, TValue>.SetBuckets, (dictionary, value));

        public static Entry<TKey, TValue>[]? GetEntries<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetEntries, X86<TKey, TValue>.GetEntries, dictionary);

        public static void SetEntries<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Entry<TKey, TValue>[]? value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetEntries, X86<TKey, TValue>.SetEntries, (dictionary, value));

        public static bool TryGetFastModMultiplier<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, out ulong fastModMultiplier)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.TryMatch(X64<TKey, TValue>.TryGetFastModMultiplier, X86<TKey, TValue>.TryGetFastModMultiplier, dictionary, out fastModMultiplier);

        public static bool TrySetFastModMultiplier<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, ulong value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.TrySetFastModMultiplier, X86<TKey, TValue>.TrySetFastModMultiplier, (dictionary, value));

        public static int GetCount<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetCount, X86<TKey, TValue>.GetCount, dictionary);

        public static void SetCount<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetCount, X86<TKey, TValue>.SetCount, (dictionary, value));

        public static int GetFreeCount<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetFreeCount, X86<TKey, TValue>.GetFreeCount, dictionary);

        public static void SetFreeCount<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetFreeCount, X86<TKey, TValue>.SetFreeCount, (dictionary, value));

        public static int GetFreeList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetFreeList, X86<TKey, TValue>.GetFreeList, dictionary);

        public static void SetFreeList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetFreeList, X86<TKey, TValue>.SetFreeList, (dictionary, value));

        public static int GetVersion<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetVersion, X86<TKey, TValue>.GetVersion, dictionary);

        public static void SetVersion<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, int value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetVersion, X86<TKey, TValue>.SetVersion, (dictionary, value));

        public static IEqualityComparer<TKey>? GetComparer<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetComparer, X86<TKey, TValue>.GetComparer, dictionary);

        public static void SetComparer<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetComparer, X86<TKey, TValue>.SetComparer, (dictionary, value));

        public static Dictionary<TKey, TValue>.KeyCollection? GetKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetKeys, X86<TKey, TValue>.GetKeys, dictionary);

        public static void SetKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue>.KeyCollection? value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetKeys, X86<TKey, TValue>.SetKeys, (dictionary, value));

        public static Dictionary<TKey, TValue>.ValueCollection? GetValues<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.GetValues, X86<TKey, TValue>.GetValues, dictionary);

        public static void SetValues<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue>.ValueCollection? value)
            where TKey : notnull
            => ArchitectureExtensions.IsProcess64Bit.Match(X64<TKey, TValue>.SetValues, X86<TKey, TValue>.SetValues, (dictionary, value));
    }
}
