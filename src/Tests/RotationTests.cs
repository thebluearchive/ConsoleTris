using Xunit;
using ConsoleTris.Pieces;

namespace Tests
{
    public class RotationTests
    {
        [Fact]
        public void TestLineRotation1()
        {
            Board board = new(4, 4);
            Point[] points = new Point[4]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1)
            };

            Line line = new(board, points);

            bool[,] expected = new bool[4, 4]
            {
                { false, true, false, false },
                { false, true, false, false},
                { false, true, false, false },
                { false, true, false, false }
            };
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Assert.Equal(expected[i, j], board.OccupiedFalling[i, j]);
                }
            }

            line.Rotate();

            Assert.Equal(2, points[0].X);
            Assert.Equal(2, points[1].X);
            Assert.Equal(2, points[2].X);
            Assert.Equal(2, points[3].X);

            Assert.Equal(0, points[0].Y);
            Assert.Equal(1, points[1].Y);
            Assert.Equal(2, points[2].Y);
            Assert.Equal(3, points[3].Y);

            expected = new bool[4, 4]
            {
                { false, false, false, false },
                { false, false, false, false },
                { true, true, true, true },
                { false, false, false, false }
            };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Assert.Equal(expected[i, j], board.OccupiedFalling[i, j]);
                }
            }
        }

        [Fact]
        public void TestLineRotation2()
        {
            Board board = new(4, 4);
            Point[] points = new Point[4]
            {
                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            };

            Line line = new(board, points);
            line.Rotation = 1;
            line.Rotate();

            Assert.Equal(3, points[0].X);
            Assert.Equal(2, points[1].X);
            Assert.Equal(1, points[2].X);
            Assert.Equal(0, points[3].X);

            Assert.Equal(2, points[0].Y);
            Assert.Equal(2, points[1].Y);
            Assert.Equal(2, points[2].Y);
            Assert.Equal(2, points[3].Y);
        }

        [Fact]
        public void TestLineRotation3()
        {
            Board board = new(4, 4);
            Point[] points = new Point[4]
            {
                new Point(3, 2),
                new Point(2, 2),
                new Point(1, 2),
                new Point(0, 2)
            };

            Line line = new(board, points);
            line.Rotation = 2;
            line.Rotate();

            Assert.Equal(1, points[0].X);
            Assert.Equal(1, points[1].X);
            Assert.Equal(1, points[2].X);
            Assert.Equal(1, points[3].X);

            Assert.Equal(0, points[0].Y);
            Assert.Equal(1, points[1].Y);
            Assert.Equal(2, points[2].Y);
            Assert.Equal(3, points[3].Y);
        }

        [Fact]
        public void TestLineRotation4()
        {
            Board board = new(4, 4);
            Point[] points = new Point[4]
            {
                new Point(1, 3),
                new Point(1, 2),
                new Point(1, 1),
                new Point(1, 0)
            };

            Line line = new(board, points);
            line.Rotation = 3;
            line.Rotate();

            Assert.Equal(0, points[0].X);
            Assert.Equal(1, points[1].X);
            Assert.Equal(2, points[2].X);
            Assert.Equal(3, points[3].X);

            Assert.Equal(1, points[0].Y);
            Assert.Equal(1, points[1].Y);
            Assert.Equal(1, points[2].Y);
            Assert.Equal(1, points[3].Y);
        }
    }
}