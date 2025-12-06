using CommunityToolkit.HighPerformance;
using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    [CollectionDefinition("Sequential Tests", DisableParallelization = true)]
    [Collection("Sequential Tests")]
    public class KeyedPoolsTests : IDisposable
    {
        public void Dispose()
        {
            KeyedPools.RemoveAll<int, byte[]>();
        }

        [Fact]
        public void Add_WhenKeyedPoolsDoesNotContainKeyedPool_ShouldAddKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            bool containsPoolWithBaseTypeBeforeAdd = KeyedPools.Contains(pool);

            KeyedPools.Add(pool);

            bool containsPoolWithBaseTypeAfterAdd = KeyedPools.Contains(pool);

            // Assert
            Assert.False(containsPoolWithBaseTypeBeforeAdd);
            Assert.True(containsPoolWithBaseTypeAfterAdd);
        }

        [Fact]
        public void Add_WhenKeyedPoolsAlreadyContainsKeyedPool_ShouldNotAddKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            int countBeforeAdd = 0;
            int countAfterAdd = 0;
            int countAfterAddingAgain = 0;
            bool trySelectorBeforeAdd(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == pool)
                    {
                        countBeforeAdd++;

                        return true;
                    }
                }

                return false;
            }
            bool trySelectorAfterAdd(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == pool)
                    {
                        countAfterAdd++;

                        return true;
                    }
                }

                return false;
            }
            bool trySelectorAfterAddingAgain(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == pool)
                    {
                        countAfterAddingAgain++;

                        return true;
                    }
                }

                return false;
            }

            // Act
            bool retrievedPoolBeforeAdd = KeyedPools.TryGet(trySelectorBeforeAdd, out IKeyedPool<int, byte[]>? selectedPool);
            KeyedPools.Add(pool);
            bool retrievedPoolAfterAdd = KeyedPools.TryGet(trySelectorAfterAdd, out selectedPool);
            KeyedPools.Add(pool);
            bool retrievedPoolAfterAddingAgain = KeyedPools.TryGet(trySelectorAfterAddingAgain, out selectedPool);

            // Assert
            Assert.False(retrievedPoolBeforeAdd);
            Assert.Equal(0, countBeforeAdd);
            Assert.True(retrievedPoolAfterAdd);
            Assert.Equal(1, countAfterAdd);
            Assert.True(retrievedPoolAfterAddingAgain);
            Assert.Equal(countAfterAdd, countAfterAddingAgain);
        }

        [Fact]
        public void TryAdd_WhenKeyedPoolsDoesNotContainKeyedPool_ShouldAddKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            bool containsPoolWithBaseTypeBeforeAdd = KeyedPools.Contains(pool);
            bool poolAdded = KeyedPools.TryAdd(pool);
            bool containsPoolWithBaseTypeAfterAdd = KeyedPools.Contains(pool);

            // Assert
            Assert.False(containsPoolWithBaseTypeBeforeAdd);
            Assert.True(poolAdded);
            Assert.True(containsPoolWithBaseTypeAfterAdd);
        }

        [Fact]
        public void TryAdd_WhenKeyedPoolsAlreadyContainsKeyedPool_ShouldReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            int countBeforeAdd = 0;
            int countAfterAdd = 0;
            int countAfterAddingAgain = 0;
            bool trySelectorBeforeTryAdd(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == pool)
                    {
                        countBeforeAdd++;

                        return true;
                    }
                }

                return false;
            }
            bool trySelectorAfterTryAdd(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == pool)
                    {
                        countAfterAdd++;

                        return true;
                    }
                }

                return false;
            }
            bool trySelectorAfterTriedAddingAgain(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == pool)
                    {
                        countAfterAddingAgain++;

                        return true;
                    }
                }

                return false;
            }

            // Act
            bool retrievedPoolBeforeAdd = KeyedPools.TryGet(trySelectorBeforeTryAdd, out IKeyedPool<int, byte[]>? selectedPool);
            bool poolAdded = KeyedPools.TryAdd(pool);
            bool retrievedPoolAfterAdd = KeyedPools.TryGet(trySelectorAfterTryAdd, out selectedPool);
            bool samePoolAdded = KeyedPools.TryAdd(pool);
            bool retrievedPoolAfterAddingAgain = KeyedPools.TryGet(trySelectorAfterTriedAddingAgain, out selectedPool);

            // Assert
            Assert.False(retrievedPoolBeforeAdd);
            Assert.Equal(0, countBeforeAdd);
            Assert.True(poolAdded);
            Assert.True(retrievedPoolAfterAdd);
            Assert.Equal(1, countAfterAdd);
            Assert.False(samePoolAdded);
            Assert.True(retrievedPoolAfterAddingAgain);
            Assert.Equal(countAfterAdd, countAfterAddingAgain);
        }

        [Fact]
        public void Remove_WhenKeyedPoolExists_ShouldRemovePool()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool removed = KeyedPools.Remove(pool);

            // Assert
            Assert.True(removed);
            Assert.False(KeyedPools.Contains(pool));
        }

        [Fact]
        public void Remove_WhenKeyedPoolDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            bool removed = KeyedPools.Remove(pool);

            // Assert
            Assert.False(removed);
        }
        [Fact]
        public void RemoveAllAcceptingMatch_WhenKeyedPoolsContainMatchingKeyedPool_ShouldRemoveMatchingKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> firstPool = new KeyedPool<int, byte[]>();
            KeyedPool<int, byte[]> secondPool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(firstPool);
            KeyedPools.Add(secondPool);

            // Act
            int removedCount = KeyedPools.RemoveAll<int, byte[]>(pool => pool == firstPool);

            // Assert
            Assert.Equal(1, removedCount);
            Assert.False(KeyedPools.Contains(firstPool));
            Assert.True(KeyedPools.Contains(secondPool));
        }

        [Fact]
        public void RemoveAllAcceptingMatch_WhenKeyedPoolsDoesNotContainMathingKeyedPool_ShouldNotRemoveAnything()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            int removedCount = KeyedPools.RemoveAll<int, byte[]>(pool => pool.GetType() == typeof(KeyedPool<int, short[]>));

            // Assert
            Assert.Equal(0, removedCount);
            Assert.True(KeyedPools.Contains(pool));
        }

        [Fact]
        public void RemoveAllAcceptingTypeAndPoolMatch_WhenKeyedPoolsContainMatchingKeyedPool_ShouldRemoveKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> firstPool = new KeyedPool<int, byte[]>();
            KeyedPool<int, byte[]> secondPool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(firstPool);
            KeyedPools.Add(secondPool);

            // Act
            int removedCount = KeyedPools.RemoveAll(
                handle => handle.ElementType == typeof(byte[]),
                pool => pool == firstPool);

            // Assert
            Assert.Equal(1, removedCount);
            Assert.False(KeyedPools.Contains(firstPool));
            Assert.True(KeyedPools.Contains(secondPool));
        }

        [Fact]
        public void RemoveAllTypeAndPoolMatch_WhenKeyedPoolsDoesNotContainMathingKeyedPool_ShouldNotRemoveKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> firstPool = new KeyedPool<int, byte[]>();
            KeyedPool<int, byte[]> secondPool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(firstPool);
            KeyedPools.Add(secondPool);

            // Act
            int removedCount = KeyedPools.RemoveAll(
                handle => handle.ElementType == typeof(short[]),
                pool => pool == firstPool);

            // Assert
            Assert.Equal(0, removedCount);
            Assert.True(KeyedPools.Contains(firstPool));
            Assert.True(KeyedPools.Contains(secondPool));
        }

        [Fact]
        public void Contains_WhenKeyedPoolsDoesNotContainKeyedPool_ShouldReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            bool containsPoolWithBaseType = KeyedPools.Contains(pool);

            // Assert
            Assert.False(containsPoolWithBaseType);
        }

        [Fact]
        public void Contains_WhenKeyedPoolsContainKeyedPool_ShouldReturnTrue()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            KeyedPools.Add(pool);

            bool containsPoolWithBaseType = KeyedPools.Contains(pool);

            // Assert
            Assert.True(containsPoolWithBaseType);
        }

        [Fact]
        public void Any_WhenKeyedPoolsDoesNotContainKeyedPoolWithDerivedType_ShouldReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            KeyedPools.Add(pool);

            bool containsAnyPoolWithBaseType = KeyedPools.Any<int, short[]>();

            // Assert
            Assert.False(containsAnyPoolWithBaseType);
        }

        [Fact]
        public void Any_WhenKeyedPoolsContainKeyedPoolWithBaseType_ShouldReturnTrue()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            KeyedPools.Add(pool);

            bool containsAnyPoolWithBaseType = KeyedPools.Any<int, byte[]>();

            // Assert
            Assert.True(containsAnyPoolWithBaseType);
        }

        [Fact]
        public void AnyAcceptingMatch_WhenKeyedPoolsContainKeyedPoolWithByteArrayType_ShouldReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            KeyedPools.Add(pool);

            bool containsAnyPoolWithTypeAssignableFromBaseType = KeyedPools.Any(handle => handle.ElementType == typeof(short[]));

            // Assert
            Assert.False(containsAnyPoolWithTypeAssignableFromBaseType);
        }

        [Fact]
        public void AnyAcceptingMatch_WhenKeyedPoolsContainKeyedPoolWithByteArrayType_ShouldReturnTrue()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            // Act
            KeyedPools.Add(pool);

            bool containsAnyPoolWithTypeAssignableFromDerivedType = KeyedPools.Any(handle => handle.ElementType == typeof(byte[]));

            // Assert
            Assert.True(containsAnyPoolWithTypeAssignableFromDerivedType);
        }

        [Fact]
        public void GetOrAdd_WhenKeyedPoolAlreadyExists_ShouldReturnExistingPool()
        {
            // Arrange
            KeyedPool<int, byte[]> existingPool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> onPoolMissing() => new KeyedPool<int, byte[]>();

            // Act
            KeyedPools.Add(existingPool);

            IKeyedPool<int, byte[]> result = KeyedPools.GetOrAdd(onPoolMissing);

            // Assert
            Assert.Same(existingPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenKeyedPoolDoesNotExist_ShouldCreateAndReturnNewPool()
        {
            // Arrange
            KeyedPool<int, byte[]> createdPool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> onPoolMissing() => createdPool;

            // Act
            IKeyedPool<int, byte[]> result = KeyedPools.GetOrAdd(onPoolMissing);

            // Assert
            Assert.True(KeyedPools.Contains(createdPool));
            Assert.Same(createdPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenInvokedMoreThanOnce_ShouldReturnSameInstance()
        {
            // Arrange
            IKeyedPool<int, byte[]> createdPool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> alternativePool = new KeyedPool<int, byte[]>();

            // Act
            IKeyedPool<int, byte[]> first = KeyedPools.GetOrAdd(() => createdPool);
            IKeyedPool<int, byte[]> second = KeyedPools.GetOrAdd(() => alternativePool);

            // Assert
            Assert.Same(createdPool, first);
            Assert.Same(createdPool, second);
            Assert.Same(first, second);
        }

        [Fact]
        public void GetOrAdd_WhenKeyedPoolsContainMatchingKeyedPool_ShouldReturnExistingKeyedPool()
        {
            // Arrange
            IKeyedPool<int, byte[]> createdPool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> alternativePool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> onPoolMissing() => alternativePool;
            bool trySelector(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == createdPool)
                        return true;
                }

                return false;
            }

            // Act
            KeyedPools.Add(createdPool);

            IKeyedPool<int, byte[]> result = KeyedPools.GetOrAdd(trySelector, onPoolMissing);

            // Assert
            Assert.True(KeyedPools.Contains(createdPool));
            Assert.False(KeyedPools.Contains(alternativePool));
            Assert.Same(createdPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenKeyedPoolsDoesNotContainMatchingKeyedPool_ShouldCreateAndReturnNewKeyedPool()
        {
            // Arrange
            KeyedPool<int, byte[]> createdPool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> onPoolMissing() => createdPool;
            bool trySelector(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == createdPool)
                        return true;
                }

                return false;
            }

            // Act
            IKeyedPool<int, byte[]> result = KeyedPools.GetOrAdd(trySelector, onPoolMissing);

            // Assert
            Assert.True(KeyedPools.Contains(createdPool));
            Assert.Same(createdPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenInvokedMoreThanOnceAndKeyedPoolsContainMatchingKeyedPool_ShouldReturnExistingKeyedPool()
        {
            // Arrange
            IKeyedPool<int, byte[]> createdPool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> alternativePool = new KeyedPool<int, byte[]>();
            IKeyedPool<int, byte[]> onPoolMissing() => alternativePool;
            bool trySelector(ReadOnlySpan<IKeyedPool<int, byte[]>> span, out IKeyedPool<int, byte[]> selectedPool)
            {
                selectedPool = default(KeyedPool<int, byte[]>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == createdPool)
                        return true;
                }

                return false;
            }

            // Act
            KeyedPools.Add(createdPool);

            IKeyedPool<int, byte[]> first = KeyedPools.GetOrAdd(trySelector, onPoolMissing);
            IKeyedPool<int, byte[]> second = KeyedPools.GetOrAdd(trySelector, onPoolMissing);

            // Assert
            Assert.Same(createdPool, first);
            Assert.Same(createdPool, second);
            Assert.Same(first, second);
        }

        [Fact]
        public void TryGet_WhenKeyedPoolsDoesNotContainKeyedPool_ShouldAssignDefaultValueAndReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool result = KeyedPools.TryGet(out IKeyedPool<int, short[]>? retrievedPool);

            // Assert
            Assert.False(result);
            Assert.Null(retrievedPool);
        }

        [Fact]
        public void TryGet_WhenKeyedPoolsContainKeyedPool_ShouldAssignKeyedPoolAndReturnTrue()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool result = KeyedPools.TryGet(out IKeyedPool<int, byte[]>? retrievedPool);

            // Assert
            Assert.True(result);
            Assert.Equal(pool, retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingMatch_WhenKeyedPoolsDoesNotContainMatchingKeyedPool_ShouldAssignDefaultValueAndReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool result = KeyedPools.TryGet(handle => handle.ElementType == typeof(short[]), out IKeyedPool? retrievedPool);

            // Assert
            Assert.False(result);
            Assert.Null(retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingMatch_WhenKeyedPoolsContainMatchingKeyedPool_ShouldAssignKeyedPoolAndReturnTrue()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool result = KeyedPools.TryGet(handle => handle.ElementType == typeof(byte[]), out IKeyedPool? retrievedPool);

            // Assert
            Assert.True(result);
            Assert.Equal(pool, retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingTypeAndKeyedPoolMatch_WhenKeyedPoolsDoesNotContainMatchingKeyedPool_ShouldReturnFalse()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool result = KeyedPools.TryGet(
                handle => handle.ElementType == typeof(short[]),
                containedPool => containedPool == pool,
                out IKeyedPool? retrievedPool);

            // Assert
            Assert.False(result);
            Assert.Null(retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingTypeAndKeyedPoolMatch_WhenKeyedPoolsContainMatchingKeyedPool_ShouldReturnPool()
        {
            // Arrange
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            KeyedPools.Add(pool);

            // Act
            bool result = KeyedPools.TryGet(
                handle => handle.ElementType == typeof(byte[]),
                containedPool => containedPool == pool,
                out IKeyedPool? retrievedPool);

            // Assert
            Assert.True(result);
            Assert.Equal(pool, retrievedPool);
        }
    }
}
