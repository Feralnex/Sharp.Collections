using Sharp.Collections.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace Sharp.Collections
{
    public class References<TTarget> : Selection<TTarget>
        where TTarget : class
    {
        public References() : base() { }

        public References(TTarget target) : base(target) { }

        public References(TTarget[] targets) : base(targets) { }

        public static implicit operator References<TTarget>(TTarget target)
            => new References<TTarget>(target);

        public static implicit operator References<TTarget>(TTarget[] targets)
            => new References<TTarget>(targets);

        protected override bool Validate(TTarget target)
            => Unsafe.As<TTarget, IntPtr>(ref target) != IntPtr.Zero;

        protected override bool TryGetFirstTargetIndex(TTarget[] targets, int length, out int firstTargetIndex)
        {
            Span<IntPtr> span = new Span<IntPtr>(targets.AsPointers(), 0, length);

            for (firstTargetIndex = 0; firstTargetIndex < span.Length; firstTargetIndex++)
            {
                if (span[firstTargetIndex] != IntPtr.Zero)
                    return true;
            }

            return false;
        }

        protected override int CountTargets(TTarget[] targets, int startIndex, int length)
        {
            IntPtr[] pointers = targets.AsPointers();
            Span<IntPtr> pointersSpan = new Span<IntPtr>(pointers, startIndex, length - startIndex);
            int nullIndex = pointersSpan.IndexOf(IntPtr.Zero);
            int count = 0;

            while (nullIndex > -1)
            {
                int lastIndex = length - pointersSpan.Length + nullIndex + 1;

                pointersSpan = new Span<IntPtr>(pointers, lastIndex, length - lastIndex);
                nullIndex = pointersSpan.IndexOf(IntPtr.Zero);
                count++;
            }

            return length - startIndex - count;
        }

        protected override void FillTargets(TTarget[] source, int startIndex, TTarget[] destination, int destinationIndex, int length)
        {
            IntPtr[] pointers = source.AsPointers();
            Span<IntPtr> pointersSpan = new Span<IntPtr>(pointers, startIndex, length - startIndex);
            Span<TTarget> sourceSpan = new Span<TTarget>(source, startIndex, length - startIndex);
            Span<TTarget> destinationSpan = new Span<TTarget>(destination, destinationIndex, destination.Length - destinationIndex);
            int nullIndex = pointersSpan.IndexOf(IntPtr.Zero);
            int lastIndex = startIndex;

            destinationIndex = 0;

            while (nullIndex > -1)
            {
                lastIndex = length - pointersSpan.Length + nullIndex + 1;

                for (int index = 0; index < nullIndex; index++, destinationIndex++)
                    destinationSpan[destinationIndex] = sourceSpan[index];

                pointersSpan = new Span<IntPtr>(pointers, lastIndex, length - lastIndex);
                sourceSpan = new Span<TTarget>(source, lastIndex, length - lastIndex);
                nullIndex = pointersSpan.IndexOf(IntPtr.Zero);
            }

            sourceSpan = new Span<TTarget>(source, lastIndex, length - lastIndex);

            for (int index = 0; index < sourceSpan.Length; index++, destinationIndex++)
                destinationSpan[destinationIndex] = sourceSpan[index];
        }
    }
}
