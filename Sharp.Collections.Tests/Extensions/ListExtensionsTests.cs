using Sharp.Collections.Extensions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sharp.Collections.Tests.Extensions
{
    public class ListExtensionsTests
    {
        [Fact]
        public void SetAndGetItems_WhenItemsHaveBeenChanged_ShouldBeReflectedInGetItems()
        {
            // Arrange
            List<int> list = [1, 2, 3];
            int[] newItems = [10, 20, 30, 40];

            // Act
            list.SetItems(newItems);
            int[] actualItems = list.GetItems();

            // Assert
            Assert.Same(newItems, actualItems);
            Assert.Equal(10, actualItems[0]);
        }

        [Fact]
        public void SetAndGetCount_WhenCountHasBeenChanged_ShouldBeReflectedInGetCount()
        {
            // Arrange
            int count = 4;
            List<string> list = [1.ToString(), 2.ToString(), 3.ToString()];

            // Act
            list.SetCount(count);
            int currentCount = list.GetCount();

            // Assert
            Assert.Equal(count, currentCount);
            Assert.Equal(count, list.Count);
        }

        [Fact]
        public void SetAndGetVersion_WhenVersionHasBeenChanged_ShouldBeReflectedInGetVersion()
        {
            // Arrange
            List<string> list = [1.ToString(), 2.ToString()];

            // Act
            list.SetVersion(9999);
            int version = list.GetVersion();

            // Assert
            Assert.Equal(9999, version);
        }

        [Fact]
        public void GetWithTheLeastCountOrAdd_WhenListHasCollectionsUnderMaxSize_ShouldReturnCollectionWithTheLeastCount()
        {
            // Arrange
            List<List<int>> lists = new List<List<int>>
            {
                new List<int> { 1, 2, 4 },
                new List<int> { 1 },
                new List<int> { 1, 2 }
            };
            int maxSize = 4;

            // Act
            List<int> result = lists.GetWithTheLeastCountOrAdd(maxSize, () => new List<int>());

            // Assert
            Assert.Same(lists[1], result); // list[1] has the least count: 1
        }

        [Fact]
        public void GetWithTheLeastCountOrAdd_WhenAllCollectionsExceedMaxSize_ShouldAddNewCollection()
        {
            // Arrange
            List<List<int>> lists = new List<List<int>>
            {
                new List<int> { 1, 2, 4, 8 },
                new List<int> { 1, 2, 4, 8 }
            };
            List<int> newCollection = new List<int> { 16 };
            int countBeforeInvoking = lists.Count;
            int maxSize = 4;

            // Act
            List<int> result = lists.GetWithTheLeastCountOrAdd(maxSize, () => newCollection);

            // Assert
            Assert.Equal(countBeforeInvoking + 1, lists.Count); // New collection added
            Assert.Same(newCollection, result);
        }

        [Fact]
        public void GetWithTheLeastCountOrAdd_WhenListIsEmpty_ShouldAddNewCollection()
        {
            // Arrange
            List<List<int>> lists = new List<List<int>>();
            List<int> newCollection = new List<int>();
            int maxSize = default;

            // Act
            List<int> result = lists.GetWithTheLeastCountOrAdd(maxSize, () => newCollection);

            // Assert
            Assert.Single(lists);
            Assert.Same(newCollection, result);
        }

        [Fact]
        public void GetWithTheLeastCountOrAdd_WhenMultipleCollectionsHaveSameLeastCount_ShouldReturnFirstOne()
        {
            // Arrange
            List<List<int>> lists = new List<List<int>>
            {
                new List<int> { 1 },
                new List<int> { 1 },
                new List<int> { 1, 2 }
            };
            int maxSize = 2;

            // Act
            List<int> result = lists.GetWithTheLeastCountOrAdd(maxSize, () => new List<int>());

            // Assert
            Assert.Same(lists.First(), result); // First one with least count
        }

        [Fact]
        public void GetWithTheLeastCountOrAdd_WhenOnCollectionsFullReturnsNull_ShouldAddNullToTheList()
        {
            // Arrange
            List<List<int>> lists = new List<List<int>>
            {
                new List<int> { 1, 2, 4, 8 }
            };
            int countBeforeInvoking = lists.Count;
            int maxSize = 3;

            // Act
            List<int> result = lists.GetWithTheLeastCountOrAdd(maxSize, () => null!);

            // Assert
            Assert.Null(result);
            Assert.Equal(countBeforeInvoking + 1, lists.Count);
            Assert.Null(lists.Last());
        }
    }
}
