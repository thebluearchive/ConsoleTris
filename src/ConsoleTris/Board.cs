using Pastel;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleTris.Pieces;
using System.Diagnostics;

namespace ConsoleTris
{
    public class Board : IBoard
    {
        internal const int WIDTH = 10;
        internal const int HEIGHT = 20;
        internal Random random = new();
        internal int Score { get; private set; } = 0;
        /// <summary>
        /// Level is incremented each time 10 rows are removed.
        /// Blocks fall faster until level 10, when the speed of blocks
        /// falling caps out.
        /// </summary>
        private int lvl = 0;
        private bool displayFPS = false;
        internal bool IsLoss = false;
        private bool canSwap = false;
        private readonly CollisionManager collisionManager;
        private int totalRowsCleared = 0;
        
        private readonly Stopwatch stopWatch = new();
        public BlockType[,] PlacedBlocks { get; private set; }

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

        public Board()
        {
            // The actual height is 2 blocks taller than the visible viewport
            PlacedBlocks = new BlockType[WIDTH, HEIGHT + 2];
            for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < PlacedBlocks.GetLength(1); j++)
                {
                    PlacedBlocks[i, j] = BlockType.Empty;
                }
            }
            collisionManager = new CollisionManager(this);

            stopWatch.Start();
        }

        public void Draw()
        {
            DrawBoard(0, 0);

            // Draw current score and lvl
            DrawScoreAndLevel(2 * WIDTH + 2, 0);

            // Draw Next Piece
            // TODO: don't hardcode next piece display height and width?
            DrawNextPiece(2 * WIDTH + 2, 3);

            // Draw held piece
            DrawHeldPiece(2 * WIDTH + 2, HEIGHT - 3);


            // number of milliseconds a tick has taken on average
            DrawFPS();

            stopWatch.Reset();
            stopWatch.Start();
        }

        private void DrawFPS()
        {
            double averageTick = FPSCalc.CalcAverageTick(stopWatch.ElapsedMilliseconds);
            double fps = 1000 / averageTick;
            if (displayFPS)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write($"FPS = {fps:F2}");
            }
        }

        /// <summary>
        /// Draws the game board at the specified position. Includes the game border
        /// </summary>
        private void DrawBoard(int xPos, int yPos)
        {
            Point[] projection = fallingPiece.GetProjection();

            // Draw top border
            DrawTopBorder(WIDTH + 2, xPos, yPos);
            
            // Draw Board
            for (int j = 2; j < PlacedBlocks.GetLength(1); j++) //start at j = 2 because the first two rows of the board array are not displayed
            {
                StringBuilder sb = new();
                sb.Append('|');
                for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
                {
                    string hex;
                    if (Array.Exists(fallingPiece.Points, p => p.X == i && p.Y == j))
                        hex = PieceHelpers.GetBlockTypeColor(fallingPiece.BlockType);
                    else if (Array.Exists(projection, p => p.X == i && p.Y == j))
                        hex = "#222222";
                    else
                        hex = PieceHelpers.GetBlockTypeColor(PlacedBlocks[i, j]);
                    sb.Append("  ".PastelBg(hex));
                }
                sb.Append('|');
                Console.SetCursorPosition(xPos, yPos + j - 1);
                Console.Write(sb.ToString());
            }

            // Draw bottom border
            DrawBottomBorder(WIDTH + 2, xPos, yPos + HEIGHT + 1);
        }

        private void DrawNextPiece(int xPos, int yPos)
        {
            int width = 5;
            int height = 4;

            DrawTopBorder(width + 2, xPos, yPos, "Next:");
            bool[,] nextPieceDisplay = new bool[width, height];
            foreach (Point point in nextPiece.Points)
            {
                nextPieceDisplay[point.X, point.Y] = true;
            }
            for (int j = 0; j < height - 1; j++)
            {
                StringBuilder sb = new();
                sb.Append('|');
                Console.SetCursorPosition(xPos, yPos + j + 1);
                for (int i = 0; i < width; i++)
                {
                    string hex;
                    if (nextPieceDisplay[i, j])
                    {
                        hex = PieceHelpers.GetBlockTypeColor(nextPiece.BlockType);
                    }
                    else
                    {
                        hex = "#000000";
                    }
                    sb.Append("  ".PastelBg(hex));
                }
                sb.Append('|');
                Console.Write(sb.ToString());
            }

            DrawBottomBorder(width + 2, xPos, yPos + height);
        }

        private void DrawHeldPiece(int xPos, int yPos)
        {
            // Draw currently held piece
            int width = 5;
            int height = 4;
            DrawTopBorder(width + 2, xPos, yPos, "Held:");
            bool[,] heldPieceDisplay = new bool[width, height];
            if (heldPiece is not null)
            {
                foreach (Point point in heldPiece.Points)
                {
                    heldPieceDisplay[point.X, point.Y] = true;
                }
            }
            for (int j = 0; j < height - 1; j++)
            {
                StringBuilder sb = new();
                sb.Append('|');
                Console.SetCursorPosition(xPos, j + yPos + 1);
                for (int i = 0; i < width; i++)
                {
                    string hex;
                    if (heldPieceDisplay[i, j])
                    {
                        hex = PieceHelpers.GetBlockTypeColor(heldPiece.BlockType);
                    }
                    else
                    {
                        hex = "#000000";
                    }
                    sb.Append("  ".PastelBg(hex));
                }
                sb.Append('|');
                Console.Write(sb.ToString());
            }

            DrawBottomBorder(width + 2, xPos, yPos + height);
        }

        private void DrawScoreAndLevel(int xPos, int yPos)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write($"Level {lvl}");
            Console.SetCursorPosition(xPos, yPos + 1);
            Console.Write($"Score: ");
            // Wrap the score if necessary
            if (Score.ToString().Length + Console.GetCursorPosition().Left > Console.WindowWidth)
            {
                Console.SetCursorPosition(xPos + 1, yPos + 2);
            }

            Console.Write($"{Score}");
        }

        private void DrawTopBorder(int borderWidth, int xPos, int yPos, string headerText = "")
        {
            StringBuilder sb = new();
            sb.Append('┌');
            
            for (int i = 0; i < borderWidth - 2; i++)
            {
                sb.Append("--");
            }
            sb.Append('┐');
            sb.Remove(0, headerText.Length);
            sb.Insert(0, headerText);
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(sb.ToString());
        }

        private void DrawBottomBorder(int borderWidth, int xPos, int yPos)
        {
            StringBuilder sb = new();
            sb.Append('└');
            for (int i = 0; i < borderWidth - 2; i++) // Adding two to account for the border
            {
                sb.Append("--");
            }
            sb.Append('┘');
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(sb.ToString());
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
            fallTimer = (fallTimer + 1) % Math.Max(50 - lvl * 4, 10);
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
            canSwap = false;
        }

        private void ClearRows()
        {
            HashSet<int> completedRows = new();
            for (int j = PlacedBlocks.GetLength(1) - 1; j >= 0; j--)
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

            int currentRow = PlacedBlocks.GetLength(1) - 1;
            for (int j = PlacedBlocks.GetLength(1) - 1; j >= 0; j--)
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
                for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
                {
                    PlacedBlocks[i, currentRow] = BlockType.Empty;
                }
            }

            totalRowsCleared += completedRows.Count;
            lvl = totalRowsCleared / 10;
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

        public bool IsValidPlacement(Point point)
        {
            return collisionManager.IsValidPlacement(point);
        }
        public bool IsValidPlacement(Point[] points)
        {
            return collisionManager.IsValidPlacement(points);
        }

        public bool IsInBounds(Point points)
        {
            return collisionManager.IsInBounds(points);
        }
    }
}
