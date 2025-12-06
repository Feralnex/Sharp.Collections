using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class ConcurrentSegmentTests
    {
        private static readonly Random _random = new Random();

        [Fact]
        public void NewConcurrentSegment_WhenProvidedSegmentSize_ShouldCreateConcurrentSegment()
        {
            // Arrange
            int segmentSize = _random.Next();

            // Act
            ConcurrentSegment<int> segment = new ConcurrentSegment<int>(segmentSize);

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
            ConcurrentSegment<string> segment = new ConcurrentSegment<string>(3);

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
            int segmentSize = 3;
            int offset = 4;
            ConcurrentSegment<int> segment = new ConcurrentSegment<int>(segmentSize);
            List<int> expectedValues = [4, 5, 6];
            List<int> actualValues = [];

            // Act
            for (int index = 0; index < segmentSize; index++)
                segment.Write(index);

            for (int index = 0; index < segmentSize; index++)
                segment.Read();

            for (int index = 0; index < segmentSize; index++)
                segment.Write(index + offset);

            for (int index = 0; index < segmentSize; index++)
            {
                int expected = segment.Read();

                actualValues.Add(expected);
            }

            // Assert
            Assert.Equal(expectedValues, actualValues);
        }

        [Fact]
        public void TryWrite_WhenConcurrentSegmentIsFull_ShouldReturnFalse()
        {
            // Arrange
            ConcurrentSegment<double> segment = new ConcurrentSegment<double>(2);

            // Act
            segment.Write(1.0);
            segment.Write(2.0);

            // Assert
            Assert.False(segment.TryWrite(3.0));
        }

        [Fact]
        public void TryWrite_WhenConcurrentSegmentIsNotFull_ShouldReturnTrue()
        {
            // Arrange
            ConcurrentSegment<double> segment = new ConcurrentSegment<double>(3);

            // Act
            segment.Write(1.0);
            segment.Write(2.0);

            // Assert
            Assert.True(segment.TryWrite(3.0));
        }

        [Fact]
        public void TryRead_WhenConcurrentSegmentIsEmpty_ShouldReturnFalse()
        {
            // Arrange
            ConcurrentSegment<int> segment = new ConcurrentSegment<int>(2);

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
            ConcurrentSegment<int> segment = new ConcurrentSegment<int>(3);

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
            ConcurrentSegment<string> segment = new ConcurrentSegment<string>(2);

            // Act
            segment.Write(item);
            segment.TryRead(out _, moveHead: false);

            // Assert
            Assert.Equal(0, segment.Head);
        }

        [Fact]
        public void Write_WhenConcurrentSegmentOverflows_ShouldThrowIndexOutOfRangeException()
        {
            // Arrange
            ConcurrentSegment<int> segment = new ConcurrentSegment<int>(2);

            // Act
            segment.Write(1);
            segment.Write(2);

            // Assert
            Assert.Throws<IndexOutOfRangeException>(() => segment.Write(3));
        }

        [Fact]
        public void Read_WhenConcurrentSegmentIsEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            ConcurrentSegment<double> segment = new ConcurrentSegment<double>(2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => segment.Read());
        }

        [Fact]
        public void WriteAndRead_InConcurrentEnvironment_ShouldMaintainCorrectItemCount()
        {
            // Arrange
            int numberOfThreads = 5;
            int itemsPerThread = 10000;
            int segmentSize = numberOfThreads * itemsPerThread;
            ConcurrentSegment<int> segment = new ConcurrentSegment<int>(segmentSize);

            // Act
            Parallel.For(0, numberOfThreads, threadIndex =>
            {
                for (int item = 0; item < itemsPerThread; item++)
                {
                    segment.Write(item);

                    bool succeeded = segment.TryRead(out int result);
                    while (!succeeded)
                        succeeded = segment.TryRead(out result);
                }
            });

            // Assert
            Assert.Equal(segmentSize, segment.Head);
            Assert.Equal(segmentSize, segment.Tail);
            Assert.Equal(0, segment.Count);
        }
    }
}