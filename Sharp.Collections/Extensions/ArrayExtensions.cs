using CommunityToolkit.HighPerformance;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Sharp.Collections.Extensions
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr[] AsPointers<TType>(this TType[] array) where TType : class
            => Unsafe.As<TType[], IntPtr[]>(ref array);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool TrySwap(this byte[] source, int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            // Check if the indices and length are within bounds for both arrays
            if (sourceIndex < 0 || destinationIndex < 0 || length < 0 ||
                sourceIndex + length > source.Length || destinationIndex + length > destination.Length)
            {
                return false; // Invalid parameters, out of bounds
            }

            source.DangerousSwap(sourceIndex, destination, destinationIndex, length);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void DangerousSwap(this byte[] source, int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            if (Vector512.IsHardwareAccelerated && length >= Vector512<byte>.Count)
            {
                do
                {
                    ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                    ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                    Vector512<byte> sourceValue = Unsafe.ReadUnaligned<Vector512<byte>>(ref sourceReference);
                    Vector512<byte> destinationValue = Unsafe.ReadUnaligned<Vector512<byte>>(ref destinationReference);
                    Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                    Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                    sourceIndex += Vector512<byte>.Count;
                    destinationIndex += Vector512<byte>.Count;
                    length -= Vector512<byte>.Count;
                }
                while (length >= Vector512<byte>.Count);
            }
            if (Vector256.IsHardwareAccelerated && length >= Vector256<byte>.Count)
            {
                if (Vector512.IsHardwareAccelerated)
                {
                    ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                    ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                    Vector256<byte> sourceValue = Unsafe.ReadUnaligned<Vector256<byte>>(ref sourceReference);
                    Vector256<byte> destinationValue = Unsafe.ReadUnaligned<Vector256<byte>>(ref destinationReference);
                    Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                    Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                    sourceIndex += Vector256<byte>.Count;
                    destinationIndex += Vector256<byte>.Count;
                    length -= Vector256<byte>.Count;
                }
                else
                {
                    do
                    {
                        ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                        ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                        Vector256<byte> sourceValue = Unsafe.ReadUnaligned<Vector256<byte>>(ref sourceReference);
                        Vector256<byte> destinationValue = Unsafe.ReadUnaligned<Vector256<byte>>(ref destinationReference);
                        Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                        Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                        sourceIndex += Vector256<byte>.Count;
                        destinationIndex += Vector256<byte>.Count;
                        length -= Vector256<byte>.Count;
                    }
                    while (length >= Vector256<byte>.Count);
                }
            }
            if (Vector128.IsHardwareAccelerated && length >= Vector128<byte>.Count)
            {
                if (Vector256.IsHardwareAccelerated)
                {
                    ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                    ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                    Vector128<byte> sourceValue = Unsafe.ReadUnaligned<Vector128<byte>>(ref sourceReference);
                    Vector128<byte> destinationValue = Unsafe.ReadUnaligned<Vector128<byte>>(ref destinationReference);
                    Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                    Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                    sourceIndex += Vector128<byte>.Count;
                    destinationIndex += Vector128<byte>.Count;
                    length -= Vector128<byte>.Count;
                }
                else
                {
                    do
                    {
                        ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                        ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                        Vector128<byte> sourceValue = Unsafe.ReadUnaligned<Vector128<byte>>(ref sourceReference);
                        Vector128<byte> destinationValue = Unsafe.ReadUnaligned<Vector128<byte>>(ref destinationReference);
                        Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                        Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                        sourceIndex += Vector128<byte>.Count;
                        destinationIndex += Vector128<byte>.Count;
                        length -= Vector128<byte>.Count;
                    }
                    while (length >= Vector128<byte>.Count);
                }
            }
            if (length >= sizeof(ulong))
            {
                if (Vector128.IsHardwareAccelerated)
                {
                    ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                    ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                    ulong sourceValue = Unsafe.ReadUnaligned<ulong>(ref sourceReference);
                    ulong destinationValue = Unsafe.ReadUnaligned<ulong>(ref destinationReference);
                    Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                    Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                    sourceIndex += sizeof(ulong);
                    destinationIndex += sizeof(ulong);
                    length -= sizeof(ulong);
                }
                else
                {
                    do
                    {
                        ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                        ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                        ulong sourceValue = Unsafe.ReadUnaligned<ulong>(ref sourceReference);
                        ulong destinationValue = Unsafe.ReadUnaligned<ulong>(ref destinationReference);
                        Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                        Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                        sourceIndex += sizeof(ulong);
                        destinationIndex += sizeof(ulong);
                        length -= sizeof(ulong);
                    }
                    while (length >= sizeof(ulong));
                }
            }
            if (length >= sizeof(uint))
            {
                ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                uint sourceValue = Unsafe.ReadUnaligned<uint>(ref sourceReference);
                uint destinationValue = Unsafe.ReadUnaligned<uint>(ref destinationReference);
                Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                sourceIndex += sizeof(uint);
                destinationIndex += sizeof(uint);
                length -= sizeof(uint);
            }
            if (length >= sizeof(ushort))
            {
                ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                ushort sourceValue = Unsafe.ReadUnaligned<ushort>(ref sourceReference);
                ushort destinationValue = Unsafe.ReadUnaligned<ushort>(ref destinationReference);
                Unsafe.WriteUnaligned(ref sourceReference, destinationValue);
                Unsafe.WriteUnaligned(ref destinationReference, sourceValue);

                sourceIndex += sizeof(ushort);
                destinationIndex += sizeof(ushort);
                length -= sizeof(ushort);
            }
            if (length > 0)
            {
                ref byte sourceReference = ref source.DangerousGetReferenceAt(sourceIndex);
                ref byte destinationReference = ref destination.DangerousGetReferenceAt(destinationIndex);

                byte sourceValue = sourceReference;
                Unsafe.WriteUnaligned(ref sourceReference, destinationReference);
                Unsafe.WriteUnaligned(ref destinationReference, sourceValue);
            }
        }
    }
}
