using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public class JPiece : FallingPiece
    {
        public override Point[] Points { get; protected set; } = new Point[4]
            {
                new Point(0, 1),
                new Point(0, 2),
                new Point(1, 2),
                new Point(2, 2)
            };

        public JPiece(IBoard board) : base(board)
        {
            BlockType = BlockType.J;
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
                    newPoints[0].X = Points[0].X + 2;
                    newPoints[0].Y = Points[0].Y;
                    newPoints[1].X = Points[1].X + 1;
                    newPoints[1].Y = Points[1].Y - 1;
                    newPoints[3].X = Points[3].X - 1;
                    newPoints[3].Y = Points[3].Y + 1;
                    break;
                case 1:
                    newPoints[0].X = Points[0].X;
                    newPoints[0].Y = Points[0].Y + 2;
                    newPoints[1].X = Points[1].X + 1;
                    newPoints[1].Y = Points[1].Y + 1;
                    newPoints[3].X = Points[3].X - 1;
                    newPoints[3].Y = Points[3].Y - 1;
                    break;
                case 2:
                    newPoints[0].X = Points[0].X - 2;
                    newPoints[0].Y = Points[0].Y;
                    newPoints[1].X = Points[1].X - 1;
                    newPoints[1].Y = Points[1].Y + 1;
                    newPoints[3].X = Points[3].X + 1;
                    newPoints[3].Y = Points[3].Y - 1;
                    break;
                case 3:
                    newPoints[0].X = Points[0].X;
                    newPoints[0].Y = Points[0].Y - 2;
                    newPoints[1].X = Points[1].X - 1;
                    newPoints[1].Y = Points[1].Y - 1;
                    newPoints[3].X = Points[3].X + 1;
                    newPoints[3].Y = Points[3].Y + 1;
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
