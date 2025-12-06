using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class ConcurrentPoolTests
    {
        [Fact]
        public void NewConcurrentPool_WhenDefaultConstructorUsed_ShouldReturnEmptyInstance()
        {
            // Arrange
            int expectedCount = default;

            // Act
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();

            // Assert
            Assert.Equal(expectedCount, pool.Count);
        }

        [Fact]
        public void Release_WhenInstanceIsNotNull_ShouldIncreaseCount()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            Derived instance = new Derived(5);
            int expectedCount = 1;

            // Act
            pool.Release(instance);

            // Assert
            Assert.Equal(expectedCount, pool.Count);
        }

        [Fact]
        public void Acquire_WhenConcurrentPoolIsNotEmpty_ShouldReturnInstance()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            Derived expected = new Derived(5);

            // Act
            pool.Release(expected);
            Derived instance = pool.Acquire();

            // Assert
            Assert.Equal(expected, instance);
        }

        [Fact]
        public void Acquire_WhenConcurrentPoolIsEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();

            // Act
            Assert.Throws<InvalidOperationException>(pool.Acquire);
        }

        [Fact]
        public void Release_WhenInstanceIsNull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            Derived? instance = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => pool.Release(instance!));
        }

        [Fact]
        public void AcquireAcceptingCallback_WhenConcurrentPoolIsEmpty_ShouldUseCallbackAndReturnNewInstance()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            int expectedInstanceValue = 5;

            // Act
            Derived instance = pool.Acquire(() => new Derived(expectedInstanceValue));

            // Assert
            Assert.Equal(expectedInstanceValue, instance.Value);
        }

        [Fact]
        public void TryAcquire_WhenConcurrentPoolIsNotEmpty_ShouldReturnTrueAndAssignInstance()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            Derived expected = new Derived(5);

            // Act
            pool.Release(expected);
            bool assigned = pool.TryAcquire(out Derived? instance);

            // Assert
            Assert.True(assigned);
            Assert.Equal(expected, instance);
        }

        [Fact]
        public void TryAcquire_WhenConcurrentPoolIsEmpty_ShouldReturnFalseAndAssignDefaultValue()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            Derived? expected = null;

            // Act
            bool assigned = pool.TryAcquire(out Derived? instance);

            // Assert
            Assert.False(assigned);
            Assert.Equal(expected, instance);
        }

        [Fact]
        public void TryRelease_WhenReleasedInstance_ShouldReturnTrue()
        {
            // Arrange
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>();
            Derived instance = new Derived(5);

            // Act
            bool released = pool.TryRelease(instance);

            // Assert
            Assert.True(released);
        }

        [Fact]
        public void Acquire_WhenTypeHasDefaultConstructor_ShouldUseDefaultConstructorAndReturnNewInstance()
        {
            // Arrange
            ConcurrentPool<Base> pool = new ConcurrentPool<Base>();
            int expectedInstanceValue = default;

            // Act
            Base instance = pool.Acquire();

            // Assert
            Assert.Equal(expectedInstanceValue, instance.Value);
        }

        [Fact]
        public void Acquire_WhenProvidedCallbackToCreateNewInstance_ShouldUseOnCreateNewCallbackAndReturnNewInstance()
        {
            // Arrange
            int expectedInstanceValue = 42;
            ConcurrentPool<Derived> pool = new ConcurrentPool<Derived>(() => new Derived(expectedInstanceValue));

            // Act
            Derived instance = pool.Acquire();

            // Assert
            Assert.Equal(expectedInstanceValue, instance.Value);
        }
    }
}
