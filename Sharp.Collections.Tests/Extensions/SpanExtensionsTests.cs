using Sharp.Collections.Extensions;
using System;
using Xunit;

namespace Sharp.Collections.Tests.Extensions
{
    public class SpanExtensionsTests
    {
        [Fact]
        public void IndexOfAnyNumberExcept_WhenDifferentValueExists_ShouldReturnCorrectIndex()
        {
            // Arrange
            Span<int> span = [5, 5, 5, 2, 5];
            int valueToAvoid = 5;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void IndexOfAnyNumberExcept_WhenAllValuesAreTheSameAsValueToAvoid_ShouldReturnNegativeOne()
        {
            // Arrange
            Span<int> span = [5, 5, 5, 5, 5];
            int valueToAvoid = 5;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOfAnyNumberExcept_WhenSpanIsEmpty_ShouldReturnNegativeOne()
        {
            // Arrange
            Span<int> span = Span<int>.Empty;
            int valueToAvoid = 5;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOfAnyNumberExcept_WhenFirstElementIsDifferent_ShouldReturnZero()
        {
            // Arrange
            Span<int> span = [2, 5, 5, 5, 5];
            int valueToAvoid = 5;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void IndexOfAnyNumberExcept_WhenLastElementIsDifferent_ShouldReturnLastIndex()
        {
            // Arrange
            Span<int> span = [5, 5, 5, 5, 2];
            int valueToAvoid = 5;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid);

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void IndexOfAnyNumberExceptAcceptingOffset_WhenDifferentValueExists_ShouldReturnCorrectIndex()
        {
            // Arrange
            Span<int> span = [5, 5, 5, 2, 5];
            int valueToAvoid = 5;
            int offset = 2;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid, offset);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void IndexOfAnyNumberExceptAcceptingOffset_WhenAllValuesAreSameAsValueToAvoid_ShouldReturnNegativeOne()
        {
            // Arrange
            Span<int> span = [5, 5, 5, 5, 5];
            int valueToAvoid = 5;
            int offset = 1;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid, offset);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOfAnyNumberExceptAcceptingOffset_WhenSpanIsEmpty_ShouldReturnNegativeOne()
        {
            // Arrange
            Span<int> span = Span<int>.Empty;
            int valueToAvoid = 5;
            int offset = 1;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid, offset);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOfAnyNumberExceptAcceptingOffset_WhenNoneOfTheElementsAtOffsetAreDifferent_ShouldReturnNegativeOne()
        {
            // Arrange
            Span<int> span = [5, 5, 2, 5, 5];
            int valueToAvoid = 5;
            int offset = 2;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid, offset);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOfAnyNumberExceptAcceptingOffset_WhenOffsetIsOutOfRange_ShouldReturnNegativeOne()
        {
            // Arrange
            Span<int> span = [5, 5, 5, 5, 5];
            int valueToAvoid = 5;
            int offset = 6;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid, offset);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOfAnyNumberExceptAcceptingOffset_WhenOffsetIsZero_ShouldCheckFirstElementOnly()
        {
            // Arrange
            Span<int> span = [2, 5, 5, 5, 5];
            int valueToAvoid = 5;
            int offset = 0;

            // Act
            int result = span.IndexOfAnyNumberExcept(valueToAvoid, offset);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public unsafe void CopyTo_WhenAllAsciiCharacters_ShouldCopyCorrectlyAndNullTerminate()
        {
            // Arrange
            ReadOnlySpan<char> source = "Hello";
            sbyte* destination = stackalloc sbyte[source.Length + 1];

            // Act
            source.CopyTo(destination);

            // Assert
            Assert.Equal((sbyte)'H', destination[0]);
            Assert.Equal((sbyte)'e', destination[1]);
            Assert.Equal((sbyte)'l', destination[2]);
            Assert.Equal((sbyte)'l', destination[3]);
            Assert.Equal((sbyte)'o', destination[4]);
            Assert.Equal(0, destination[5]); // null terminator
        }

        [Fact]
        public unsafe void CopyTo_WhenContainsNonAsciiCharacters_ShouldReplaceWithQuestionMark()
        {
            // Arrange
            ReadOnlySpan<char> source = "Hęllo"; // 'ę' is non-ASCII
            sbyte* destination = stackalloc sbyte[source.Length + 1];

            // Act
            source.CopyTo(destination);

            // Assert
            Assert.Equal((sbyte)'H', destination[0]);
            Assert.Equal((sbyte)'?', destination[1]); // replaced non-ASCII
            Assert.Equal((sbyte)'l', destination[2]);
            Assert.Equal((sbyte)'l', destination[3]);
            Assert.Equal((sbyte)'o', destination[4]);
            Assert.Equal(0, destination[5]); // null terminator
        }

        [Fact]
        public unsafe void CopyTo_WhenEmptySpan_ShouldOnlyWriteNullTerminator()
        {
            // Arrange
            ReadOnlySpan<char> source = ReadOnlySpan<char>.Empty;
            sbyte* destination = stackalloc sbyte[1];

            // Act
            source.CopyTo(destination);

            // Assert
            Assert.Equal(0, destination[0]); // only null terminator
        }
    }
}
