using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class QueueTests
    {
        [Fact]
        public void Enqueue_WhenItemEnqueued_ShouldIncreaseCount()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();

            // Act
            queue.Enqueue(1);

            // Assert
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void Enqueue_WhenNullEnqueued_ShouldThrowArgumentNullException()
        {
            // Arrange
            Queue<object> queue = new Queue<object>();

            // Assert
            Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null!));
        }

        [Fact]
        public void TryEnqueue_WhenNullEnqueued_ShouldReturnFalse()
        {
            // Arrange
            Queue<object> queue = new Queue<object>();

            bool succeded = queue.TryEnqueue(null!);

            // Assert
            Assert.False(succeded);
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public void Dequeue_WhenQueueNotEmpty_ShouldRemoveAndReturnItem()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);

            // Act
            int item = queue.Dequeue();

            // Assert
            Assert.Equal(1, item);
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public void Dequeue_WhenQueueEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }

        [Fact]
        public void TryDequeue_WhenQueueNotEmpty_ShouldRemoveAndReturnItem()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);

            // Act
            bool success = queue.TryDequeue(out var item);

            // Assert
            Assert.True(success);
            Assert.Equal(1, item);
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public void TryDequeue_WhenQueueEmpty_ShouldReturnFalse()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();

            // Act
            bool success = queue.TryDequeue(out var item);

            // Assert
            Assert.False(success);
            Assert.Equal(0, queue.Count);
            Assert.Equal(default, item);
        }

        [Fact]
        public void Peek_WhenQueueNotEmpty_ShouldReturnItemWithoutRemoving()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);

            // Act
            int item = queue.Peek();

            // Assert
            Assert.Equal(1, item);
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void Peek_WhenQueueEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
        }

        [Fact]
        public void TryPeek_WhenQueueNotEmpty_ShouldReturnItemWithoutRemoving()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);

            // Act
            bool success = queue.TryPeek(out var item);

            // Assert
            Assert.True(success);
            Assert.Equal(1, item);
            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void TryPeek_WhenQueueEmpty_ShouldReturnFalse()
        {
            // Arrange
            Queue<int> queue = new Queue<int>();

            // Act
            bool success = queue.TryPeek(out var item);

            // Assert
            Assert.False(success);
            Assert.Equal(0, queue.Count);
            Assert.Equal(default, item);
        }

        [Fact]
        public void Enqueue_SegmentSizeItems_ThenDequeue_ShouldDequeueCorrectItems()
        {
            // Arrange
            int segmentSize = 5;
            int numberOfItems = 5;
            Queue<int> queue = new Queue<int>(segmentSize);
            List<int> expectedItems = [.. Enumerable.Range(1, segmentSize)];
            List<int> actualItems = [];

            // Act
            for (int index = 1; index <= numberOfItems; index++)
                queue.Enqueue(index);

            for (int index = 1; index <= numberOfItems; index++)
            {
                int item = queue.Dequeue();

                actualItems.Add(item);
            }

            // Assert
            Assert.Equal(0, queue.Count);
            Assert.Equal(expectedItems, actualItems);
        }

        [Fact]
        public void Enqueue_MoreThanSegmentSizeItems_ShouldEnqueueItemsSuccessfully()
        {
            // Arrange
            int segmentSize = 5;
            int numberOfItems = 10;
            Queue<int> queue = new Queue<int>(segmentSize);

            // Act
            for (int index = 1; index <= numberOfItems; index++)
                queue.Enqueue(index);

            // Assert
            Assert.Equal(numberOfItems, queue.Count);
        }

        [Fact]
        public void Enqueue_MoreThanSegmentSizeItems_ThenDequeue_ShouldDequeueCorrectItems()
        {
            // Arrange
            int numberOfItems = 100000;
            int iterationSize = numberOfItems / 10;
            Queue<int> queue = new Queue<int>();
            List<int> expectedItems = [.. Enumerable.Range(0, numberOfItems)];
            List<int> actualItems = [];

            // Act
            for (int iteration = 0; iteration < 10; iteration++)
            {
                for (int item = iteration * iterationSize; item < (iteration + 1) * iterationSize; item++)
                    queue.Enqueue(item);

                for (int index = 0; index < iterationSize; index++)
                {
                    int item = queue.Dequeue();

                    actualItems.Add(item);
                }
            }

            // Assert
            Assert.Equal(0, queue.Count);
            Assert.Equal(expectedItems, actualItems);
        }

        [Fact]
        public void Enqueue_MoreThanSegmentSizeItems_ThenTryDequeue_ShouldDequeueCorrectItems()
        {
            // Arrange
            int numberOfItems = 100000;
            int iterationSize = numberOfItems / 10;
            Queue<int> queue = new Queue<int>();
            List<int> expectedItems = [.. Enumerable.Range(0, numberOfItems)];
            List<bool> actualResults = [];
            List<int> actualItems = [];

            // Act
            for (int iteration = 0; iteration < 10; iteration++)
            {
                for (int item = iteration * iterationSize; item < (iteration + 1) * iterationSize; item++)
                    queue.Enqueue(item);

                for (int index = 0; index < iterationSize; index++)
                {
                    bool succeeded = queue.TryDequeue(out int item);

                    actualResults.Add(succeeded);
                    actualItems.Add(item);
                }
            }

            // Assert
            Assert.Equal(0, queue.Count);
            Assert.All(actualResults, Assert.True);
            Assert.Equal(expectedItems, actualItems);
        }
    }
}