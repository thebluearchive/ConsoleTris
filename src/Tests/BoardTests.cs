using ConsoleTris.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class BoardTests
    {
        [Fact]
        public void TestBoardConstructor()
        {
            Board board = new(4, 4);

            int width = board.PlacedBlocks.GetLength(0);
            int height = board.PlacedBlocks.GetLength(1);
            Assert.Equal(4, width);
            Assert.Equal(4, height);
        }
    }
}
