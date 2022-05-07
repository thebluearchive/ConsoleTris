using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    internal class TPiece : FallingPiece
    {
        public override Point[] Points { get; protected set; } = new Point[4]
            {
                new Point(0, 2),
                new Point(1, 2),
                new Point(2, 2),
                new Point(1, 1)
            };

        public TPiece(Board board) : base(board)
        {
            BlockType = BlockType.T;
        }

        public override void Rotate()
        {
            Point[] newPoints = new Point[4];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = new(Points[i].X, Points[i].Y);
            }

            switch (Rotation)
            {
                case 0:
                    newPoints[0].X = Points[0].X + 1;
                    newPoints[0].Y = Points[0].Y - 1;
                    newPoints[2].X = Points[2].X - 1;
                    newPoints[2].Y = Points[2].Y + 1;
                    newPoints[3].X = Points[3].X + 1;
                    newPoints[3].Y = Points[3].Y + 1;
                    break;
                case 1:
                    newPoints[0].X = Points[0].X + 1;
                    newPoints[0].Y = Points[0].Y + 1;
                    newPoints[2].X = Points[2].X - 1;
                    newPoints[2].Y = Points[2].Y - 1;
                    newPoints[3].X = Points[3].X - 1;
                    newPoints[3].Y = Points[3].Y + 1;
                    break;
                case 2:
                    newPoints[0].X = Points[0].X - 1;
                    newPoints[0].Y = Points[0].Y + 1;
                    newPoints[2].X = Points[2].X + 1;
                    newPoints[2].Y = Points[2].Y - 1;
                    newPoints[3].X = Points[3].X - 1;
                    newPoints[3].Y = Points[3].Y - 1;
                    break;
                case 3:
                    newPoints[0].X = Points[0].X - 1;
                    newPoints[0].Y = Points[0].Y - 1;
                    newPoints[2].X = Points[2].X + 1;
                    newPoints[2].Y = Points[2].Y + 1;
                    newPoints[3].X = Points[3].X + 1;
                    newPoints[3].Y = Points[3].Y - 1;
                    break;
            }

            // Check validity of proposed rotation
            if (!_board.IsValidPlacement(newPoints)) return;

            Points = newPoints;

            // Increment the rotation counter
            Rotation = (Rotation + 1) % 4;
        }
    }
}
