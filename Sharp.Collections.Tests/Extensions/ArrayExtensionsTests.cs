using Sharp.Collections.Extensions;
using Xunit;

namespace Sharp.Collections.Tests.Extensions
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void TrySwap_WhenProvidedValidIndicesAndLength_ShouldSwapSuccessfully()
        {
            // Arrange
            byte[] source = [1, 2, 3, 4, 5];
            byte[] destination = [10, 11, 12, 13, 14];
            int sourceIndex = 1;
            int destinationIndex = 2;
            int length = 2;

            // Act
            bool result = source.TrySwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.True(result);
            Assert.Equal([1, 12, 13, 4, 5], source);
            Assert.Equal([10, 11, 2, 3, 14], destination);
        }

        [Fact]
        public void TrySwap_WhenProvidedSourceIndexOutOfBounds_ShouldReturnFalse()
        {
            // Arrange
            byte[] source = [1, 2, 3];
            byte[] destination = [10, 11, 12];
            int sourceIndex = 5;
            int destinationIndex = 1;
            int length = 2;

            // Act
            bool result = source.TrySwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TrySwap_WhenProvidedDestinationIndexOutOfBounds_ShouldReturnFalse()
        {
            // Arrange
            byte[] source = [1, 2, 3];
            byte[] destination = [10, 11, 12];
            int sourceIndex = 1;
            int destinationIndex = 4;
            int length = 2;

            // Act
            bool result = source.TrySwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TrySwap_WhenProvidedLengthExceedsSource_ShouldReturnFalse()
        {
            // Arrange
            byte[] source = [1, 2, 3];
            byte[] destination = [10, 11, 12];
            int sourceIndex = 1;
            int destinationIndex = 1;
            int length = 3;

            // Act
            bool result = source.TrySwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TrySwap_WhenProvidedLengthIsZero_ShouldNotPerformSwap()
        {
            // Arrange
            byte[] source = [1, 2, 3];
            byte[] destination = [10, 11, 12];
            int sourceIndex = 1;
            int destinationIndex = 1;
            int length = 0;

            // Act
            bool result = source.TrySwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.True(result);
            Assert.Equal([1, 2, 3], source);
            Assert.Equal([10, 11, 12], destination);
        }

        [Fact]
        public void DangerousSwap_WhenProvidedValidIndicesAndLength_ShouldSwapSuccessfully()
        {
            // Arrange
            byte[] source = [1, 2, 3, 4, 5];
            byte[] destination = [10, 11, 12, 13, 14];
            int sourceIndex = 1;
            int destinationIndex = 2;
            int length = 2;

            // Act
            source.DangerousSwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.Equal([1, 12, 13, 4, 5], source);
            Assert.Equal([10, 11, 2, 3, 14], destination);
        }

        [Fact]
        public void DangerousSwap_WhenProvidedLengthIsZero_ShouldNotPerformSwap()
        {
            // Arrange
            byte[] source = [1, 2, 3];
            byte[] destination = [10, 11, 12];
            int sourceIndex = 1;
            int destinationIndex = 1;
            int length = 0;

            // Act
            source.DangerousSwap(sourceIndex, destination, destinationIndex, length);

            // Assert
            Assert.Equal([1, 2, 3], source);
            Assert.Equal([10, 11, 12], destination);
        }
    }
}
