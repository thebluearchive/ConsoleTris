using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public class OPiece : FallingPiece
    {
        public OPiece(Board board) : base(board)
        {
            Points = new Point[4]
            {
                new Point(4, 1),
                new Point(5, 1),
                new Point(4, 0),
                new Point(5, 0)
            };

            foreach (Point point in Points)
            {
                board.OccupiedFalling[point.X, point.Y] = true;
            }
            BlockType = BlockType.O;
        }
    }
}
