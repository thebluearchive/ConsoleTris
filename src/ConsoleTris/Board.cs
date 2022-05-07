using Pastel;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleTris.Pieces;
using System.Linq;
using System.Diagnostics;

namespace ConsoleTris
{
    public class Board
    {
        public readonly int WIDTH;
        public readonly int HEIGHT;
        private Random random = new();
        public int Score { get; private set; } = 0;
        private int lvl = 0;
        private readonly int _posX = 1;
        private readonly int _posY = 1;
        private bool displayFPS = false;
        public bool IsLoss = false;
        private bool canSwap = false;

        private readonly Stopwatch stopWatch = new();

        public bool[,] OccupiedFalling { get; set; }
        public BlockType[,] PlacedBlocks { get; set; }

        private FallingPiece fallingPiece;
        private FallingPiece nextPiece;
        /// <summary>
        /// Represents the piece that is currently being held. This piece can
        /// be swapped with the current falling piece as long as no swap has
        /// yet occurred with the current falling piece. This piece is null
        /// a piece is first held.
        /// </summary>
        private FallingPiece heldPiece;

        private int fallTimer = 0;

        public Board(int width, int height, int posX, int posY)
        {
            WIDTH = width;
            // The actual height is 2 blocks taller than the visible viewport
            HEIGHT = height + 2;

            PlacedBlocks = new BlockType[WIDTH, HEIGHT];
            for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < PlacedBlocks.GetLength(1); j++)
                {
                    PlacedBlocks[i, j] = BlockType.Empty;
                }
            }
            OccupiedFalling = new bool[WIDTH, HEIGHT];

            _posX = posX;
            _posY = posY;

