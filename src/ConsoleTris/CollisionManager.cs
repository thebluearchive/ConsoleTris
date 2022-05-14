using ConsoleTris.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTris
{
    public class CollisionManager
    {
        private IBoard _board;
        public CollisionManager(IBoard board)
        {
            _board = board;
        }

        /// <summary>
        /// Returns true if a point is within bounds and false otherwise
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInBounds(Point point)
        {
            return point.X >= 0
                && point.X < _board.PlacedBlocks.GetLength(0)
                && point.Y >= 0
                && point.Y < _board.PlacedBlocks.GetLength(1);
        }

        /// <summary>
        /// Returns true if the point collides with other objects on the board,
        /// false otherwise
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsCollision(Point point)
        {
            return !(_board.PlacedBlocks[point.X, point.Y] == BlockType.Empty);
        }

        /// <summary>
        /// Returns true if the point is within bounds and does not collide with
        /// any existing objects.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsValidPlacement(Point point)
        {
            return IsInBounds(point) && !IsCollision(point);
        }

        /// <summary>
        /// Returns true if each point in the array can be placed at the specified
        /// location and false otherwise
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool IsValidPlacement(Point[] points)
        {
            bool isValid = true;
            foreach (Point point in points)
            {
                isValid &= IsValidPlacement(point);
            }
            return isValid;
        }
    }
}
