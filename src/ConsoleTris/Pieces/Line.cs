﻿using System;

namespace ConsoleTris.Pieces
{
    /// <summary>
    /// Represents the 'I' Piece
    /// </summary>
    public class Line : FallingPiece
    {
        public Line(Board board, Point[] points) : base(board)
        {
            if (points.Length != 4)
            {
                throw new ArgumentException("Line must contain 4 segments.");
            }
            Points = points;
            foreach (Point point in Points)
            {
                board.OccupiedFalling[point.X, point.Y] = true;
            }
        }

        public Line(Board board) : base(board)
        {
            Points = new Point[4]
            {
                new Point(3, 0),
                new Point(4, 0),
                new Point(5, 0),
                new Point(6, 0)
            };

            foreach (Point point in Points)
            {
                board.OccupiedFalling[point.X, point.Y] = true;
            }
            BlockType = BlockType.I;
        }

        /// <summary>
        /// Causes the rotate if possible
        /// </summary>
        public override void Rotate()
        {
            Point[] newPoints = new Point[4];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = new();
            }

            switch (Rotation)
            {
                case 0:
                    for (int i = 0; i < 4; i++)
                    {
                        newPoints[i].X = Points[i].X + 2 - i;
                        newPoints[i].Y = Points[i].Y + i - 1;
                    }
                    break;
                case 1:
                    for (int i = 0; i < 4; i++)
                    {
                        newPoints[i].X = Points[i].X + 1 - i;
                        newPoints[i].Y = Points[i].Y + 2 - i;
                    }
                    break;
                case 2:
                    for (int i = 0; i < 4; i++)
                    {
                        newPoints[i].X = Points[i].X + i - 2;
                        newPoints[i].Y = Points[i].Y + i - 2;
                    }
                    break;
                case 3:
                    for (int i = 0; i < 4; i++)
                    {
                        newPoints[i].X += Points[i].X + i - 1;
                        newPoints[i].Y += Points[i].Y + 1 - i;
                    }
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
