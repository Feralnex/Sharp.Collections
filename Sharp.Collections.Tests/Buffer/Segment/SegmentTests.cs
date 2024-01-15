using System;
using System.Collections.Generic;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class SegmentTests
    {
        [Fact]
        public void Initialization_WhenSegmentSizeProvided_ShouldInitializeSegmentProperly()
        {
            // Arrange
            Segment<int> segment = new Segment<int>(5);

            // Act & Assert
            Assert.NotNull(segment);
            Assert.Equal(0, segment.Head);
            Assert.Equal(0, segment.Tail);
            Assert.Equal(0, segment.Count);
            Assert.Equal(5, segment.Size);
            Assert.Null(segment.NextHead);
            Assert.Null(segment.NextTail);
        }

        [Fact]
        public void WriteAndRead_WhenItemsWrittenAndRead_ShouldReturnCorrectItem()
        {
            // Arrange
            string firstItem = "Item1";
            string secondItem = "Item2";
            Segment<string> segment = new Segment<string>(3);

            // Act
            segment.Write(firstItem);
            segment.Write(secondItem);
            string item = segment.Read();

            // Assert
            Assert.Equal(firstItem, item);
        }

        [Fact]
        public void MultipleReads_WhenItemsWrittenAndRead_ShouldReturnCorrectItems()
        {
            // Arrange
            int segmnetSize = 3;
            int offset = 4;
            Segment<int> segment = new Segment<int>(segmnetSize);
            List<int> expectedValues = [4, 5, 6];
            List<int> actualValues = [];

            // Act
            for (int index = 0; index < segmnetSize; index++)
                segment.Write(index);

            for (int index = 0; index < segmnetSize; index++)
                segment.Read();

            for (int index = 0; index < segmnetSize; index++)
                segment.Write(index + offset);

            for (int index = 0; index < segmnetSize; index++)
            {
                int expected = segment.Read();

                actualValues.Add(expected);
            }

            // Assert
            Assert.Equal(expectedValues, actualValues);
        }

        [Fact]
        public void TryWrite_WhenSegmentFull_ShouldReturnFalse()
        {
            // Arrange
            Segment<double> segment = new Segment<double>(2);

            // Act
            segment.Write(1.0);
            segment.Write(2.0);

            // Assert
            Assert.False(segment.TryWrite(3.0));
        }

        [Fact]
        public void TryRead_WhenSegmentEmpty_ShouldReturnFalse()
        {
            // Arrange
            Segment<int> segment = new Segment<int>(2);

            // Act
            bool succeded = segment.TryRead(out int item);

            // Assert
            Assert.False(succeded);
            Assert.Equal(default, item);
        }

        [Fact]
        public void Write_WhenMoveTailFalse_ShouldNotIncrementTail()
        {
            // Arrange
            Segment<int> segment = new Segment<int>(3);

            // Act
            segment.Write(1, moveTail: false);

            // Assert
            Assert.Equal(0, segment.Tail);
        }

        [Fact]
        public void TryRead_WhenMoveHeadIsFalse_ShouldNotIncrementHead()
        {
            // Arrange
            string item = "Item";
            Segment<string> segment = new Segment<string>(2);

            // Act
            segment.Write(item);
            segment.TryRead(out _, moveHead: false);

            // Assert
            Assert.Equal(0, segment.Head);
        }

        [Fact]
        public void Write_WhenSegmentOverflow_ShouldThrowIndexOutOfRangeException()
        {
            // Arrange
            Segment<int> segment = new Segment<int>(2);

            // Act
            segment.Write(1);
            segment.Write(2);

            // Assert
            Assert.Throws<IndexOutOfRangeException>(() => segment.Write(3));
        }

        [Fact]
        public void Read_WhenSegmentEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var segment = new Segment<double>(2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => segment.Read());
        }
    }
}