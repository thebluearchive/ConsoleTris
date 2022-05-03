using Pastel;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleTris.Pieces;

namespace ConsoleTris
{
    public class Board
    {
        public readonly int WIDTH;
        public readonly int HEIGHT;
        private Random random = new();

        public bool[,] OccupiedFalling { get; set; }
        public BlockType[,] PlacedBlocks { get; set; }

        private FallingPiece fallingPiece;

        private int fallTimer = 0;

        public Board() : this(10, 20)
        {
        }

        public Board(int width, int height)
        {
            WIDTH = width;
            HEIGHT = height;

            PlacedBlocks = new BlockType[WIDTH, HEIGHT];
            for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < PlacedBlocks.GetLength(1); j++)
                {
                    PlacedBlocks[i, j] = BlockType.Empty;
                }
            }
            OccupiedFalling = new bool[WIDTH, HEIGHT];
        }

        public void Draw()
        {
            StringBuilder sb = new();
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j < HEIGHT; j++)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    string hex = GetBlockTypeColor(PlacedBlocks[i, j]);
                    if (OccupiedFalling[i, j])
                        hex = GetBlockTypeColor(fallingPiece.BlockType);
                    sb.Append("  ".PastelBg(hex));
                }
            }
            string str = sb.ToString();
            Console.Write(str);
        }

        public void UpdateState()
        {
            // If there are currently no falling pieces, we generate a new piece
            if (fallingPiece == null || !fallingPiece.IsFalling)
            {
                AddPiece();
            }

            // Cause any existing blocks to fall
            Fall();

            // Clear any completed rows
            ClearRows();
        }

        private void AddPiece()
        {
            FallingPiece newPiece;
            int randomNum = random.Next(7);
            switch (randomNum)
            {
                case 0:
                    newPiece = new Line(this);
                    break;
                case 1:
                    newPiece = new TPiece(this);
                    break;
                case 2:
                    newPiece = new OPiece(this);
                    break;
                case 3:
                    newPiece = new SPiece(this);
                    break;
                case 4:
                    newPiece = new ZPiece(this);
                    break;
                case 5:
                    newPiece = new LPiece(this);
                    break;
                case 6:
                    newPiece = new JPiece(this);
                    break;
                default:
                    // This case will never be hit, but the compiler really wants me to put it
                    newPiece = new Line(this);
                    break;
            }
            fallingPiece = newPiece;
        }

        private void Fall()
        {
            if (fallTimer % 20 == 0)
            {
                fallingPiece.MoveDown();
            }
            fallTimer = (fallTimer + 1) % 20;
        }

        public void HandleUserInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.RightArrow:
                    fallingPiece.MoveRight();
                    break;
                case ConsoleKey.LeftArrow:
                    fallingPiece.MoveLeft();
                    break;
                case ConsoleKey.DownArrow:
                    fallingPiece.MoveDown();
                    break;
                case ConsoleKey.C:
                    fallingPiece.Rotate();
                    break;
            }
        }

        /// <summary>
        /// Returns true if a point is within bounds and false otherwise
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInBounds(Point point)
        {
            return point.X >= 0
                && point.X < WIDTH
                && point.Y >= 0
                && point.Y < HEIGHT;
        }

        /// <summary>
        /// Returns true if the point collides with other objects on the board,
        /// false otherwise
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsCollision(Point point)
        {
            return !(PlacedBlocks[point.X, point.Y] == BlockType.Empty);
        }

        /// <summary>
        /// Returns true if the point is within bounds and does not collide with
        /// any existing objects.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsValidPlacement(Point point)
        {
            bool inBounds = IsInBounds(point);
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

        private string GetBlockTypeColor(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.I:
                    return "#00FFFF";
                case BlockType.J:
                    return "#0000FF";
                case BlockType.L:
                    return "#FFAA00";
                case BlockType.O:
                    return "#FFFF00";
                case BlockType.S:
                    return "#00FF00";
                case BlockType.T:
                    return "#9900FF";
                case BlockType.Z:
                    return "#FF0000";
                case BlockType.Empty:
                    return "#000000";
            }

            // Code should never hit this since BlockType is non-nullable and 
            // we have checked every possible value for the BlockType enum.
            // However the compiler is unhappy unless I return something or throw
            // an error here. So I am choosing to throw an error if this line of 
            // code is ever hit.
            throw new Exception("Unable to determine Block Type.");
        }

        private void ClearRows()
        {
            HashSet<int> completedRows = new();
            for (int j = HEIGHT - 1; j >= 0; j--)
            {
                bool rowCompleted = true;
                for (int i = 0; i < WIDTH; i++)
                {
                    rowCompleted &= (PlacedBlocks[i, j] != BlockType.Empty);
                }
                if (rowCompleted)
                {
                    completedRows.Add(j);
                }
            }

            int currentRow = HEIGHT - 1;
            for (int j = HEIGHT - 1; j >= 0; j--)
            {
                if (!completedRows.Contains(j))
                {
                    for (int i = 0; i < WIDTH; i++)
                    {
                        PlacedBlocks[i, currentRow] = PlacedBlocks[i, j];
                    }
                    currentRow--;
                }
            }

            for (int j = 0; j < currentRow; j++)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    PlacedBlocks[i, currentRow] = BlockType.Empty;
                }
            }
        }
    }
}
