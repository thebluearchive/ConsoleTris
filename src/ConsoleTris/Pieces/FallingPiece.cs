using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris.Pieces
{
    public abstract class FallingPiece
    {
        /// <summary>
        /// Array of points that belong to the piece, relative to the top of the board
        /// </summary>
        public Point[] Points { get; protected set; }
        /// <summary>
        /// Rotation is a number between 0 and 3, representing how many times the block
        /// has been rotated clockwise from its original position
        /// </summary>
        public int Rotation { get; set; }
        public bool IsFalling { get; protected set; } = true;
        protected readonly Board _board;
        public BlockType BlockType;

        /// <summary>
        /// Creates a new falling piece object
        /// </summary>
        /// <param name="board">The board that this falling piece belongs to</param>
        public FallingPiece(Board board)
        {
            _board = board;
        }

        /// <summary>
        /// Causes the piece to fall one block if possible
        /// </summary>
        public void MoveDown()
        {
            foreach (Point point in Points)
            {
                // Check bounds and collisions
                Point pointLowered = new(point.X, point.Y + 1);
                if (!_board.IsValidPlacement(pointLowered))
                {
                    IsFalling = false;
                    PieceStoppedFalling();
                    return;
                }
            }

            foreach (Point point in Points)
            {
                _board.OccupiedFalling[point.X, point.Y] = false;
                point.Y++;
            }
            foreach (Point point in Points)
            {
                _board.OccupiedFalling[point.X, point.Y] = true;
            }
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
                _board.OccupiedFalling[point.X, point.Y] = false;
                point.X++;
            }
            foreach (Point point in Points)
            {
                _board.OccupiedFalling[point.X, point.Y] = true;
            }
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
                _board.OccupiedFalling[point.X, point.Y] = false;
                point.X--;
            }
            foreach (Point point in Points)
            {
                _board.OccupiedFalling[point.X, point.Y] = true;
            }
        }

        public void PieceStoppedFalling()
        {
            foreach (Point point in Points)
            {
                _board.OccupiedFalling[point.X, point.Y] = false;
                _board.PlacedBlocks[point.X, point.Y] = BlockType;
            }
        }

        public virtual void Rotate()
        {
        }
    }
}
