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
                new Point(1, 2),
                new Point(2, 2),
                new Point(1, 1),
                new Point(2, 1)
            };
        protected override Point Center { get; set; } = new(1, 1);

        public OPiece(IBoard board) : base(board)
        {
            BlockType = BlockType.O;
        }

        public override void Rotate()
        {
            // OPiece cannot rotate
        }
    }
}
