using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sharp.Collections
{
    public class NativeList<TElement>
        where TElement : unmanaged
    {
        protected static ConcurrentDictionary<IntPtr, NativeList<TElement>> ReverseCache { get; }

        private IntPtr _pointer;

        public nuint Size { get; }

        public unsafe NativeList(nuint size)
        {
            _pointer = (IntPtr)NativeMemory.AllocZeroed(size);

            Size = size;

            ReverseCache.TryAdd(_pointer, this);
        }

        static NativeList()
            => ReverseCache = new ConcurrentDictionary<nint, NativeList<TElement>>();

        unsafe ~NativeList()
        {
            NativeMemory.Free((void*)_pointer);

            ReverseCache.Remove(_pointer, out _);
        }

        public static implicit operator NativeList<TElement>(IntPtr pointer)
        {
            if (!ReverseCache.TryGetValue(pointer, out NativeList<TElement>? nativeList))
                throw new InvalidCastException();

            return nativeList;
        }

        public static implicit operator IntPtr(NativeList<TElement> nativeList)
            => nativeList._pointer;
    }
}
