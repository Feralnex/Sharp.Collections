using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class PoolTests
    {
        [Fact]
        public void NewPool_WhenDefaultConstructorUsed_ShouldReturnEmptyInstance()
        {
            // Arrange
            int expectedCount = 0;

            // Act
            Pool<Derived> pool = new Pool<Derived>();

            // Assert
            Assert.Equal(expectedCount, pool.Count);
        }

        [Fact]
        public void Release_WhenInstanceIsNotNull_ShouldIncreaseCount()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();
            Derived instance = new Derived(5);
            int expectedCount = 1;

            // Act
            pool.Release(instance);

            // Assert
            Assert.Equal(expectedCount, pool.Count);
        }

        [Fact]
        public void Acquire_WhenPoolIsNotEmpty_ShouldReturnInstance()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();
            Derived expected = new Derived(5);
            int expectedCount = 0;

            // Act
            pool.Release(expected);
            Derived instance = pool.Acquire();

            // Assert
            Assert.Equal(expected, instance);
            Assert.Equal(expectedCount, pool.Count);
        }

        [Fact]
        public void Acquire_WhenPoolIsEmpty_ShouldThrowInvalidOperationException()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();

            // Act
            Assert.Throws<InvalidOperationException>(pool.Acquire);
        }

        [Fact]
        public void Release_WhenInstanceIsNull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();
            Derived? instance = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => pool.Release(instance!));
        }

        [Fact]
        public void AcquireAcceptingCallback_WhenPoolIsEmpty_ShouldUseCallbackAndReturnNewInstance()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();
            int expectedInstanceValue = 5;

            // Act
            Derived instance = pool.Acquire(() => new Derived(expectedInstanceValue));

            // Assert
            Assert.Equal(expectedInstanceValue, instance.Value);
        }

        [Fact]
        public void TryAcquire_WhenPoolIsNotEmpty_ShouldReturnTrueAndAssignInstance()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();
            Derived expected = new Derived(5);

            // Act
            pool.Release(expected);
            bool assigned = pool.TryAcquire(out Derived? instance);

            // Assert
            Assert.True(assigned);
            Assert.Equal(expected, instance);
        }

        [Fact]
        public void TryAcquire_WhenPoolIsEmpty_ShouldReturnFalseAndAssignDefaultValue()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();

            // Act
            bool assigned = pool.TryAcquire(out Derived? instance);

            // Assert
            Assert.False(assigned);
            Assert.Equal(default, instance);
        }

        [Fact]
        public void TryRelease_WhenReleasedInstance_ShouldReturnTrue()
        {
            // Arrange
            Pool<Derived> pool = new Pool<Derived>();
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
            Pool<Base> pool = new Pool<Base>();
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
            Pool<Derived> pool = new Pool<Derived>(() => new Derived(expectedInstanceValue));

            // Act
            Derived instance = pool.Acquire();

            // Assert
            Assert.Equal(expectedInstanceValue, instance.Value);
        }
    }
}
