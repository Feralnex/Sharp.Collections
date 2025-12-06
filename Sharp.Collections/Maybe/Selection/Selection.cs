using CommunityToolkit.HighPerformance;
using Sharp.Collections.Extensions;
using System;
using System.Collections.Generic;

namespace Sharp.Collections
{
    public abstract partial class Selection<TTarget> : Maybe<TTarget>
    {
        private Many _many;
        private Maybe<TTarget> _current;

        public override bool HasSome => _current.HasSome;
        public override int Count => _current.Count;

        public Selection()
        {
            _many = new Many();
            _current = None;
        }

        public Selection(TTarget target)
        {
            _many = new Many();

            bool isValid = Validate(target);

            if (isValid)
            {
                _many.Targets.Add(target);
                _current = _many;
            }
            else
                _current = None;
        }

        public Selection(TTarget[] targets)
        {
            _many = Initialize(targets, targets.Length);
            _current = _many.HasSome
                ? _many
                : None;
        }

        public Selection(List<TTarget> targets)
        {
            _many = Initialize(targets.GetItems(), targets.Count);
            _current = _many.HasSome
                ? _many
                : None;
        }

        public bool TryGet(out ReadOnlySpan<TTarget> targets)
        {
            targets = _many.GetAll();

            return HasSome;
        }

        public bool TryGetAt(int index, out TTarget? target)
            => _many.TryGetAt(index, out target);

        public ReadOnlySpan<TTarget> GetAll()
            => _many.GetAll();

        public void Clear()
        {
            if (HasSome)
            {
                _many.Targets.Clear();
                _current = None;
            }
        }

        public void Add(TTarget target)
        {
            bool hadSome = HasSome;
            bool isValid = Validate(target);

            if (isValid)
            {
                _many.Targets.Add(target);

                if (!hadSome)
                    _current = _many;
            }
        }

        public void Add(TTarget[] targets)
            => Add(targets, targets.Length);

        public void Add(List<TTarget> targets)
            => Add(targets.GetItems(), targets.Count);

        public void Remove(TTarget target)
            => _current.IfSome(OnSomeRemove, target);

        public void Remove(TTarget[] targets)
            => Remove(targets, targets.Length);

        public void Remove(List<TTarget> targets)
            => Remove(targets.GetItems(), targets.Count);

        public override bool IfNone(Action none)
            => _current.IfNone(none);

        public override bool IfNone<TInput>(Action<TInput> none, TInput input)
            => _current.IfNone(none, input);

        public override bool IfNone<TOutput>(Func<TOutput> none, out TOutput output)
            => _current.IfNone(none, out output);

        public override bool IfNone<TInput, TOutput>(Func<TInput, TOutput> none, TInput input, out TOutput output)
            => _current.IfNone(none, input, out output);

        public override bool IfSome(Action some)
            => _current.IfSome(some);

        public override bool IfSome<TInput>(Action<TInput> some, TInput input)
            => _current.IfSome(some, input);

        public override bool IfSome(Action<TTarget> some)
            => _current.IfSome(some);

        public override bool IfSome<TInput>(Action<TTarget, TInput> some, TInput input)
            => _current.IfSome(some, input);

        public override bool IfSome<TOutput>(Func<TOutput> some, out TOutput output)
            => _current.IfSome(some, out output);

        public override bool IfSome<TInput, TOutput>(Func<TInput, TOutput> some, TInput input, out TOutput output)
            => _current.IfSome(some, input, out output);

        public override void Match(Action some, Action none)
            => _current.Match(some, none);

        public override void Match<TInput>(Action<TInput> some, Action<TInput> none, TInput input)
            => _current.Match(some, none, input);

        public override void Match(Action<TTarget> some, Action none)
            => _current.Match(some, none);

        public override void Match<TInput>(Action<TTarget, TInput> some, Action<TInput> none, TInput input)
            => _current.Match(some, none, input);

