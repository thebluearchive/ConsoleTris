using System;

namespace ConsoleTris.Pieces
{
    /// <summary>
    /// Represents the 'I' Piece
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
        /// Causes the rotate if possible
        /// </summary>
        public override void Rotate()
        {
            Point[] rotatedPoints = new Point[Points.Length];
            for (int i = 0; i < rotatedPoints.Length; i++)
            {
                Point relPoint = new(Points[i].X - Center.X, Points[i].Y - Center.Y);
                float relPointX = Points[i].X - (Center.X - 0.5f);
                float relPointY = Points[i].Y - (Center.Y - 0.5f);
                rotatedPoints[i] = new Point();
                rotatedPoints[i].X = (int)(-relPointY + (Center.X - 0.5f));
                rotatedPoints[i].Y = (int)(relPointX + (Center.Y - 0.5f));
                //rotatedPoints[i] = new Point(-relPoint.Y + Center.X, relPoint.X + Center.Y);
            }

            // Check validity of proposed rotation
            if (!_board.IsValidPlacement(rotatedPoints)) return;

            Points = rotatedPoints;
        }
    }
}
