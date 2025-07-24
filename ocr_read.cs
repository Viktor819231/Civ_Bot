using System;
using System.Drawing;
using System.Diagnostics;

namespace GameChatBot_OCR
{
    class OcrReader
    {
               static void Main(string[] args)
        {
            Console.WriteLine("Taking screenshot...");
            TakeScreenshot("chat.png");

            Console.WriteLine("Opening image...");
            Process.Start("explorer", "chat.png");

            Console.WriteLine("Done.");
        }
        static void TakeScreenshot(string filePath)
        {
            int x = 100, y = 100, width = 500, height = 200;

            using Bitmap bmp = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, bmp.Size);

            bmp.Save(filePath);
        }

    }
}