        public override TOutput Match<TOutput>(Func<TOutput> some, Func<TOutput> none)
            => _current.Match(some, none);

        public override TOutput Match<TInput, TOutput>(Func<TInput, TOutput> some, Func<TInput, TOutput> none, TInput input)
            => _current.Match(some, none, input);

        protected abstract bool Validate(TTarget target);

        protected abstract bool TryGetFirstTargetIndex(TTarget[] source, int length, out int firstTargetIndex);

        protected abstract int CountTargets(TTarget[] source, int startIndex, int length);

        protected abstract void FillTargets(TTarget[] source, int startIndex, TTarget[] destination, int destinationIndex, int length);

        private Many Initialize(TTarget[] targets, int length)
        {
            int firstTargetIndex = 0;
            bool mightHaveSome = length > 0
                && TryGetFirstTargetIndex(targets, length, out firstTargetIndex);
            int count = mightHaveSome
                ? CountTargets(targets, firstTargetIndex + 1, length) + 1
                : 0;

            if (count > 1)
            {
                List<TTarget> nonEmptyTargets = new List<TTarget>(count);
                TTarget[] items = nonEmptyTargets.GetItems();
                int version = nonEmptyTargets.GetVersion();

                items.DangerousGetReferenceAt(0) = targets.DangerousGetReferenceAt(firstTargetIndex);
                FillTargets(targets, firstTargetIndex + 1, items, 1, length);

                nonEmptyTargets.SetCount(count);
                nonEmptyTargets.SetVersion(++version);

                return new Many(nonEmptyTargets);
            }
            else if (count == 1)
            {
                return new Many(targets.DangerousGetReferenceAt(firstTargetIndex));
            }
            else
            {
                return new Many();
            }
        }

        private void Add(TTarget[] targets, int length)
        {
            bool hadSome = HasSome;
            int firstTargetIndex = 0;
            bool mightHaveSome = length > 0
                && TryGetFirstTargetIndex(targets, length, out firstTargetIndex);
            int count = mightHaveSome
                ? CountTargets(targets, firstTargetIndex + 1, length) + 1
                : 0;

            if (count > 0)
            {
                int newCapacity = _many.Count + count;

                _many.Targets.EnsureCapacity(newCapacity);

                TTarget[] items = _many.Targets.GetItems();
                int version = _many.Targets.GetVersion();

                items.DangerousGetReferenceAt(_many.Targets.Count) = targets.DangerousGetReferenceAt(firstTargetIndex);
                FillTargets(targets, firstTargetIndex + 1, items, _many.Targets.Count + 1, length);

                _many.Targets.SetCount(newCapacity);
                _many.Targets.SetVersion(++version);

                if (!hadSome)
                    _current = _many;
            }
        }

        private void Remove(TTarget[] targets, int length)
            => _current.IfSome(OnSomeRemove, (Targets: targets, Length: length));

        private void OnSomeRemove(TTarget target)
        {
            bool isValid = Validate(target);

            if (isValid)
            {
                bool removed = _many.Targets.Remove(target);

                if (removed && _many.Count == 0)
                    _current = None;
            }
        }

        private void OnSomeRemove((TTarget[] Targets, int Length) input)
        {
            int firstTargetIndex = 0;
            bool mightHaveSome = input.Length > 0
                && TryGetFirstTargetIndex(input.Targets, input.Length, out firstTargetIndex);
            int count = mightHaveSome
                ? CountTargets(input.Targets, firstTargetIndex + 1, input.Length) + 1
                : 0;

            if (count > 0)
            {
                Span<TTarget> span = new Span<TTarget>(input.Targets, firstTargetIndex, input.Length - firstTargetIndex);

                for (int index = 0; index < span.Length; index++)
                    _many.Targets.Remove(span.DangerousGetReferenceAt(index));

                if (_many.Count == 0)
                    _current = None;
            }
        }
    }
}
