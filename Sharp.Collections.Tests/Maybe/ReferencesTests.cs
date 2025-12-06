using System;
using System.Linq;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class ReferencesTests
    {
        private Random _random;

        public ReferencesTests()
            => _random = new Random();

        [Fact]
        public void Initialization_WhenUsedDefaultConstructor_ShouldSetHasSomeToFalse()
        {
            // Arrange and Act
            References<string> references = new References<string>();

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void Initialization_WhenProvidedObjectReference_ShouldSetHasSomeToTrue()
        {
            // Arrange
            string target = nameof(target);

            // Act
            References<string> references = new References<string>(target);

            // Assert
            Assert.True(references.HasSome);
        }

        [Fact]
        public void Initialization_WhenProvidedObjectReferences_ShouldSetHasSomeToTrue()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];

            // Act
            References<string> references = new References<string>(targets);

            // Assert
            Assert.True(references.HasSome);
        }

        [Fact]
        public void Initialization_WhenProvidedNullReference_ShouldSetHasSomeToFalse()
        {
            // Arrange
            string target = null!;

            // Act
            References<string> references = new References<string>(target);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void TryGet_WhenReferencesDoesNotHaveSome_ShouldReturnFalseAndAssignDefaultOutput()
        {
            // Arrange
            References<string> references = new References<string>();

            // Act
            bool success = references.TryGet(out ReadOnlySpan<string> output);

            // Assert
            Assert.False(success);
            Assert.True(output.IsEmpty);
        }

        [Fact]
        public void TryGet_WhenReferencesHasSome_ShouldReturnTrueAndAssignOutput()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);

            // Act
            bool success = references.TryGet(out ReadOnlySpan<string> output);

            // Assert
            Assert.True(success);

            for (int index = 0; index < targets.Length; index++)
                Assert.Equal(targets[index], output[index]);
        }

        [Fact]
        public void TryGetAtInvokedWithAnyIndex_WhenReferencesDoesNotHaveSome_ShouldReturnFalseAndAssignDefaultOutput()
        {
            // Arrange
            References<string> references = new References<string>();
            int index = _random.Next(0, int.MaxValue);

            // Act
            bool success = references.TryGetAt(index, out string? output);

            // Assert
            Assert.False(success);
            Assert.Equal(default, output);
        }

        [Fact]
        public void TryGetAtInvokedWithIndexInRangeOfReferences_WhenReferencesHasSome_ShouldReturnTrueAndAssignOutput()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);
            int index = _random.Next(0, targets.Length);

            // Act
            bool success = references.TryGetAt(index, out string? output);

            // Assert
            Assert.True(success);
            Assert.Equal(targets[index], output);
        }

        [Fact]
        public void TryGetAtInvokedWithIndexExceedingRangeOfReferences_WhenReferenceHasSome_ShouldReturnFalseAndAssignDefaultOutput()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);
            int index = _random.Next(targets.Length, int.MaxValue);

            // Act
            bool success = references.TryGetAt(index, out string? output);

            // Assert
            Assert.False(success);
            Assert.Equal(default, output);
        }

        [Fact]
        public void GetAll_WhenReferencesDoesNotHaveSome_ShouldReturnEmptySpan()
        {
            // Arrange
            References<string> references = new References<string>();

            // Act
            ReadOnlySpan<string> output = references.GetAll();

            // Assert
            Assert.Equal(0, output.Length);
        }

        [Fact]
        public void GetAll_WhenReferencesHasSome_ShouldReturnSpanWithAllElements()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);

            // Act
            ReadOnlySpan<string> output = references.GetAll();

            // Assert
            Assert.Equal(targets.Length, output.Length);

            for (int index = 0; index < targets.Length; index++)
                Assert.Equal(targets[index], output[index]);
        }

        [Fact]
        public void AddingNonNullObjectReference_WhenReferencesDoesNotHaveSome_ShouldAddObjectReferenceAndSetHasSomeFlagToTrue()
        {
            // Arrange
            string target = nameof(target);
            References<string> references = new References<string>();

            // Act
            references.Add(target);
            bool success = references.TryGetAt(0, out string? output);

            // Assert
            Assert.True(success);
            Assert.True(references.HasSome);
            Assert.Equal(target, output);
        }

        [Fact]
        public void AddingNullObjectReference_WhenReferencesDoesNotHaveSome_ShouldSetHasSomeToFalse()
        {
            // Arrange
            string target = null!;
            References<string> references = new References<string>();

            // Act
            references.Add(target);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void AddingNonNullObjectReferences_WhenReferencesDoesNotHaveSome_ShouldAddObjectReferencesAndSetHasSomeFlagToTrue()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>();

            // Act
            references.Add(targets);
            ReadOnlySpan<string> output = references.GetAll();

            // Assert
            Assert.True(references.HasSome);

            for (int index = 0; index < targets.Length; index++)
                Assert.Equal(targets[index], output[index]);
        }

        [Fact]
        public void AddingNullObjectReferences_WhenReferencesDoesNotHaveSome_ShouldSetHasSomeFlagToFalse()
        {
            // Arrange
            string[] targets = [null!, null!];
            References<string> references = new References<string>();

            // Act
            references.Add(targets);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void AddingObjectReferences_WhenReferencesDoesNotHaveSome_ShouldAddOnlyNonNullObjectReferences()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), null!, nameof(targets),
                                nameof(Reference<string>), nameof(targets), null!,
                                null!, nameof(Reference<string>), nameof(targets)];
            string[] nonNullTargets = targets.Where(target => target is not null).ToArray();
            References<string> references = new References<string>();

            // Act
            references.Add(targets);
            ReadOnlySpan<string> output = references.GetAll();

            // Assert
            Assert.True(references.HasSome);

            for (int index = 0; index < nonNullTargets.Length; index++)
                Assert.Equal(nonNullTargets[index], output[index]);
        }

        [Fact]
        public void RemovingNonNullObjectReference_WhenReferencesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            string target = nameof(target);
            References<string> references = new References<string>();

            // Act
            references.Remove(target);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void RemovingNullObjectReference_WhenReferencesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            string target = null!;
            References<string> references = new References<string>();

            // Act
            references.Remove(target);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void RemovingNonNullObjectReference_WhenReferencesHasSome_ShouldRemoveMatchingObjectReference()
        {
            // Arrange
            string target = nameof(target);
            References<string> references = new References<string>(target);

            // Act
            references.Remove(target);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void RemovingNullObjectReference_WhenReferencesHasSome_ShouldDoNothing()
        {
            // Arrange
            string nullTarget = null!;
            string target = nameof(target);
            References<string> references = new References<string>(target);

            // Act
            references.Remove(nullTarget);

            // Assert
            Assert.True(references.HasSome);
        }

        [Fact]
        public void RemovingNonNullObjectReferences_WhenReferencesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>();

            // Act
            references.Remove(targets);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void RemovingNullObjectReferences_WhenReferencesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            string[] targets = [null!, null!];
            References<string> references = new References<string>();

            // Act
            references.Remove(targets);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void RemovingObjectReferences_WhenReferencesHaveSome_ShouldRemoveMatchingObjectReferences()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), null!, nameof(targets),
                                nameof(Reference<string>), nameof(targets), null!,
                                null!, nameof(Reference<string>), nameof(targets)];
            string[] nonNullTargets = targets.Where(target => target is not null).ToArray();
            References<string> references = new References<string>(targets);

            // Act
            references.Remove(nonNullTargets);

            // Assert
            Assert.False(references.HasSome);
        }

        [Fact]
        public void Clear_WhenReferencesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            References<string> references = new References<string>();
            bool hasSomeBeforeClear = references.HasSome;
            int countBeforeClear = references.Count;

            // Act
            references.Clear();

            // Assert
            Assert.Equal(hasSomeBeforeClear, references.HasSome);
            Assert.Equal(countBeforeClear, references.Count);
        }

        [Fact]
        public void Clear_WhenReferenceHasSome_ShouldResetReference()
        {
            // Arrange
            string[] targets = [nameof(Reference<string>), null!, nameof(targets),
                                nameof(Reference<string>), nameof(targets), null!,
                                null!, nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);
            bool hasSomeBeforeClear = references.HasSome;
            int countBeforeClear = references.Count;

            // Act
            references.Clear();

            // Assert
            Assert.NotEqual(hasSomeBeforeClear, references.HasSome);
            Assert.NotEqual(countBeforeClear, references.Count);
        }

        [Fact]
        public void IfNoneInvokedWithAction_WhenReferencesDoesNotHaveSome_ShouldInvokeProvidedActionAndReturnTrue()
        {
            // Arrange
            bool onNoneInvoked = default;
            References<string> references = new References<string>();
            void onNone() => onNoneInvoked = true;

            // Act
            bool matched = references.IfNone(onNone);

            // Assert
            Assert.True(matched);
            Assert.True(onNoneInvoked);
        }

        [Fact]
        public void IfNoneInvokedWithAction_WhenReferencesHasSome_ShouldReturnFalse()
        {
            // Arrange
            bool onNoneInvoked = default;
            string target = nameof(target);
            References<string> references = new References<string>(target);
            void onNone() => onNoneInvoked = true;

            // Act
            bool matched = references.IfNone(onNone);

            // Assert
            Assert.False(matched);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void IfNoneInvokedWithInputAndActionAcceptingInput_WhenReferencesDoesNotHaveSome_ShouldInvokeProvidedActionPassingInputAndReturnTrue()
        {
            // Arrange
            bool onNoneInvoked = default;
            string input = nameof(input);
            References<string> references = new References<string>();
            void onNone(string noneInput)
            {
                // Assert
                Assert.Equal(input, noneInput);

                onNoneInvoked = true;
            };

            // Act
            bool matched = references.IfNone(onNone, input);

            // Assert
            Assert.True(matched);
            Assert.True(onNoneInvoked);
        }

        [Fact]
        public void IfNoneInvokedWithInputAndActionAcceptingInput_WhenReferencesHasSome_ShouldReturnFalse()
        {
            // Arrange
            bool onNoneInvoked = default;
            string target = nameof(target);
            string input = nameof(input);
            References<string> references = new References<string>(target);
            void onNone(string noneInput)
            {
                // Assert
                Assert.Equal(input, noneInput);

                onNoneInvoked = true;
            };

            // Act
            bool matched = references.IfNone(onNone, input);

            // Assert
            Assert.False(matched);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void IfSomeInvokedWithAction_WhenReferencesHasSome_ShouldInvokeProvidedActionAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);
            void onSome() => onSomeInvokedCount++;

            // Act
            bool matched = references.IfSome(onSome);

            // Assert
            Assert.True(matched);
            Assert.Equal(1, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithAction_WhenReferencesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            References<string> references = new References<string>();
            void onSome() => onSomeInvokedCount++;

            // Act
            bool matched = references.IfSome(onSome);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingInput_WhenReferencesHasSome_ShouldInvokeProvidedActionPassingInputAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            string input = nameof(input);
            References<string> references = new References<string>(targets);
            void onSome(string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = references.IfSome(onSome, input);

            // Assert
            Assert.True(matched);
            Assert.Equal(1, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingInput_WhenReferencesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string input = nameof(input);
            References<string> references = new References<string>();
            void onSome(string someInput) => onSomeInvokedCount++;

            // Act
            bool matched = references.IfSome(onSome, input);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount); ;
        }

        [Fact]
        public void IfSomeInvokedWithActionAcceptingTarget_WhenReferencesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTargetAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);
            void onSome(string someTarget)
            {
                // Assert
                Assert.Contains(someTarget, targets);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = references.IfSome(onSome);

            // Assert
            Assert.True(matched);
            Assert.Equal(targets.Length, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithActionAcceptingTarget_WhenReferencesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            References<string> references = new References<string>();
            void onSome(string someTarget) => onSomeInvokedCount++;

            // Act
            bool matched = references.IfSome(onSome);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingTargetAndInput_WhenReferencesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTargetAndInputAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            string input = nameof(input);
            References<string> references = new References<string>(targets);
            void onSome(string someTarget, string someInput)
            {
                // Assert
                Assert.Contains(someTarget, targets);
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = references.IfSome(onSome, input);

            // Assert
            Assert.True(matched);
            Assert.Equal(targets.Length, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingTargetAndInput_WhenReferencesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string input = nameof(input);
            References<string> references = new References<string>();
            void onSome(string someTarget, string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = references.IfSome(onSome, input);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount); ;
        }

        [Fact]
        public void MatchInvokedWithActionAcceptingTargetAndAction_WhenReferencesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTarget()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            References<string> references = new References<string>(targets);
            void onSome(string someTarget)
            {
                // Assert
                Assert.Contains(someTarget, targets);

                onSomeInvokedCount++;
            };
            void onNone() => onNoneInvoked = true;

            // Act
            references.Match(onSome, onNone);

            // Assert
            Assert.Equal(targets.Length, onSomeInvokedCount);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void MatchInvokedWithActionAcceptingTargetAndAction_WhenReferencesDoesNotHaveSome_ShouldInvokeProvidedAction()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            References<string> references = new References<string>();
            void onSome(string someTarget) => onSomeInvokedCount++;
            void onNone() => onNoneInvoked = true;

            // Act
            references.Match(onSome, onNone);

            // Assert
            Assert.Equal(default, onSomeInvokedCount);
            Assert.True(onNoneInvoked);
        }

        [Fact]
        public void MatchInvokedWithInputAndWithActionAcceptingTargetAndInputAndActionAcceptingInput_WhenReferencesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTargetAndInput()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            string[] targets = [nameof(Reference<string>), nameof(targets)];
            string input = nameof(input);
            References<string> references = new References<string>(targets);
            void onSome(string someTarget, string someInput)
            {
                // Assert
                Assert.Contains(someTarget, targets);
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };
            void onNone(string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onNoneInvoked = true;
            };

            // Act
            references.Match(onSome, onNone, input);

            // Assert
            Assert.Equal(targets.Length, onSomeInvokedCount);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void MatchInvokedWithInputAndActionAcceptingTargetAndInputAndActionAcceptingInput_WhenReferencesDoesNotHaveSome_ShouldInvokeProvidedActionPassingInput()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            string input = nameof(input);
            References<string> references = new References<string>();
            void onSome(string someTarget, string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };
            void onNone(string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onNoneInvoked = true;
            };

            // Act
            references.Match(onSome, onNone, input);

            // Assert
            Assert.Equal(default, onSomeInvokedCount);
            Assert.True(onNoneInvoked);
        }
    }
}
