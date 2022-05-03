using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTris
{
    class Program
    {
        static void Main(string[] args)
        {
            //Image<Rgba32> image = Image.Load<Rgba32>(@"res\test.png");
            //ImagePrinter.Print(image);
            //Console.ReadKey();

            GameManager gameManager = new();
            gameManager.Run();
        }
    }
}
