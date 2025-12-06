using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class KeyedPoolTests
    {
        [Fact]
        public void NewKeyedPool_WhenDefaultConstructorUsed_ShouldReturnEmptyInstance()
        {
            int size = sizeof(int);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            int count = pool.Count(size);

            Assert.Equal(0, count);
        }

        [Fact]
        public void Release_WhenArrayIsNotNull_ShouldIncreaseCountForKey()
        {
            int size = sizeof(long);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            byte[] buffer = new byte[size];

            pool.Release(size, buffer);

            Assert.Equal(1, pool.Count(size));
        }

        [Fact]
        public void Acquire_WhenPoolForKeyIsNotEmpty_ShouldReturnSameArray()
        {
            int size = sizeof(double);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            byte[] expected = new byte[size];
            pool.Release(size, expected);

            byte[] instance = pool.Acquire(size);

            Assert.Same(expected, instance);
            Assert.Equal(0, pool.Count(size));
        }

        [Fact]
        public void Acquire_WhenTypeHasDefaultConstructor_ShouldReturnNewInstanceFromDefaultConstructor()
        {
            // Arrange
            int size = sizeof(int);
            KeyedPool<int, Base> pool = new KeyedPool<int, Base>();

            // Act
            Base instance = pool.Acquire(size);

            // Assert
            Assert.NotNull(instance);
            Assert.Equal(default, instance.Value); // default ctor sets Value = 0
        }

        [Fact]
        public void Acquire_WhenPoolForKeyIsEmptyAndNoCreator_ShouldThrowInvalidOperationException()
        {
            int size = sizeof(decimal);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            Assert.Throws<InvalidOperationException>(() => pool.Acquire(size));
        }

        [Fact]
        public void Acquire_WhenProvidedCallback_ShouldUseCallbackAndReturnNewArray()
        {
            int size = sizeof(short);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            byte[] buffer = pool.Acquire(size, length => new byte[length]);

            Assert.Equal(size, buffer.Length);
        }

        [Fact]
        public void Acquire_WhenPoolHasOnCreateNew_ShouldUseOnCreateNewAndReturnNewArray()
        {
            int size = sizeof(ushort);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>(length => new byte[length]);

            byte[] buffer = pool.Acquire(size);

            Assert.Equal(size, buffer.Length);
        }

        [Fact]
        public void TryAcquire_WhenPoolForKeyIsNotEmpty_ShouldReturnTrueAndAssignArray()
        {
            int size = sizeof(uint);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            byte[] expected = new byte[size];
            pool.Release(size, expected);

            bool acquired = pool.TryAcquire(size, out byte[]? buffer);

            Assert.True(acquired);
            Assert.Same(expected, buffer);
        }

        [Fact]
        public void TryAcquire_WhenPoolForKeyIsEmptyAndNoCreator_ShouldReturnFalse()
        {
            int size = sizeof(ulong);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            bool acquired = pool.TryAcquire(size, out byte[]? buffer);

            Assert.False(acquired);
            Assert.Null(buffer);
        }

        [Fact]
        public void TryAcquire_WhenPoolForKeyIsEmptyAndHasOnCreateNew_ShouldReturnTrueAndNewArray()
        {
            int size = sizeof(float);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>(length => new byte[length]);

            bool acquired = pool.TryAcquire(size, out byte[]? buffer);

            Assert.True(acquired);
            Assert.NotNull(buffer);
            Assert.Equal(size, buffer!.Length);
        }

        [Fact]
        public void TryAcquire_WithOverride_ShouldAlwaysReturnTrueAndUseOverride()
        {
            int size = sizeof(double);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();

            bool acquired = pool.TryAcquire(size, length => new byte[length], out byte[]? buffer);

            Assert.True(acquired);
            Assert.Equal(size, buffer!.Length);
        }

        [Fact]
        public void TryRelease_WhenReleasedArray_ShouldReturnTrue()
        {
            int size = sizeof(decimal);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            byte[] buffer = new byte[size];

            bool released = pool.TryRelease(size, buffer);

            Assert.True(released);
            Assert.Equal(1, pool.Count(size));
        }

        [Fact]
        public void ReleaseAndAcquire_WithDifferentSizes_ShouldKeepBucketsIsolated()
        {
            int smallSize = sizeof(short);
            int largeSize = sizeof(long);
            KeyedPool<int, byte[]> pool = new KeyedPool<int, byte[]>();
            byte[] small = new byte[smallSize];
            byte[] large = new byte[largeSize];

            pool.Release(smallSize, small);
            pool.Release(largeSize, large);

            byte[] acquiredSmall = pool.Acquire(smallSize);
            byte[] acquiredLarge = pool.Acquire(largeSize);

            Assert.Same(small, acquiredSmall);
            Assert.Same(large, acquiredLarge);
            Assert.Equal(0, pool.Count(smallSize));
            Assert.Equal(0, pool.Count(largeSize));
        }

        [Fact]
        public void IKeyedPool_AcquireAndRelease_ShouldWorkThroughInterface()
        {
            int size = sizeof(int);
            IKeyedPool pool = new KeyedPool<int, byte[]>(length => new byte[length]);
            byte[] buffer = new byte[size];

            pool.Release(size, buffer);
            object acquired = pool.Acquire(size);

            Assert.Same(buffer, acquired);
            Assert.Equal(0, pool.Count(size));
        }

        [Fact]
        public void IKeyedPool_TryAcquireAndTryRelease_ShouldWorkThroughInterface()
        {
            int size = sizeof(long);
            IKeyedPool pool = new KeyedPool<int, byte[]>(length => new byte[length]);
            byte[] buffer = new byte[size];

            bool released = pool.TryRelease(size, buffer);
            bool acquired = pool.TryAcquire(size, out object? acquiredObj);

            Assert.True(released);
            Assert.True(acquired);
            Assert.Same(buffer, acquiredObj);
        }
    }
}
