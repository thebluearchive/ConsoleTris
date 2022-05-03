using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleTris
{
    internal class GameManager
    {
        private static Queue<ConsoleKeyInfo> controlQueue = new();
        private static bool gameOngoing = true;
        private readonly Board board = new();
        private static Playback _playback;

        public void Run()
        {
            SetupConsole();
            DrawTitleScreen();
            PlayMidi();
            StartKeyboardListener();
            ExecuteGameLoop();
        }
        
        private void DrawTitleScreen()
        {
            Console.WriteLine(@"/------------------\");
            for (int i = 0; i < board.HEIGHT / 2 - 2; i++)
            {
                Console.WriteLine("|------------------|");
            }
            Console.WriteLine(@"|~  CONSOLE-TRIS  ~|");
            for (int i = 0; i < board.HEIGHT / 2 - 3; i++)
            {
                Console.WriteLine("|------------------|");
            }
            Console.WriteLine(@"\__________________/");
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
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
                if (controlQueue.TryDequeue(out ConsoleKeyInfo keyInfo))
                {
                    board.HandleUserInput(keyInfo);
                }
                Thread.Sleep(10);
            }
        }

        private void SetupConsole()
        {
            Console.CursorVisible = false;
            Console.WindowWidth = board.WIDTH * 2;
            Console.WindowHeight = board.HEIGHT;
            Console.SetBufferSize(board.WIDTH * 2, board.HEIGHT);
            Console.Clear();
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
