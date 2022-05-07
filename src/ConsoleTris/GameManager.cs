using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleTris
{
    internal class GameManager
    {
        private static Queue<ConsoleKeyInfo> controlQueue;
        private static bool gameOngoing = true;
        private Board board;
        private static Playback _playback;
        private const int leftBorder = 0;
        private const int rightBorder = 14;
        private const int topBorder = 1;
        private const int bottomBorder = 1;
        
        public void Run()
        {
            while (true)
            {
                board = new();
                SetupConsole();
                gameOngoing = true;
                DrawTitleScreen();
                //PlayMidi();
                StartKeyboardListener();
                ExecuteGameLoop();
            }
        }
        
        private void DrawTitleScreen()
        {
            Console.Clear();
            Console.WriteLine(@" __________________ ");
            Console.WriteLine(@"/                  \");
            for (int i = 0; i < Board.HEIGHT / 2 - 3; i++)
            {
                Console.WriteLine("|------------------|");
            }
            Console.WriteLine(@"|~  CONSOLE-TRIS  ~|");
            for (int i = 0; i < Board.HEIGHT / 2 - 3; i++)
            {
                Console.WriteLine("|------------------|");
            }
            Console.WriteLine(@"\__________________/");
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
            Console.Clear();
        }

        private void PlayMidi()
        {
            var midiFile = MidiFile.Read(@"res\consoletris.mid");
            var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
            _playback = midiFile.GetPlayback(outputDevice);
            _playback.Loop = true;
            _playback.Start();
        }

        private void ExecuteGameLoop()
        {
            while (gameOngoing)
            {
                board.UpdateState();
                board.Draw();
                gameOngoing = !board.IsLoss;
                if (controlQueue.TryDequeue(out ConsoleKeyInfo keyInfo))
                {
                    board.HandleUserInput(keyInfo);
                }
                Thread.Sleep(10);
            }
            DisplayLosingScreen();
        }

        private void DisplayLosingScreen()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Game Over. Final Score: {board.Score}");
            // Prevent the user from accidentally skipping over this screen
            // for the next two seconds
            Thread.Sleep(2000);
            Console.ReadKey(true);
        }

        private void SetupConsole()
        {
            Console.Clear();
            Console.WindowWidth = Board.WIDTH * 2 + leftBorder + rightBorder;
            Console.WindowHeight = Board.HEIGHT + bottomBorder + topBorder;
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            Console.CursorVisible = false;
        }

        private static void StartKeyboardListener()
        {
            controlQueue = new();
            var thread = new Thread(() => {
                while (gameOngoing)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    controlQueue.Enqueue(key);
                }
            });

            thread.IsBackground = true;
            thread.Start();
        }
    }
}
