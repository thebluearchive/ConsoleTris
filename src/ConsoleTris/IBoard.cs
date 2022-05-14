using ConsoleTris.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris
{
    public interface IBoard
    {
        public BlockType[,] PlacedBlocks { get; }
        public bool IsValidPlacement(Point point);
        public bool IsValidPlacement(Point[] points);
        public bool IsInBounds(Point point);
    }
}
