using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Pastel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleTris
{
    internal class GameManager
    {
        private static Queue<ConsoleKeyInfo> controlQueue = new();
        private static bool gameOngoing = true;
        private Board board;
        private static Playback _playback;
        private const int leftBorder = 0;
        private const int rightBorder = 14;
        private const int topBorder = 1;
        private const int bottomBorder = 1;
        private TitleScreenAnimator titleScreenAnimator = new();
        
        public void Run()
        {
            SetupConsole();
            Console.WriteLine("Loading...");
            PlayMidi();
            titleScreenAnimator.Initiate();
            while (true)
            {
                board = new();
                gameOngoing = true;
                StartKeyboardListener();
                ExecuteGameLoop();
                titleScreenAnimator.Initiate();
            }
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
            controlQueue = new Queue<ConsoleKeyInfo>();
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
            string gameOverText = "Game Over";
            Console.SetCursorPosition((Console.WindowWidth - gameOverText.Length)/ 2, Console.WindowHeight / 2);
            Console.WriteLine(gameOverText.Pastel("FF0000"));
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
