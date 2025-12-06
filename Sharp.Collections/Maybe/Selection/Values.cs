using System;

namespace Sharp.Collections
{
    public class Values<TTarget> : Selection<TTarget>
        where TTarget : struct
    {
        public Values() : base() { }

        public Values(TTarget target) : base(target) { }

        public Values(TTarget[] targets) : base(targets) { }

        public static implicit operator Values<TTarget>(TTarget target)
            => new Values<TTarget>(target);

        public static implicit operator Values<TTarget>(TTarget[] targets)
            => new Values<TTarget>(targets);

        protected override bool Validate(TTarget target)
            => true;

        protected override bool TryGetFirstTargetIndex(TTarget[] targets, int length, out int firstTargetIndex)
        {
            firstTargetIndex = 0;

            return true;
        }

        protected override int CountTargets(TTarget[] targets, int startIndex, int length)
            => length - startIndex;

        protected override void FillTargets(TTarget[] source, int startIndex, TTarget[] destination, int destinationIndex, int length)
        {
            Span<TTarget> sourceSpan = new Span<TTarget>(source, startIndex, length - startIndex);
            Span<TTarget> destinationSpan = new Span<TTarget>(destination, destinationIndex, destination.Length - destinationIndex);

            for (int index = 0; index < sourceSpan.Length; index++)
                destinationSpan[index] = sourceSpan[index];
        }
    }
}
