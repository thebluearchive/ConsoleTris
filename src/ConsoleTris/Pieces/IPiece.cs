using System;

namespace ConsoleTris.Pieces
{
    /// <summary>
    /// Represents the 'I' Piece. *Not* an interface.
    /// </summary>
    public class IPiece : FallingPiece
    {
        public override Point[] Points { get; protected set; } = new Point[4]
            {
                new Point(0, 2),
                new Point(1, 2),
                new Point(2, 2),
                new Point(3, 2)
            };
        protected override Point Center { get; set; } = new(2, 2);

        public IPiece(IBoard board) : base(board)
        {
            BlockType = BlockType.I;
        }

        /// <summary>
        /// Causes the piece to rotate if possible.
        /// </summary>
        public override void Rotate()
        {
            // The I Piece rotates a bit differently than the other pieces. It doesn't rotate about 
            // a center block, but rather around a center point which is on the corner of a block.
            Point[] rotatedPoints = new Point[Points.Length];
            for (int i = 0; i < rotatedPoints.Length; i++)
            {
                Point relPoint = new(Points[i].X - Center.X, Points[i].Y - Center.Y);
                rotatedPoints[i] = new Point(-relPoint.Y + Center.X - 1, relPoint.X + Center.Y);
            }

            // Check validity of proposed rotation
            if (!_board.IsValidPlacement(rotatedPoints)) return;

            Points = rotatedPoints;
        }
    }
}
