using Xunit;

namespace Sharp.Collections.Tests
{
    public class SegmentsTests
    {
        [Fact]
        public void Constructor_WhenInitializedWithRoot_ShouldSetHeadAndTailToRoot()
        {
            // Arrange
            int segmentSize = 16;
            Segment<string> root = new Segment<string>(segmentSize);

            // Act
            Segments<string> segments = new Segments<string>(root);

            // Assert
            Assert.Equal(root, segments.Head);
            Assert.Equal(root, segments.Tail);
        }

        [Fact]
        public void AddToHead_WhenAddingNewHead_ShouldAddSegmentToHeadAndUpdateTail()
        {
            // Arrange
            int segmentSize = 16;
            Segment<string> root = new Segment<string>(segmentSize);
            Segment<string> secondSegment = new Segment<string>(segmentSize);
            Segments<string> segments = new Segments<string>(root);

            // Act
            segments.AddToHead(secondSegment);

            // Assert
            Assert.Equal(root, segments.Head);
            Assert.Equal(secondSegment, segments.Head.NextHead);
            Assert.Equal(secondSegment, segments.Tail);
        }

        [Fact]
        public void AddToTail_WhenAddingNewTail_ShouldAddSegmentToTailAndUpdateTail()
        {
            // Arrange
            int segmentSize = 16;
            Segment<string> root = new Segment<string>(segmentSize);
            Segment<string> secondSegment = new Segment<string>(segmentSize);
            Segments<string> segments = new Segments<string>(root);

            // Act
            segments.AddToTail(secondSegment);

            // Assert
            Assert.Equal(root, segments.Head);
            Assert.Equal(secondSegment, segments.Head.NextTail);
            Assert.Equal(secondSegment, segments.Tail);
        }

        [Fact]
        public void MoveToNextHead_WhenMovingToNextHead_ShouldMoveHeadToNextHead()
        {
            // Arrange
            int segmentSize = 16;
            Segment<string> root = new Segment<string>(segmentSize);
            Segment<string> newHead = new Segment<string>(segmentSize);
            Segments<string> segments = new Segments<string>(root);

            // Act
            segments.AddToHead(newHead);
            segments.MoveToNextHead();

            // Assert
            Assert.Equal(newHead, segments.Head);
            Assert.Equal(newHead, segments.Tail);
            Assert.Null(root.NextHead);
        }

        [Fact]
        public void MoveToNextTail_WhenMovingToNextTail_ShouldMoveHeadToNextTail()
        {
            // Arrange
            int segmentSize = 16;
            Segment<string> root = new Segment<string>(segmentSize);
            Segment<string> newTail = new Segment<string>(segmentSize);
            Segments<string> segments = new Segments<string>(root);

            // Act
            segments.AddToTail(newTail);
            segments.MoveToNextTail();

            // Assert
            Assert.Equal(newTail, segments.Head);
            Assert.Equal(newTail, segments.Tail);
            Assert.Null(root.NextTail);
        }
    }
}