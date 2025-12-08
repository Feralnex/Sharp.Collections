using Sharp.Extensions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sharp.Collections.Extensions
{
    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int IndexOfAnyNumberExcept<TValue>(this Span<TValue> span, TValue value)
            where TValue : struct, INumber<TValue>
        {
            ref TValue searchSpace = ref MemoryMarshal.GetReference(span);
            int length = span.Length;

            return searchSpace.DangerousIndexOfAnyNumberExcept(value, 0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int IndexOfAnyNumberExcept<TValue>(this Span<TValue> span, TValue value, int offset)
            where TValue : struct, INumber<TValue>
        {
            ref TValue searchSpace = ref MemoryMarshal.GetReference(span);
            int index = offset - 1;
            int length = span.Length - offset;

            return searchSpace.DangerousIndexOfAnyNumberExcept(value, index, length, offset);
        }

        public static unsafe void CopyTo(this ReadOnlySpan<char> source, sbyte* destination)
        {
            for (int index = 0; index < source.Length; index++)
            {
                char character = source[index];
                destination[index] = (sbyte)(character <= 0x7F ? character : 0x3F);
            }

            destination[source.Length] = 0;
        }
    }
}
