using CommunityToolkit.HighPerformance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sharp.Collections.Extensions
{
    public static partial class ListExtensions
    {
        public static TTarget[] GetItems<TTarget>(this List<TTarget> list)
            => Unsafe.As<UnsafeList<TTarget>>(list).Items;

        public static void SetItems<TTarget>(this List<TTarget> list, TTarget[] value)
            => Unsafe.As<UnsafeList<TTarget>>(list).Items = value;

        public static int GetCount<TTarget>(this List<TTarget> list)
            => Unsafe.As<UnsafeList<TTarget>>(list).Count;

        public static void SetCount<TTarget>(this List<TTarget> list, int value)
            => Unsafe.As<UnsafeList<TTarget>>(list).Count = value;

        public static int GetVersion<TTarget>(this List<TTarget> list)
            => Unsafe.As<UnsafeList<TTarget>>(list).Version;

        public static void SetVersion<TTarget>(this List<TTarget> list, int value)
            => Unsafe.As<UnsafeList<TTarget>>(list).Version = value;

        public static TCollection GetWithTheLeastCountOrAdd<TCollection>(this List<TCollection> list, int maxSize, Func<TCollection> onCollectionsFull)
            where TCollection : ICollection
        {
            int leastCountIndex = default;
            int leastCount = int.MaxValue;

            for (int index = leastCountIndex; index < list.Count; index++)
            {
                if (list.GetItems().DangerousGetReferenceAt(index).Count < leastCount)
                {
                    leastCountIndex = index;
                    leastCount = list[index].Count;
                }
            }

            if (leastCount >= maxSize)
            {
                TCollection collection = onCollectionsFull();

                list.Add(collection);

                leastCountIndex = list.Count - 1;
            }

            return list.GetItems().DangerousGetReferenceAt(leastCountIndex);
        }
    }
}
