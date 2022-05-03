using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public class LPiece : FallingPiece
    {
        public LPiece(Board board) : base(board)
        {
            Points = new Point[4]
            {
                new Point(4, 1),
                new Point(5, 1),
                new Point(6, 1),
                new Point(6, 0)
            };

            foreach (Point point in Points)
            {
                board.OccupiedFalling[point.X, point.Y] = true;
            }
            BlockType = BlockType.L;
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
                    newPoints[3].X = Points[3].X;
                    newPoints[3].Y = Points[3].Y + 2;
                    break;
                case 1:
                    newPoints[0].X = Points[0].X + 1;
                    newPoints[0].Y = Points[0].Y + 1;
                    newPoints[2].X = Points[2].X - 1;
                    newPoints[2].Y = Points[2].Y - 1;
                    newPoints[3].X = Points[3].X - 2;
                    newPoints[3].Y = Points[3].Y;
                    break;
                case 2:
                    newPoints[0].X = Points[0].X - 1;
                    newPoints[0].Y = Points[0].Y + 1;
                    newPoints[2].X = Points[2].X + 1;
                    newPoints[2].Y = Points[2].Y - 1;
                    newPoints[3].X = Points[3].X;
                    newPoints[3].Y = Points[3].Y - 2;
                    break;
                case 3:
                    newPoints[0].X = Points[0].X - 1;
                    newPoints[0].Y = Points[0].Y - 1;
                    newPoints[2].X = Points[2].X + 1;
                    newPoints[2].Y = Points[2].Y + 1;
                    newPoints[3].X = Points[3].X + 2;
                    newPoints[3].Y = Points[3].Y;
                    break;
            }

            // Check validity of proposed rotation
            if (!_board.IsValidPlacement(newPoints)) return;

            foreach (Point point in Points)
            {
                _board.OccupiedFalling[point.X, point.Y] = false;
            }
            foreach (Point point in newPoints)
            {
                _board.OccupiedFalling[point.X, point.Y] = true;
            }
            Points = newPoints;

            // Increment the rotation counter
            Rotation = (Rotation + 1) % 4;
        }
    }
}
