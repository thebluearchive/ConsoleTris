using ConsoleTris.Pieces;
using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTris
{
    public class TitleScreenAnimator : IBoard
    {
        private int blockTimer;
        public BlockType[,] PlacedBlocks { get; private set; }
        private int width;
        private int height;
        private bool keyReceived;
        private Random random = new();
        private CollisionManager collisionManager;
        private FallingPiece[] fallingPieces = new FallingPiece[maxFallingPieces];
        private int fallingPieceIndex = 0;
        private const int maxFallingPieces = 20;
        private const int hiddenRowsAbove = 4;
        private const int hiddenRowsBelow = 4;
        private const string title = "C O N S O L E  T R I S";
        private const string bottomText = "Press any key to begin...";
        private char[,] textArray;

        internal void Initiate()
        {
            width = Console.WindowWidth/2;
            height = Console.WindowHeight;
            PlacedBlocks = new BlockType[width, height + hiddenRowsAbove + hiddenRowsBelow]; // 4 hidden rows above and below
            for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < PlacedBlocks.GetLength(1); j++)
                {
                    PlacedBlocks[i, j] = BlockType.Empty;
                }
            }
            blockTimer = 0;
            keyReceived = false;
            collisionManager = new CollisionManager(this);

            InitiateTextArray();
            StartKeyListener();
            StartAnimationLoop();
        }

        private void InitiateTextArray()
        {
            textArray = new char[Console.WindowWidth, Console.WindowHeight];
            for (int i = 0; i < textArray.GetLength(0); i++)
            {
                for (int j = 0; j < textArray.GetLength(1); j++)
                {
                    textArray[i, j] = ' ';
                }
            }

            int titleXPos = (Console.WindowWidth - title.Length) / 2;
            int titleYPos = Console.WindowHeight / 2;
            for (int i = 0; i < title.Length; i++)
            {
                textArray[titleXPos + i, titleYPos] = title[i];
            }
            for (int i = 0; i < bottomText.Length; i++)
            {
                textArray[i, Console.WindowHeight - 1] = bottomText[i];
            }
        }

        private void StartKeyListener()
        {
            var thread = new Thread(() => {
                while (!keyReceived)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    keyReceived = true;
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }

        private void StartAnimationLoop()
        {
            while (!keyReceived)
            {
                UpdateFallingBlocks();
                Animate();
                Thread.Sleep(10);
            }
            Console.Clear();
        }

        private void UpdateFallingBlocks()
        {
            // Spawn block if applicable
            if (blockTimer == 0)
            {
                SpawnBlock();
            }

            if (blockTimer % 20 == 0)
            {
                foreach (FallingPiece fallingPiece in fallingPieces)
                {
                    if (fallingPiece != null)
                    {
                        fallingPiece.MoveDownNoCollision();
                    }
                }
            }
            // Update board
            // First, set the board to its default state
            for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < PlacedBlocks.GetLength(1); j++)
                {
                    PlacedBlocks[i, j] = BlockType.Empty;
                }
            }
            // Then update any pieces and add them to the correct board position
            foreach (FallingPiece fallingPiece in fallingPieces)
            {
                if (fallingPiece is not null)
                {
                    foreach (Point point in fallingPiece.Points)
                    {
                        PlacedBlocks[point.X, point.Y] = fallingPiece.BlockType;
                    }
                }
            }

            blockTimer = (blockTimer + 1) % 80;
        }

        private void SpawnBlock()
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

            for (int i = 0; i < random.Next(1, width - 1); i++)
            {
                newPiece.MoveRight();
            }
            for (int i = 0; i < random.Next(0, 4); i++)
            {
                newPiece.Rotate();
            }
            fallingPieces[fallingPieceIndex] = newPiece;
            fallingPieceIndex = (fallingPieceIndex + 1) % maxFallingPieces;
        }

        private void Animate()
        {
            for (int j = hiddenRowsAbove; j < PlacedBlocks.GetLength(1) - hiddenRowsBelow; j++)
            {
                StringBuilder sb = new();
                for (int i = 0; i < PlacedBlocks.GetLength(0); i++)
                {
                    string hex = PieceHelpers.GetBlockTypeColor(PlacedBlocks[i, j]);
                    sb.Append(textArray[2 * i, j - hiddenRowsAbove].ToString().PastelBg(hex));
                    sb.Append(textArray[2 * i + 1, j - hiddenRowsAbove].ToString().PastelBg(hex));
                }
                Console.SetCursorPosition(0, j - hiddenRowsAbove);
                Console.Write(sb.ToString());
            }
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
