using System;
using System.Collections.Generic;
using Sharp.Collections.Extensions;
using Xunit;

namespace Sharp.Collections.Tests.Extensions
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void GetOrAddAcceptingKey_WhenKeyExists_ShouldReturnExistingValue()
        {
            // Arrange
            var dictionary = new Dictionary<int, List<int>>
            {
                { 1, [1, 2, 3] }
            };
            int key = 1;

            // Act
            var result = dictionary.GetOrAdd(key);

            // Assert
            Assert.Equal([1, 2, 3], result);
        }

        [Fact]
        public void GetOrAddAcceptingKey_WhenKeyDoesNotExist_ShouldAddDefaultValueToDictionary()
        {
            // Arrange
            var dictionary = new Dictionary<int, Base>();
            int key = 3;

            // Act
            var result = dictionary.GetOrAdd(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(default, result.Value);
            Assert.True(dictionary.ContainsKey(key));
        }

        [Fact]
        public void GetOrAddAcceptingKey_WhenDictionaryIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Dictionary<int, Base>? dictionary = null;
            int key = 1;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                dictionary!.GetOrAdd(key)
            );
        }

        [Fact]
        public void GetOrAddAcceptingKeyAndCallback_WhenKeyExists_ShouldReturnExistingValue()
        {
            // Arrange
            var dictionary = new Dictionary<int, string>
            {
                { 1, 1.ToString() },
                { 2, 2.ToString() }
            };
            int key = 1;

            // Act
            var result = dictionary.GetOrAdd(key, 3.ToString);

            // Assert
            Assert.Equal(1.ToString(), result);
        }

        [Fact]
        public void GetOrAddAcceptingKeyAndCallback_WhenKeyDoesNotExist_ShouldCallOnMissingKeyCallbackAndReturnNewValue()
        {
            // Arrange
            var dictionary = new Dictionary<int, string>();
            int key = 3;
            bool callbackCalled = false;

            // Act
            var result = dictionary.GetOrAdd(key, () =>
            {
                callbackCalled = true;
                return 3.ToString();
            });

            // Assert
            Assert.True(callbackCalled);
            Assert.Equal(3.ToString(), result);
            Assert.Equal(3.ToString(), dictionary[key]);
        }

        [Fact]
        public void GetOrAddAcceptingKeyAndCallback_WhenOnMissingKeyIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var dictionary = new Dictionary<int, string>();
            int key = 1;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                dictionary.GetOrAdd(key, null!)
            );
        }

        [Fact]
        public void GetOrAddAcceptingKeyAndCallback_WhenDictionaryIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Dictionary<int, string> dictionary = null!;
            int key = 1;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                dictionary.GetOrAdd(key, 2.ToString)
            );
        }

        [Fact]
        public void SetAndGetBuckets_WhenBucketsHaveBeenChanged_ShouldBeReflectedInGetBuckets()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int[] expectedBuckets = [1, 2, 3];

            // Act
            dictionary.SetBuckets(expectedBuckets);
            int[]? actualBuckets = dictionary.GetBuckets();

            // Assert
            Assert.Equal(expectedBuckets, actualBuckets);
        }

        [Fact]
        public void SetAndGetEntries_WhenEntriesHaveBeenChanged_ShouldBeReflectedInGetEntries()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            DictionaryExtensions.Entry<string, int>[] expectedEntries =
            [
                new DictionaryExtensions.Entry<string, int> { hashCode = 1, key = 1.ToString(), value = 10, next = -1 }
            ];

            // Act
            dictionary.SetEntries(expectedEntries);
            DictionaryExtensions.Entry<string, int>[]? actualEntries = dictionary.GetEntries();

            // Assert
            Assert.NotNull(actualEntries);
            Assert.Single(actualEntries!);
            Assert.Equal(1.ToString(), actualEntries[0].key);
            Assert.Equal(10, actualEntries[0].value);
        }

        [Fact]
        public void TrySetAndGetFastModMultiplier_WhenFastModMultiplierHasBeenChanged_ShouldBeReflectedInTryGetFastModMultiplier()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            ulong testMultiplier = 987654321;

            // Act
            if (Environment.Is64BitProcess)
            {
                // x64: Supported
                bool setResult = dictionary.TrySetFastModMultiplier(testMultiplier);
                bool getResult = dictionary.TryGetFastModMultiplier(out ulong actualMultiplier);

                // Assert
                Assert.True(setResult);
                Assert.True(getResult);
                Assert.Equal(testMultiplier, actualMultiplier);
            }
            else
            {
                // x86: Not supported
                bool setResult = dictionary.TrySetFastModMultiplier(testMultiplier);
                bool getResult = dictionary.TryGetFastModMultiplier(out ulong actualMultiplier);

                // Assert
                Assert.False(setResult);
                Assert.False(getResult);
                Assert.Equal(default, actualMultiplier);
            }
        }

        [Fact]
        public void SetAndGetCount_WhenCountHasBeenChanged_ShouldBeReflectedInGetCount()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int expectedCount = 5;

            // Act
            dictionary.SetCount(expectedCount);
            int actualCount = dictionary.GetCount();

            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void SetAndGetFreeCount_WhenFreeCountHasBeenChanged_ShouldBeReflectedInGetFreeCount()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int expectedFreeCount = 3;

            // Act
            dictionary.SetFreeCount(expectedFreeCount);
            int actualFreeCount = dictionary.GetFreeCount();

            // Assert
            Assert.Equal(expectedFreeCount, actualFreeCount);
        }

        [Fact]
        public void SetAndGetFreeList_WhenFreeListHasBeenChanged_ShouldBeReflectedInGetFreeList()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int expectedFreeList = 2;

            // Act
            dictionary.SetFreeList(expectedFreeList);
            int actualFreeList = dictionary.GetFreeList();

            // Assert
            Assert.Equal(expectedFreeList, actualFreeList);
        }

        [Fact]
        public void SetAndGetVersion_WhenVersionHasBeenChanged_ShouldBeReflectedInGetVersion()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int expectedVersion = 42;

            // Act
            dictionary.SetVersion(expectedVersion);
            int actualVersion = dictionary.GetVersion();

            // Assert
            Assert.Equal(expectedVersion, actualVersion);
        }

        [Fact]
        public void SetAndGetComparer_WhenComparerHasBeenChanged_ShouldBeReflectedInGetComparer()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            StringComparer expectedComparer = StringComparer.OrdinalIgnoreCase;

            // Act
            dictionary.SetComparer(expectedComparer);
            IEqualityComparer<string>? actualComparer = dictionary.GetComparer();

            // Assert
            Assert.Same(expectedComparer, actualComparer);
        }

        [Fact]
        public void SetAndGetKeys_WhenKeysHaveBeenChanged_ShouldBeReflectedInGetKeys()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            Dictionary<string, int>.KeyCollection expectedKeys = new Dictionary<string, int>.KeyCollection(dictionary);

            // Act
            dictionary.SetKeys(expectedKeys);
            Dictionary<string, int>.KeyCollection? actualKeys = dictionary.GetKeys();

            // Assert
            Assert.Same(expectedKeys, actualKeys);
        }

        [Fact]
        public void SetAndGetValues_WhenValuesHaveBeenChanged_ShouldBeReflectedInGetValues()
        {
            // Arrange
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            Dictionary<string, int>.ValueCollection expectedValues = new Dictionary<string, int>.ValueCollection(dictionary);

            // Act
            dictionary.SetValues(expectedValues);
            Dictionary<string, int>.ValueCollection? actualValues = dictionary.GetValues();

            // Assert
            Assert.Same(expectedValues, actualValues);
        }
    }
}
