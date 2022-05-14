﻿using System;

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

        public IPiece(IBoard board) : base(board)
        {
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

            Points = newPoints;

            // Increment the rotation counter
            Rotation = (Rotation + 1) % 4;
        }
    }
}
