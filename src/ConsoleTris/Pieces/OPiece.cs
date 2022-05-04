using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public class OPiece : FallingPiece
    {
        public override Point[] Points { get; protected set; } = new Point[4]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(0, 0),
                new Point(1, 0)
            };

        public OPiece(Board board) : base(board)
        {
            BlockType = BlockType.O;
        }

        public override void Rotate()
        {
            // OPiece cannot rotate
        }
    }
}
