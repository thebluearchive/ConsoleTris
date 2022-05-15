using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public class LPiece : FallingPiece
    {
        public override Point[] Points { get; protected set; } = new Point[4]
            {
                new Point(0, 2),
                new Point(1, 2),
                new Point(2, 2),
                new Point(2, 1)
            };
        protected override Point Center { get; set; } = new(1, 2);


        public LPiece(IBoard board) : base(board)
        {
            BlockType = BlockType.L;
        }
    }
}
