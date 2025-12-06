using System;
using System.Collections.Generic;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class SegmentTests
    {
        private static readonly Random _random = new Random();

        [Fact]
        public void NewSegment_WhenSegmentSizeProvided_ShouldCreateNewSegment()
        {
            // Arrange
            int segmentSize = _random.Next();

            // Act
            Segment<int> segment = new Segment<int>(segmentSize);

            // Assert
            Assert.NotNull(segment);
            Assert.Equal(0, segment.Head);
            Assert.Equal(0, segment.Tail);
            Assert.Equal(0, segment.Count);
            Assert.Equal(segmentSize, segment.Size);
            Assert.Null(segment.NextHead);
            Assert.Null(segment.NextTail);
        }

        [Fact]
        public void WriteAndRead_WhenItemsAreWrittenAndRead_ShouldReturnCorrectItem()
        {
            // Arrange
            string firstItem = nameof(firstItem);
            string secondItem = nameof(secondItem);
            Segment<string> segment = new Segment<string>(3);

            // Act
            segment.Write(firstItem);
            segment.Write(secondItem);
            string item = segment.Read();

            // Assert
            Assert.Equal(firstItem, item);
        }

        [Fact]
        public void MultipleReads_WhenItemsAreWrittenAndRead_ShouldReturnCorrectItems()
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
        public void TryWrite_WhenSegmentIsFull_ShouldReturnFalse()
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
        public void TryRead_WhenSegmentIsEmpty_ShouldReturnFalse()
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
        public void Write_WhenMoveTailIsFalse_ShouldNotIncrementTail()
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
            string item = nameof(item);
            Segment<string> segment = new Segment<string>(2);

            // Act
            segment.Write(item);
            segment.TryRead(out _, moveHead: false);

            // Assert
            Assert.Equal(0, segment.Head);
        }

        [Fact]
        public void Write_WhenSegmentOverflows_ShouldThrowIndexOutOfRangeException()
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
        public void Read_WhenSegmentIsEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var segment = new Segment<double>(2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => segment.Read());
        }
    }
}