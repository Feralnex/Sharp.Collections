using CommunityToolkit.HighPerformance;
using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    [CollectionDefinition("Sequential Tests", DisableParallelization = true)]
    [Collection("Sequential Tests")]
    public class PoolsTests : IDisposable
    {
        public void Dispose()
        {
            Pools.RemoveAll<Base>();
            Pools.RemoveAll<Derived>();
        }

        [Fact]
        public void Add_WhenPoolsDoesNotContainPool_ShouldAddPool()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            bool containsPoolWithBaseTypeBeforeAdd = Pools.Contains(pool);

            Pools.Add(pool);

            bool containsPoolWithBaseTypeAfterAdd = Pools.Contains(pool);

            // Assert
            Assert.False(containsPoolWithBaseTypeBeforeAdd);
            Assert.True(containsPoolWithBaseTypeAfterAdd);
        }

        [Fact]
        public void Add_WhenPoolsAlreadyContainsPool_ShouldNotAddPool()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            int countBeforeAdd = 0;
            int countAfterAdd = 0;
            int countAfterAddingAgain = 0;
            bool trySelectorBeforeAdd(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

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
            bool trySelectorAfterAdd(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

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
            bool trySelectorAfterAddingAgain(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

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
            bool retrievedPoolBeforeAdd = Pools.TryGet(trySelectorBeforeAdd, out IPool<Base>? selectedPool);
            Pools.Add(pool);
            bool retrievedPoolAfterAdd = Pools.TryGet(trySelectorAfterAdd, out selectedPool);
            Pools.Add(pool);
            bool retrievedPoolAfterAddingAgain = Pools.TryGet(trySelectorAfterAddingAgain, out selectedPool);

            // Assert
            Assert.False(retrievedPoolBeforeAdd);
            Assert.Equal(0, countBeforeAdd);
            Assert.True(retrievedPoolAfterAdd);
            Assert.Equal(1, countAfterAdd);
            Assert.True(retrievedPoolAfterAddingAgain);
            Assert.Equal(countAfterAdd, countAfterAddingAgain);
        }

        [Fact]
        public void TryAdd_WhenPoolsDoesNotContainPool_ShouldAddPool()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            bool containsPoolWithBaseTypeBeforeAdd = Pools.Contains(pool);
            bool poolAdded = Pools.TryAdd(pool);
            bool containsPoolWithBaseTypeAfterAdd = Pools.Contains(pool);

            // Assert
            Assert.False(containsPoolWithBaseTypeBeforeAdd);
            Assert.True(poolAdded);
            Assert.True(containsPoolWithBaseTypeAfterAdd);
        }

        [Fact]
        public void TryAdd_WhenPoolsAlreadyContainsPool_ShouldReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            int countBeforeAdd = 0;
            int countAfterAdd = 0;
            int countAfterAddingAgain = 0;
            bool trySelectorBeforeTryAdd(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

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
            bool trySelectorAfterTryAdd(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

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
            bool trySelectorAfterTriedAddingAgain(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

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
            bool retrievedPoolBeforeAdd = Pools.TryGet(trySelectorBeforeTryAdd, out IPool<Base>? selectedPool);
            bool poolAdded = Pools.TryAdd(pool);
            bool retrievedPoolAfterAdd = Pools.TryGet(trySelectorAfterTryAdd, out selectedPool);
            bool samePoolAdded = Pools.TryAdd(pool);
            bool retrievedPoolAfterAddingAgain = Pools.TryGet(trySelectorAfterTriedAddingAgain, out selectedPool);

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
        public void Remove_WhenPoolExists_ShouldRemovePool()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool removed = Pools.Remove(pool);

            // Assert
            Assert.True(removed);
            Assert.False(Pools.Contains(pool));
        }

        [Fact]
        public void Remove_WhenPoolDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            bool removed = Pools.Remove(pool);

            // Assert
            Assert.False(removed);
        }
        [Fact]
        public void RemoveAllAcceptingMatch_WhenPoolsContainMatchingPool_ShouldRemoveMatchingPools()
        {
            // Arrange
            Pool<Base> firstPool = new Pool<Base>();
            Pool<Base> secondPool = new Pool<Base>();
            Pools.Add(firstPool);
            Pools.Add(secondPool);

            // Act
            int removedCount = Pools.RemoveAll<Base>(pool => pool == firstPool);

            // Assert
            Assert.Equal(1, removedCount);
            Assert.False(Pools.Contains(firstPool));
            Assert.True(Pools.Contains(secondPool));
        }

        [Fact]
        public void RemoveAllAcceptingMatch_WhenPoolsDoesNotContainMathingPool_ShouldNotRemoveAnything()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            int removedCount = Pools.RemoveAll<Base>(pool => pool.GetType() == typeof(Pool<Derived>));

            // Assert
            Assert.Equal(0, removedCount);
            Assert.True(Pools.Contains(pool));
        }

        [Fact]
        public void RemoveAllAcceptingTypeAndPoolMatch_WhenPoolsContainMatchingPool_ShouldRemovePool()
        {
            // Arrange
            Pool<Base> firstPool = new Pool<Base>();
            Pool<Base> secondPool = new Pool<Base>();
            Pools.Add(firstPool);
            Pools.Add(secondPool);

            // Act
            int removedCount = Pools.RemoveAll(
                type => type == typeof(Base),
                pool => pool == firstPool);

            // Assert
            Assert.Equal(1, removedCount);
            Assert.False(Pools.Contains(firstPool));
            Assert.True(Pools.Contains(secondPool));
        }

        [Fact]
        public void RemoveAllTypeAndPoolMatch_WhenPoolsDoesNotContainMathingPool_ShouldNotRemovePool()
        {
            // Arrange
            Pool<Base> firstPool = new Pool<Base>();
            Pool<Base> secondPool = new Pool<Base>();
            Pools.Add(firstPool);
            Pools.Add(secondPool);

            // Act
            int removedCount = Pools.RemoveAll(
                type => type == typeof(Derived),
                pool => pool == firstPool);

            // Assert
            Assert.Equal(0, removedCount);
            Assert.True(Pools.Contains(firstPool));
            Assert.True(Pools.Contains(secondPool));
        }

        [Fact]
        public void Contains_WhenPoolsDoesNotContainPool_ShouldReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            bool containsPoolWithBaseType = Pools.Contains(pool);

            // Assert
            Assert.False(containsPoolWithBaseType);
        }

        [Fact]
        public void Contains_WhenPoolsContainPool_ShouldReturnTrue()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            Pools.Add(pool);

            bool containsPoolWithBaseType = Pools.Contains(pool);

            // Assert
            Assert.True(containsPoolWithBaseType);
        }

        [Fact]
        public void Any_WhenPoolsDoesNotContainPoolWithDerivedType_ShouldReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            Pools.Add(pool);

            bool containsAnyPoolWithBaseType = Pools.Any<Derived>();

            // Assert
            Assert.False(containsAnyPoolWithBaseType);
        }

        [Fact]
        public void Any_WhenPoolsContainPoolWithBaseType_ShouldReturnTrue()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            Pools.Add(pool);

            bool containsAnyPoolWithBaseType = Pools.Any<Base>();

            // Assert
            Assert.True(containsAnyPoolWithBaseType);
        }

        [Fact]
        public void AnyAcceptingMatch_WhenPoolsContainPoolWithDerivedType_ShouldReturnFalse()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();

            // Act
            Pools.Add(pool);

            bool containsAnyPoolWithTypeAssignableFromBaseType = Pools.Any(type => type.IsAssignableFrom(typeof(Base)));

            // Assert
            Assert.False(containsAnyPoolWithTypeAssignableFromBaseType);
        }

        [Fact]
        public void AnyAcceptingMatch_WhenPoolsContainPoolWithBaseType_ShouldReturnTrue()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();

            // Act
            Pools.Add(pool);

            bool containsAnyPoolWithTypeAssignableFromDerivedType = Pools.Any(type => type.IsAssignableFrom(typeof(Derived)));

            // Assert
            Assert.True(containsAnyPoolWithTypeAssignableFromDerivedType);
        }

        [Fact]
        public void GetOrAdd_WhenPoolAlreadyExists_ShouldReturnExistingPool()
        {
            // Arrange
            Pool<Base> existingPool = new Pool<Base>();
            IPool<Base> onPoolMissing() => new Pool<Base>();

            // Act
            Pools.Add(existingPool);

            IPool<Base> result = Pools.GetOrAdd(onPoolMissing);

            // Assert
            Assert.Same(existingPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenPoolDoesNotExist_ShouldCreateAndReturnNewPool()
        {
            // Arrange
            Pool<Base> createdPool = new Pool<Base>();
            IPool<Base> onPoolMissing() => createdPool;

            // Act
            IPool<Base> result = Pools.GetOrAdd(onPoolMissing);

            // Assert
            Assert.True(Pools.Contains(createdPool));
            Assert.Same(createdPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenInvokedMoreThanOnce_ShouldReturnSameInstance()
        {
            // Arrange
            IPool<Base> createdPool = new Pool<Base>();
            IPool<Base> alternativePool = new Pool<Base>();

            // Act
            IPool<Base> first = Pools.GetOrAdd(() => createdPool);
            IPool<Base> second = Pools.GetOrAdd(() => alternativePool);

            // Assert
            Assert.Same(createdPool, first);
            Assert.Same(createdPool, second);
            Assert.Same(first, second);
        }

        [Fact]
        public void GetOrAdd_WhenPoolsContainMatchingPool_ShouldReturnExistingPool()
        {
            // Arrange
            IPool<Base> createdPool = new Pool<Base>();
            IPool<Base> alternativePool = new Pool<Base>();
            IPool<Base> onPoolMissing() => alternativePool;
            bool trySelector(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == createdPool)
                        return true;
                }

                return false;
            }

            // Act
            Pools.Add(createdPool);

            IPool<Base> result = Pools.GetOrAdd(trySelector, onPoolMissing);

            // Assert
            Assert.True(Pools.Contains(createdPool));
            Assert.False(Pools.Contains(alternativePool));
            Assert.Same(createdPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenPoolsDoesNotContainMatchingPool_ShouldCreateAndReturnNewPool()
        {
            // Arrange
            IPool<Base> createdPool = new Pool<Base>();
            IPool<Base> onPoolMissing() => createdPool;
            bool trySelector(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == createdPool)
                        return true;
                }

                return false;
            }

            // Act
            IPool<Base> result = Pools.GetOrAdd(trySelector, onPoolMissing);

            // Assert
            Assert.True(Pools.Contains(createdPool));
            Assert.Same(createdPool, result);
        }

        [Fact]
        public void GetOrAdd_WhenInvokedMoreThanOnceAndPoolsContainMatchingPool_ShouldReturnExistingPool()
        {
            // Arrange
            IPool<Base> createdPool = new Pool<Base>();
            IPool<Base> alternativePool = new Pool<Base>();
            IPool<Base> onPoolMissing() => alternativePool;
            bool trySelector(ReadOnlySpan<IPool<Base>> span, out IPool<Base> selectedPool)
            {
                selectedPool = default(Pool<Base>)!;

                for (int index = 0; index < span.Length; index++)
                {
                    selectedPool = span.DangerousGetReferenceAt(index);

                    if (selectedPool == createdPool)
                        return true;
                }

                return false;
            }

            // Act
            Pools.Add(createdPool);

            IPool<Base> first = Pools.GetOrAdd(trySelector, onPoolMissing);
            IPool<Base> second = Pools.GetOrAdd(trySelector, onPoolMissing);

            // Assert
            Assert.Same(createdPool, first);
            Assert.Same(createdPool, second);
            Assert.Same(first, second);
        }

        [Fact]
        public void TryGet_WhenPoolsDoesNotContainPool_ShouldAssignDefaultValueAndReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool result = Pools.TryGet(out IPool<Derived>? retrievedPool);

            // Assert
            Assert.False(result);
            Assert.Null(retrievedPool);
        }

        [Fact]
        public void TryGet_WhenPoolsContainPool_ShouldAssignPoolAndReturnTrue()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool result = Pools.TryGet(out IPool<Base>? retrievedPool);

            // Assert
            Assert.True(result);
            Assert.Equal(pool, retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingMatch_WhenPoolsDoesNotContainMatchingPool_ShouldAssignDefaultValueAndReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool result = Pools.TryGet(type => type == typeof(Derived), out IPool? retrievedPool);

            // Assert
            Assert.False(result);
            Assert.Null(retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingMatch_WhenPoolsContainMatchingPool_ShouldAssignPoolAndReturnTrue()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool result = Pools.TryGet(type => type == typeof(Base), out IPool? retrievedPool);

            // Assert
            Assert.True(result);
            Assert.Equal(pool, retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingTypeAndPoolMatch_WhenPoolsDoesNotContainMatchingPool_ShouldReturnFalse()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool result = Pools.TryGet(
                type => type == typeof(Derived),
                containedPool => containedPool == pool,
                out IPool? retrievedPool);

            // Assert
            Assert.False(result);
            Assert.Null(retrievedPool);
        }

        [Fact]
        public void TryGetAcceptingTypeAndPoolMatch_WhenPoolsContainMatchingPool_ShouldReturnPool()
        {
            // Arrange
            Pool<Base> pool = new Pool<Base>();
            Pools.Add(pool);

            // Act
            bool result = Pools.TryGet(
                type => type == typeof(Base),
                containedPool => containedPool == pool,
                out IPool? retrievedPool);

            // Assert
            Assert.True(result);
            Assert.Equal(pool, retrievedPool);
        }
    }
}
