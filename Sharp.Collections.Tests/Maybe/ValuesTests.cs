using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class ValuesTests
    {
        private Random _random;

        public ValuesTests()
            => _random = new Random();

        [Fact]
        public void Initialization_WhenUsedDefaultConstructor_ShouldSetHasSomeToFalse()
        {
            // Arrange and Act
            Values<int> values = new Values<int>();

            // Assert
            Assert.False(values.HasSome);
        }

        [Fact]
        public void Initialization_WhenProvidedValue_ShouldSetHasSomeToTrue()
        {
            // Arrange
            int target = 1;

            // Act
            Values<int> values = new Values<int>(target);

            // Assert
            Assert.True(values.HasSome);
        }

        [Fact]
        public void Initialization_WhenProvidedValues_ShouldSetHasSomeToTrue()
        {
            // Arrange
            int[] targets = [1, 2];

            // Act
            Values<int> values = new Values<int>(targets);

            // Assert
            Assert.True(values.HasSome);
        }

        [Fact]
        public void TryGet_WhenValuesDoesNotHaveSome_ShouldReturnFalseAndAssignDefaultOutput()
        {
            // Arrange
            Values<int> values = new Values<int>();

            // Act
            bool success = values.TryGet(out ReadOnlySpan<int> output);

            // Assert
            Assert.False(success);
            Assert.True(output.IsEmpty);
        }

        [Fact]
        public void TryGet_WhenValuesHasSome_ShouldReturnTrueAndAssignOutput()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);

            // Act
            bool success = values.TryGet(out ReadOnlySpan<int> output);

            // Assert
            Assert.True(success);
            
            for (int index = 0; index < targets.Length; index++)
                Assert.Equal(targets[index], output[index]);
        }

        [Fact]
        public void TryGetAtInvokedWithAnyIndex_WhenValuesDoesNotHaveSome_ShouldReturnFalseAndAssignDefaultOutput()
        {
            // Arrange
            Values<int> values = new Values<int>();
            int index = _random.Next(0, int.MaxValue);

            // Act
            bool success = values.TryGetAt(index, out int output);

            // Assert
            Assert.False(success);
            Assert.Equal(default, output);
        }

        [Fact]
        public void TryGetAtInvokedWithIndexInRangeOfValues_WhenValuesHasSome_ShouldReturnTrueAndAssignOutput()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);
            int index = _random.Next(0, targets.Length);

            // Act
            bool success = values.TryGetAt(index, out int output);

            // Assert
            Assert.True(success);
            Assert.Equal(targets[index], output);
        }

        [Fact]
        public void TryGetAtInvokedWithIndexExceedingRangeOfValues_WhenValuesHasSome_ShouldReturnFalseAndAssignDefaultOutput()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);
            int index = _random.Next(targets.Length, int.MaxValue);

            // Act
            bool success = values.TryGetAt(index, out int output);

            // Assert
            Assert.False(success);
            Assert.Equal(default, output);
        }

        [Fact]
        public void GetAll_WhenValuesDoesNotHaveSome_ShouldReturnEmptySpan()
        {
            // Arrange
            Values<int> values = new Values<int>();

            // Act
            ReadOnlySpan<int> output = values.GetAll();

            // Assert
            Assert.Equal(0, output.Length);
        }

        [Fact]
        public void GetAll_WhenValuesHasSome_ShouldReturnSpanWithAllElements()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);

            // Act
            ReadOnlySpan<int> output = values.GetAll();

            // Assert
            Assert.Equal(targets.Length, output.Length);

            for (int index = 0; index < targets.Length; index++)
                Assert.Equal(targets[index], output[index]);
        }

        [Fact]
        public void AddingValue_WhenValuesDoesNotHaveSome_ShouldAddValueAndSetHasSomeFlagToTrue()
        {
            // Arrange
            int target = 1;
            Values<int> values = new Values<int>();

            // Act
            values.Add(target);
            bool success = values.TryGetAt(0, out int output);

            // Assert
            Assert.True(success);
            Assert.True(values.HasSome);
            Assert.Equal(target, output);
        }

        [Fact]
        public void AddingValues_WhenValuesDoesNotHaveSome_ShouldAddValuesAndSetHasSomeFlagToTrue()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>();

            // Act
            values.Add(targets);
            ReadOnlySpan<int> output = values.GetAll();

            // Assert
            Assert.True(values.HasSome);

            for (int index = 0; index < targets.Length; index++)
                Assert.Equal(targets[index], output[index]);
        }

        [Fact]
        public void RemovingValue_WhenValuesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            int target = 1;
            Values<int> values = new Values<int>();

            // Act
            values.Remove(target);

            // Assert
            Assert.False(values.HasSome);
        }

        [Fact]
        public void RemovingValue_WhenValuesHasSome_ShouldRemoveMatchingValue()
        {
            // Arrange
            int target = 1;
            Values<int> values = new Values<int>(target);

            // Act
            values.Remove(target);

            // Assert
            Assert.False(values.HasSome);
        }

        [Fact]
        public void RemovingValues_WhenValuesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>();

            // Act
            values.Remove(targets);

            // Assert
            Assert.False(values.HasSome);
        }

        [Fact]
        public void RemovingValues_WhenValuesHasSome_ShouldRemoveMatchingValues()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);

            // Act
            values.Remove(targets);

            // Assert
            Assert.False(values.HasSome);
        }

        [Fact]
        public void Clear_WhenValuesDoesNotHaveSome_ShouldDoNothing()
        {
            // Arrange
            Values<int> values = new Values<int>();
            bool hasSomeBeforeClear = values.HasSome;
            int countBeforeClear = values.Count;

            // Act
            values.Clear();

            // Assert
            Assert.Equal(hasSomeBeforeClear, values.HasSome);
            Assert.Equal(countBeforeClear, values.Count);
        }

        [Fact]
        public void Clear_WhenValuesHasSome_ShouldResetReference()
        {
            // Arrange
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);
            bool hasSomeBeforeClear = values.HasSome;
            int countBeforeClear = values.Count;

            // Act
            values.Clear();

            // Assert
            Assert.NotEqual(hasSomeBeforeClear, values.HasSome);
            Assert.NotEqual(countBeforeClear, values.Count);
        }

        [Fact]
        public void IfNoneInvokedWithAction_WhenValuesDoesNotHaveSome_ShouldInvokeProvidedActionAndReturnTrue()
        {
            // Arrange
            bool onNoneInvoked = default;
            Values<int> values = new Values<int>();
            void onNone() => onNoneInvoked = true;

            // Act
            bool matched = values.IfNone(onNone);

            // Assert
            Assert.True(matched);
            Assert.True(onNoneInvoked);
        }

        [Fact]
        public void IfNoneInvokedWithAction_WhenValuesHasSome_ShouldReturnFalse()
        {
            // Arrange
            bool onNoneInvoked = default;
            int target = 1;
            Values<int> values = new Values<int>(target);
            void onNone() => onNoneInvoked = true;

            // Act
            bool matched = values.IfNone(onNone);

            // Assert
            Assert.False(matched);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void IfNoneInvokedWithInputAndActionAcceptingInput_WhenValuesDoesNotHaveSome_ShouldInvokeProvidedActionPassingInputAndReturnTrue()
        {
            // Arrange
            bool onNoneInvoked = default;
            string input = nameof(input);
            Values<int> values = new Values<int>();
            void onNone(string noneInput)
            {
                // Assert
                Assert.Equal(input, noneInput);

                onNoneInvoked = true;
            };

            // Act
            bool matched = values.IfNone(onNone, input);

            // Assert
            Assert.True(matched);
            Assert.True(onNoneInvoked);
        }

        [Fact]
        public void IfNoneInvokedWithInputAndActionAcceptingInput_WhenValuesHasSome_ShouldReturnFalse()
        {
            // Arrange
            bool onNoneInvoked = default;
            int target = 1;
            string input = nameof(input);
            Values<int> values = new Values<int>(target);
            void onNone(string noneInput)
            {
                // Assert
                Assert.Equal(input, noneInput);

                onNoneInvoked = true;
            };

            // Act
            bool matched = values.IfNone(onNone, input);

            // Assert
            Assert.False(matched);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void IfSomeInvokedWithAction_WhenValuesHasSome_ShouldInvokeProvidedActionAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);
            void onSome() => onSomeInvokedCount++;

            // Act
            bool matched = values.IfSome(onSome);

            // Assert
            Assert.True(matched);
            Assert.Equal(1, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithAction_WhenValuesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            Values<int> values = new Values<int>();
            void onSome() => onSomeInvokedCount++;

            // Act
            bool matched = values.IfSome(onSome);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingInput_WhenValuesHasSome_ShouldInvokeProvidedActionPassingInputAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            int[] targets = [1, 2];
            string input = nameof(input);
            Values<int> values = new Values<int>(targets);
            void onSome(string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = values.IfSome(onSome, input);

            // Assert
            Assert.True(matched);
            Assert.Equal(1, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingInput_WhenValuesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string input = nameof(input);
            Values<int> values = new Values<int>();
            void onSome(string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = values.IfSome(onSome, input);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount); ;
        }

        [Fact]
        public void IfSomeInvokedWithActionAcceptingTarget_WhenValuesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTargetAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);
            void onSome(int someTarget)
            {
                // Assert
                Assert.Contains(someTarget, targets);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = values.IfSome(onSome);

            // Assert
            Assert.True(matched);
            Assert.Equal(targets.Length, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithActionAcceptingTarget_WhenValuesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            Values<int> values = new Values<int>();
            void onSome(int someTarget) => onSomeInvokedCount++;

            // Act
            bool matched = values.IfSome(onSome);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingTargetAndInput_WhenValuesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTargetAndInputAndReturnTrue()
        {
            // Arrange
            int onSomeInvokedCount = default;
            int[] targets = [1, 2];
            string input = nameof(input);
            Values<int> values = new Values<int>(targets);
            void onSome(int someTarget, string someInput)
            {
                // Assert
                Assert.Contains(someTarget, targets);
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = values.IfSome(onSome, input);

            // Assert
            Assert.True(matched);
            Assert.Equal(targets.Length, onSomeInvokedCount);
        }

        [Fact]
        public void IfSomeInvokedWithInputAndActionAcceptingTargetAndInput_WhenValuesDoesNotHaveSome_ShouldReturnFalse()
        {
            // Arrange
            int onSomeInvokedCount = default;
            string input = nameof(input);
            Values<int> values = new Values<int>();
            void onSome(int someTarget, string someInput)
            {
                // Assert
                Assert.Equal(input, someInput);

                onSomeInvokedCount++;
            };

            // Act
            bool matched = values.IfSome(onSome, input);

            // Assert
            Assert.False(matched);
            Assert.Equal(default, onSomeInvokedCount); ;
        }

        [Fact]
        public void MatchInvokedWithActionAcceptingTargetAndAction_WhenValuesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTarget()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            int[] targets = [1, 2];
            Values<int> values = new Values<int>(targets);
            void onSome(int someTarget)
            {
                // Assert
                Assert.Contains(someTarget, targets);

                onSomeInvokedCount++;
            };
            void onNone() => onNoneInvoked = true;

            // Act
            values.Match(onSome, onNone);

            // Assert
            Assert.Equal(targets.Length, onSomeInvokedCount);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void MatchInvokedWithActionAcceptingTargetAndAction_WhenValuesDoesNotHaveSome_ShouldInvokeProvidedAction()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            Values<int> values = new Values<int>();
            void onSome(int someTarget) => onSomeInvokedCount++;
            void onNone() => onNoneInvoked = true;

            // Act
            values.Match(onSome, onNone);

            // Assert
            Assert.Equal(default, onSomeInvokedCount);
            Assert.True(onNoneInvoked);
        }

        [Fact]
        public void MatchInvokedWithInputAndWithActionAcceptingTargetAndInputAndActionAcceptingInput_WhenValuesHasSome_ShouldInvokeProvidedActionForEachTargetPassingTargetAndInput()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            int[] targets = [1, 2];
            string input = nameof(input);
            Values<int> values = new Values<int>(targets);
            void onSome(int someTarget, string someInput)
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
            values.Match(onSome, onNone, input);

            // Assert
            Assert.Equal(targets.Length, onSomeInvokedCount);
            Assert.False(onNoneInvoked);
        }

        [Fact]
        public void MatchInvokedWithInputAndActionAcceptingTargetAndInputAndActionAcceptingInput_WhenValuesDoesNotHaveSome_ShouldInvokeProvidedActionPassingInput()
        {
            // Arrange
            int onSomeInvokedCount = default;
            bool onNoneInvoked = default;
            string input = nameof(input);
            Values<int> values = new Values<int>();
            void onSome(int someTarget, string someInput)
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
            values.Match(onSome, onNone, input);

            // Assert
            Assert.Equal(default, onSomeInvokedCount);
            Assert.True(onNoneInvoked);
        }
    }
}