            stopWatch.Start();
        }

        public void Draw()
        {
            Point[] projection = fallingPiece.GetProjection();
            
            // Draw Board
            for (int j = 2; j < HEIGHT; j++)
            {
                StringBuilder sb = new();
                Console.SetCursorPosition(_posX, _posY + j - 2);
                for (int i = 0; i < WIDTH; i++)
                {
                    string hex;
                    if (OccupiedFalling[i, j])
                        hex = GetBlockTypeColor(fallingPiece.BlockType);
                    else if (Array.Exists(projection, p => p.X == i && p.Y == j))
                        hex = "#222222";
                    else
                        hex = GetBlockTypeColor(PlacedBlocks[i, j]);
                    sb.Append("  ".PastelBg(hex));
                }
                Console.Write(sb.ToString());
            }

            // Draw Next Piece
            // TODO: don't hardcode next piece display height and width?
            int width = 5;
            int height = 4;
            bool[, ] nextPieceDisplay = new bool[width, height];
            foreach (Point point in nextPiece.Points)
            {
                nextPieceDisplay[point.X, point.Y] = true;
            }
            Console.SetCursorPosition(_posX + 2 * WIDTH, _posY);
            Console.Write("Next:");
            for (int j = 0; j < height - 1; j++)
            {
                StringBuilder sb = new();
                Console.SetCursorPosition(_posX + 2 * WIDTH, _posY + j + 1);
                for (int i = 0; i < width; i++)
                {
                    string hex;
                    if (nextPieceDisplay[i, j])
                    {
                        hex = GetBlockTypeColor(nextPiece.BlockType);
                    }
                    else
                    {
                        hex = "#000000";
                    }
                    sb.Append("  ".PastelBg(hex));
                }
                Console.Write(sb.ToString());
            }

            // Draw currently held piece
            width = 5;
            height = 4;
            bool[,] heldPieceDisplay = new bool[width, height];
            if (heldPiece is not null)
            {
                foreach (Point point in heldPiece.Points)
                {
                    heldPieceDisplay[point.X, point.Y] = true;
                }
            }
            Console.SetCursorPosition(_posX + 2 * WIDTH, _posY + height);
            Console.Write("Held:");
            for (int j = 0; j < height - 1; j++)
            {
                StringBuilder sb = new();
                Console.SetCursorPosition(_posX + 2 * WIDTH, _posY + j + height + 1);
                for (int i = 0; i < width; i++)
                {
                    string hex;
                    if (heldPieceDisplay[i, j])
                    {
                        hex = GetBlockTypeColor(heldPiece.BlockType);
                    }
                    else
                    {
                        hex = "#000000";
                    }
                    sb.Append("  ".PastelBg(hex));
                }
                Console.Write(sb.ToString());
            }

            // Draw current score and lvl
            Console.SetCursorPosition(_posX + 2 * WIDTH, _posY + 2*height);
            Console.Write($"Score: {Score}");
            Console.SetCursorPosition(_posX + 2 * WIDTH, _posY + 2*height + 1);
            Console.Write($"Level {lvl}");

            // number of milliseconds a tick has taken on average
            double averageTick = FPSCalc.CalcAverageTick(stopWatch.ElapsedMilliseconds);
            double fps = 1000 / averageTick;
            if (displayFPS)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write($"FPS = {fps.ToString("F2")}");
            }
            stopWatch.Reset();
            stopWatch.Start();
        }

        public void UpdateState()
        {
            // If there are currently no falling pieces, we generate a new piece
            if (fallingPiece == null || !fallingPiece.IsFalling)
            {
                canSwap = true;
                UpdateFallingPiece();
            }

            // Cause any existing blocks to fall
            Fall();

            // Clear any completed rows
            ClearRows();
        }

        private void UpdateFallingPiece()
        {
            // This should only run once at the beginning of the game
            if (nextPiece is null)
            {
                nextPiece = GenerateRandomPiece();
            }

            fallingPiece = nextPiece;
            IsLoss = !fallingPiece.Initialize();

            nextPiece = GenerateRandomPiece();
        }

        private FallingPiece GenerateRandomPiece()
        {
            int randomNum = random.Next(7);
            FallingPiece newPiece = randomNum switch
            {
                0 => new IPiece(this),
                1 => new TPiece(this),
                2 => new OPiece(this),
                3 => new SPiece(this),
                4 => new ZPiece(this),
                5 => new LPiece(this),
                6 => new JPiece(this),
                _ => new IPiece(this),// This case will never be hit, but the compiler really wants me to put it
            };
            return newPiece;
        }

        private void Fall()
        {
            if (fallTimer == 0)
            {
                fallingPiece.MoveDown();
            }
            fallTimer = (fallTimer + 1) % 50;
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
                case ConsoleKey.Z:
                    fallingPiece.Rotate();
                    break;
                case ConsoleKey.Spacebar:
                    fallingPiece.Drop();
                    break;
                case ConsoleKey.C:
                    SwapFallingPiece();
                    break;
                case ConsoleKey.F:
                    displayFPS = !displayFPS;
                    break;
            }
        }

        private void SwapFallingPiece()
        {
            if (!canSwap) return;
            foreach (Point point in fallingPiece.Points)
            {
                OccupiedFalling[point.X, point.Y] = false;
            }
            if (heldPiece is null)
            {
                heldPiece = (FallingPiece)Activator.CreateInstance(fallingPiece.GetType(), this);
                //heldPiece = fallingPiece;
                UpdateFallingPiece();
            }
            else
            {
                var temp = (FallingPiece)Activator.CreateInstance(fallingPiece.GetType(), this);
                fallingPiece = heldPiece;
                fallingPiece.Initialize();
                heldPiece = temp;
            }
            foreach (Point point in fallingPiece.Points)
            {
                OccupiedFalling[point.X, point.Y] = true;
            }
            canSwap = false;
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

            UpdateScore(completedRows.Count);
        }

        /// <summary>
        /// Adds the appropriate amount to the score based on the
        /// number of lines cleared and the current level
        /// </summary>
        private void UpdateScore(int linesCleared)
        {
            int multiplier = 0;
            switch (linesCleared)
            {
                case 1:
                    multiplier = 40;
                    break;
                case 2:
                    multiplier = 100;
                    break;
                case 3:
                    multiplier = 300;
                    break;
                case 4:
                    multiplier = 1200;
                    break;
            }

            Score += (lvl + 1) * multiplier;
        }
    }
}
