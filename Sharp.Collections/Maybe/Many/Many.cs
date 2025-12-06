using Sharp.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using Sharp.Collections.Extensions;

namespace Sharp.Collections
{
    public abstract partial class Selection<TTarget>
    {
        protected partial class Many : Maybe<TTarget>
        {
            private static IntPtr _typePointer;
            private static int _typeSize;

            private IterableList _targets;

            public List<TTarget> Targets => _targets.Source;
            public override bool HasSome => true;
            public override int Count => Targets.Count;

            public Many()
                => _targets = new IterableList();

            public Many(TTarget target)
                => _targets = new IterableList(1)
                {
                    target
                };

            public Many(List<TTarget> targets)
                => _targets = new IterableList(targets);

            static Many()
            {
                _typePointer = typeof(Iterator).TypeHandle.Value;
                _typeSize = Marshal.ReadInt32(_typePointer, sizeof(int));
            }

            public bool TryGetAt(int index, out TTarget? target)
            {
                bool hasTarget = Count > index;

                if (hasTarget)
                    target = Targets.GetItems().DangerousGetReferenceAt(index);
                else
                    target = default;

                return hasTarget;
            }

            public ReadOnlySpan<TTarget> GetAll()
                => _targets.AsReadOnlySpan();

            public void Clear()
                => _targets.Clear();

            public override bool IfNone(Action none)
                => false;

            public override bool IfNone<TInput>(Action<TInput> none, TInput input)
                => false;

            public override bool IfNone<TOutput>(Func<TOutput> none, out TOutput output)
            {
                output = default!;

                return false;
            }

            public override bool IfNone<TInput, TOutput>(Func<TInput, TOutput> none, TInput input, out TOutput output)
            {
                output = default!;

                return false;
            }

            public override bool IfSome(Action some)
            {
                some();

                return true;
            }

            public override bool IfSome<TInput>(Action<TInput> some, TInput input)
            {
                some(input);

                return true;
            }

            public unsafe override bool IfSome(Action<TTarget> some)
            {
                if (Targets.Count == 1)
                    some(Targets.GetItems().DangerousGetReferenceAt(0));
                else
                {
                    #region Allocate iterator on stack

                    int size = _typeSize;
                    byte* allocatedObjectMemory = stackalloc byte[size];
                    IntPtr handle = new IntPtr(allocatedObjectMemory);
                    IntPtr pointer = handle + IntPtr.Size;

                    *(IntPtr*)pointer = _typePointer;

                    Iterator iterator = pointer.Cast<Iterator>();
                    iterator.Length = Targets.Count;

                    #endregion Allocate iterator on stack

                    iterator.Subscribe(_targets);

                    for (iterator.Index = 0; iterator.Index < Targets.Count; iterator.Index++)
                        some(Targets.GetItems().DangerousGetReferenceAt(iterator.Index));

                    iterator.Unsubscribe(_targets);
                }

                return true;
            }

            public unsafe override bool IfSome<TInput>(Action<TTarget, TInput> some, TInput input)
            {
                if (Targets.Count == 1)
                    some(Targets.GetItems().DangerousGetReferenceAt(0), input);
                else
                {
                    #region Allocate iterator on stack

                    int size = _typeSize;
                    byte* allocatedObjectMemory = stackalloc byte[size];
                    IntPtr handle = new IntPtr(allocatedObjectMemory);
                    IntPtr pointer = handle + IntPtr.Size;

                    *(IntPtr*)pointer = _typePointer;

                    Iterator iterator = pointer.Cast<Iterator>();
                    iterator.Length = Targets.Count;

                    #endregion Allocate iterator on stack

                    iterator.Subscribe(_targets);

                    for (iterator.Index = 0; iterator.Index < Targets.Count; iterator.Index++)
                        some(Targets.GetItems().DangerousGetReferenceAt(iterator.Index), input);

                    iterator.Unsubscribe(_targets);
                }

                return true;
            }

            public override bool IfSome<TOutput>(Func<TOutput> some, out TOutput output)
            {
                output = some();

                return true;
            }

            public override bool IfSome<TInput, TOutput>(Func<TInput, TOutput> some, TInput input, out TOutput output)
            {
                output = some(input);

                return true;
            }

            public unsafe override void Match(Action some, Action none)
            {
                if (Targets.Count == 1)
                    some();
                else
                {
                    for (int index = 0; index < Targets.Count; index++)
                        some();
                }
            }

            public unsafe override void Match<TInput>(Action<TInput> some, Action<TInput> none, TInput input)
            {
                if (Targets.Count == 1)
                    some(input);
                else
                {
                    for (int index = 0; index < Targets.Count; index++)
                        some(input);
                }
            }

            public unsafe override void Match(Action<TTarget> some, Action none)
            {
                if (Targets.Count == 1)
                    some(Targets.GetItems().DangerousGetReferenceAt(0));
                else
                {
                    #region Allocate iterator on stack

                    int size = _typeSize;
                    byte* allocatedObjectMemory = stackalloc byte[size];
                    IntPtr handle = new IntPtr(allocatedObjectMemory);
                    IntPtr pointer = handle + IntPtr.Size;

                    *(IntPtr*)pointer = _typePointer;

                    Iterator iterator = pointer.Cast<Iterator>();
                    iterator.Length = Targets.Count;

                    #endregion Allocate iterator on stack

                    iterator.Subscribe(_targets);

                    for (iterator.Index = 0; iterator.Index < Targets.Count; iterator.Index++)
                        some(Targets.GetItems().DangerousGetReferenceAt(iterator.Index));

                    iterator.Unsubscribe(_targets);
                }
            }

            public unsafe override void Match<TInput>(Action<TTarget, TInput> some, Action<TInput> none, TInput input)
            {
                if (Targets.Count == 1)
                    some(Targets.GetItems().DangerousGetReferenceAt(0), input);
                else
                {
                    #region Allocate iterator on stack

                    int size = _typeSize;
                    byte* allocatedObjectMemory = stackalloc byte[size];
                    IntPtr handle = new IntPtr(allocatedObjectMemory);
                    IntPtr pointer = handle + IntPtr.Size;

                    *(IntPtr*)pointer = _typePointer;

                    Iterator iterator = pointer.Cast<Iterator>();
                    iterator.Length = Targets.Count;

                    #endregion Allocate iterator on stack

                    iterator.Subscribe(_targets);

                    for (iterator.Index = 0; iterator.Index < Targets.Count; iterator.Index++)
                        some(Targets.GetItems().DangerousGetReferenceAt(iterator.Index), input);

                    iterator.Unsubscribe(_targets);
                }
            }

            public override TOutput Match<TOutput>(Func<TOutput> some, Func<TOutput> none)
                => some();

            public override TOutput Match<TInput, TOutput>(Func<TInput, TOutput> some, Func<TInput, TOutput> none, TInput input)
                => some(input);
        }
    }
}
