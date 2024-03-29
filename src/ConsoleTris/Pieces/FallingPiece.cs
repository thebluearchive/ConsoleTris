﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public abstract class FallingPiece
    {
        /// <summary>
        /// Array of points that belong to the piece, relative to the top of the board.
        /// </summary>
        public abstract Point[] Points { get; protected set; }
        /// <summary>
        /// Keeps track of the current piece's center of rotation.
        /// </summary>
        protected abstract Point Center { get; set; }
        public bool IsFalling { get; protected set; } = true;
        protected readonly IBoard _board;
        public BlockType BlockType;

        /// <summary>
        /// Creates a new falling piece object
        /// </summary>
        /// <param name="board">The board that this falling piece belongs to</param>
        public FallingPiece(IBoard board)
        {
            _board = board;
        }

        /// <summary>
        /// Places the falling piece on the board
        /// </summary>
        /// <returns>True if the piece can be successfully placed, false otherwise</returns>
        public virtual bool Initialize()
        {
            bool translateUpOne = false;
            foreach (Point point in Points)
            {
                //translate points to the middle of the board
                point.X += Board.WIDTH / 2 - 2;
                if (_board.PlacedBlocks[point.X, point.Y] != BlockType.Empty)
                {
                    translateUpOne = true;
                }
            }
            Center.X += Board.WIDTH / 2 - 2;

            if (translateUpOne)
            {
                foreach (Point point in Points)
                {
                    point.Y -= 1;
                }
                Center.Y -= 1;
            }

            // Check loss condition
            foreach (Point point in Points)
            {
                if (_board.PlacedBlocks[point.X, point.Y] != BlockType.Empty)
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Causes the piece to fall one block if possible
        /// </summary>
        public virtual void MoveDown()
        {
            foreach (Point point in Points)
            {
                // Check bounds and collisions
                Point pointLowered = new(point.X, point.Y + 1);
                if (!_board.IsValidPlacement(pointLowered))
                {
                    PieceStoppedFalling();
                    return;
                }
            }

            foreach (Point point in Points)
            {
                point.Y++;
            }
            Center.Y += 1;
        }

        public void MoveRight()
        {
            if (!IsFalling) return;

            bool canMoveRight = true;
            foreach (Point point in Points)
            {
                // Check bounds and collisions
                Point pointMoved = new(point.X + 1, point.Y);
                if (!_board.IsValidPlacement(pointMoved))
                {
                    canMoveRight = false;
                }
            }

            if (!canMoveRight) return;
            foreach (Point point in Points)
            {
                point.X++;
            }
            Center.X += 1;
        }
        
        public void MoveLeft()
        {
            bool canMoveLeft = true;
            foreach (Point point in Points)
            {
                // Check bounds and collisions
                Point pointMoved = new(point.X - 1, point.Y);
                if (!_board.IsValidPlacement(pointMoved))
                {
                    canMoveLeft = false;
                }
            }

            if (!canMoveLeft) return;

            foreach (Point point in Points)
            {
                point.X--;
            }
            Center.X -= 1;
        }

        public void PieceStoppedFalling()
        {
            IsFalling = false;
            foreach (Point point in Points)
            {
                _board.PlacedBlocks[point.X, point.Y] = BlockType;
            }
        }

        public Point[] GetProjection()
        {
            Point[] projectedPoints = new Point[Points.Length];

            for (int i = 0; i < projectedPoints.Length; i++)
            {
                projectedPoints[i] = new(Points[i].X, Points[i].Y);                
            }

            int lowestProjY = 0;
            for (int i = 0; i < Board.HEIGHT; i++)
            {
                for (int j = 0; j < Points.Length; j++)
                {
                    projectedPoints[j].Y = Points[j].Y + i;
                }
                
                if (_board.IsValidPlacement(projectedPoints))
                {
                    lowestProjY = i;
                }
                else // if we find an invalid projection, we cannot search any lower for valid projections
                {
                    break;
                }
            }

            for (int i = 0; i < projectedPoints.Length; i++)
            {
                projectedPoints[i].Y = Points[i].Y + lowestProjY;
            }
            return projectedPoints;
        }

        public void Drop()
        {
            Points = GetProjection();
            PieceStoppedFalling();
        }

        public virtual void MoveDownNoCollision()
        {
            foreach (Point point in Points)
            {
                // Check bounds and collisions
                Point pointLowered = new(point.X, point.Y + 1);
                if (!_board.IsInBounds(pointLowered))
                {
                    PieceStoppedFalling();
                    return;
                }
            }

            foreach (Point point in Points)
            {
                point.Y++;
            }
            Center.Y += 1;
        }

        public virtual void Rotate()
        {
            Point[] rotatedPoints = new Point[Points.Length];
            for (int i = 0; i < rotatedPoints.Length; i++)
            {
                Point relPoint = new(Points[i].X - Center.X, Points[i].Y - Center.Y);
                rotatedPoints[i] = new Point(-relPoint.Y + Center.X, relPoint.X + Center.Y);
            }

            // Check validity of proposed rotation
            if (!_board.IsValidPlacement(rotatedPoints)) return;

            Points = rotatedPoints;
        }
    }
}
