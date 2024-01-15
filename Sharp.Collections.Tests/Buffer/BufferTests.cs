using System;
using System.Collections.Generic;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class BufferTests
    {
        [Fact]
        public void Initialization_WhenBufferSizeProvided_ShouldInitializeBufferProperly()
        {
            // Arrange
            Buffer<int> buffer = new Buffer<int>(5);

            // Act & Assert
            Assert.NotNull(buffer);
            Assert.Equal(0, buffer.Head);
            Assert.Equal(0, buffer.Tail);
            Assert.Equal(0, buffer.Count);
            Assert.Equal(5, buffer.Size);
        }

        [Fact]
        public void WriteAndRead_WhenItemsWrittenAndRead_ShouldReturnCorrectItem()
        {
            // Arrange
            string firstItem = "Item1";
            string secondItem = "Item2";
            Buffer<string> buffer = new Buffer<string>(3);

            // Act
            buffer.Write(firstItem);
            buffer.Write(secondItem);
            string item = buffer.Read();

            // Assert
            Assert.Equal(firstItem, item);
        }

        [Fact]
        public void MultipleReads_WhenItemsWrittenAndRead_ShouldReturnCorrectItems()
        {
            // Arrange
            int bufferSize = 3;
            int offset = 4;
            Buffer<int> buffer = new Buffer<int>(bufferSize);
            List<int> expectedValues = [4, 5, 6];
            List<int> actualValues = [];

            // Act
            for (int index = 0; index < bufferSize; index++)
                buffer.Write(index);

            for (int index = 0; index < bufferSize; index++)
                buffer.Read();

            for (int index = 0; index < bufferSize; index++)
                buffer.Write(index + offset);

            for (int index = 0; index < bufferSize; index++)
            {
                int expected = buffer.Read();

                actualValues.Add(expected);
            }

            // Assert
            Assert.Equal(expectedValues, actualValues);
        }

        [Fact]
        public void TryWrite_WhenBufferFull_ShouldReturnFalse()
        {
            // Arrange
            Buffer<double> buffer = new Buffer<double>(2);

            // Act
            buffer.Write(1.0);
            buffer.Write(2.0);

            // Assert
            Assert.False(buffer.TryWrite(3.0));
        }

        [Fact]
        public void TryRead_WhenBufferEmpty_ShouldReturnFalse()
        {
            // Arrange
            Buffer<int> buffer = new Buffer<int>(2);

            // Act
            bool succeded = buffer.TryRead(out int item);

            // Assert
            Assert.False(succeded);
            Assert.Equal(default, item);
        }

        [Fact]
        public void Write_WhenMoveTailFalse_ShouldNotIncrementTail()
        {
            // Arrange
            Buffer<int> buffer = new Buffer<int>(3);

            // Act
            buffer.Write(1, moveTail: false);

            // Assert
            Assert.Equal(0, buffer.Tail);
        }

        [Fact]
        public void TryRead_WhenMoveHeadIsFalse_ShouldNotIncrementHead()
        {
            // Arrange
            string item = "Item";
            Buffer<string> buffer = new Buffer<string>(2);

            // Act
            buffer.Write(item);
            buffer.TryRead(out _, moveHead: false);

            // Assert
            Assert.Equal(0, buffer.Head);
        }

        [Fact]
        public void Write_WhenBufferOverflow_ShouldThrowIndexOutOfRangeException()
        {
            // Arrange
            Buffer<int> buffer = new Buffer<int>(2);

            // Act
            buffer.Write(1);
            buffer.Write(2);

            // Assert
            Assert.Throws<IndexOutOfRangeException>(() => buffer.Write(3));
        }

        [Fact]
        public void Read_WhenBufferEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var buffer = new Buffer<double>(2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => buffer.Read());
        }
    }
}