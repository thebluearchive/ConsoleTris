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
        public abstract Point[] Points { get; protected set; }
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
            
            if (translateUpOne)
            {
                foreach (Point point in Points)
                {
                    point.Y -= 1;
                }
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

        public abstract void Rotate();
    }
}
