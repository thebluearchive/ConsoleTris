using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public class SPiece : FallingPiece
    {
        public override Point[] Points { get; protected set; } = new Point[4]
            {
                new Point(2, 1),
                new Point(1, 1),
                new Point(1, 2),
                new Point(0, 2)
            };
        protected override Point Center { get; set; } = new(1, 2);

        public SPiece(IBoard board) : base(board)
        {
            BlockType = BlockType.S;
        }
    }
}
