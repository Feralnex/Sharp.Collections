using System;
using Xunit;

namespace Sharp.Collections.Tests
{
    public class ConcurrentKeyedPoolTests
    {
        [Fact]
        public void NewConcurrentKeyedPool_WhenDefaultConstructorUsed_ShouldReturnEmptyInstance()
        {
            int size = sizeof(int);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();

            Assert.Equal(0, pool.Count(size));
        }

        [Fact]
        public void Release_WhenArrayIsNotNull_ShouldIncreaseCount()
        {
            int size = sizeof(long);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();
            byte[] buffer = new byte[size];

            pool.Release(size, buffer);

            Assert.Equal(1, pool.Count(size));
        }

        [Fact]
        public void Acquire_WhenConcurrentKeyedPoolIsNotEmpty_ShouldReturnInstance()
        {
            int size = sizeof(double);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();
            byte[] expected = new byte[size];

            pool.Release(size, expected);
            byte[] instance = pool.Acquire(size);

            Assert.Same(expected, instance);
        }

        [Fact]
        public void Acquire_WhenConcurrentKeyedPoolIsEmpty_ShouldThrowInvalidOperationException()
        {
            int size = sizeof(decimal);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();

            Assert.Throws<InvalidOperationException>(() => pool.Acquire(size));
        }

        [Fact]
        public void Release_WhenArrayIsNull_ShouldThrowArgumentNullException()
        {
            int size = sizeof(short);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();
            byte[]? buffer = null;

            Assert.Throws<ArgumentNullException>(() => pool.Release(size, buffer!));
        }

        [Fact]
        public void AcquireAcceptingCallback_WhenConcurrentKeyedPoolIsEmpty_ShouldUseCallbackAndReturnNewArray()
        {
            int size = sizeof(ushort);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();

            byte[] buffer = pool.Acquire(size, length => new byte[length]);

            Assert.Equal(size, buffer.Length);
        }

        [Fact]
        public void TryAcquire_WhenConcurrentKeyedPoolIsNotEmpty_ShouldReturnTrueAndAssignInstance()
        {
            int size = sizeof(uint);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();
            byte[] expected = new byte[size];

            pool.Release(size, expected);
            bool acquired = pool.TryAcquire(size, out byte[]? buffer);

            Assert.True(acquired);
            Assert.Same(expected, buffer);
        }

        [Fact]
        public void TryAcquire_WhenConcurrentKeyedPoolIsEmpty_ShouldReturnFalseAndAssignDefaultValue()
        {
            int size = sizeof(ulong);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();

            bool acquired = pool.TryAcquire(size, out byte[]? buffer);

            Assert.False(acquired);
            Assert.Null(buffer);
        }

        [Fact]
        public void TryRelease_WhenReleasedArray_ShouldReturnTrue()
        {
            int size = sizeof(float);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();
            byte[] buffer = new byte[size];

            bool released = pool.TryRelease(size, buffer);

            Assert.True(released);
        }

        [Fact]
        public void Acquire_WhenProvidedCallbackToCreateNewInstance_ShouldUseOnCreateNewCallbackAndReturnNewArray()
        {
            int size = sizeof(int);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>(length => new byte[length]);

            byte[] buffer = pool.Acquire(size);

            Assert.Equal(size, buffer.Length);
        }

        [Fact]
        public void ReleaseAndAcquire_WithDifferentKeys_ShouldKeepBucketsIsolated()
        {
            int smallSize = sizeof(short);
            int largeSize = sizeof(long);
            ConcurrentKeyedPool<int, byte[]> pool = new ConcurrentKeyedPool<int, byte[]>();
            byte[] small = new byte[smallSize];
            byte[] large = new byte[largeSize];

            pool.Release(smallSize, small);
            pool.Release(largeSize, large);

            byte[] acquiredSmall = pool.Acquire(smallSize);
            byte[] acquiredLarge = pool.Acquire(largeSize);

            Assert.Same(small, acquiredSmall);
            Assert.Same(large, acquiredLarge);
        }
    }
}
