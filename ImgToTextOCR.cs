using System.Drawing;
using Tesseract;
using System.Runtime.InteropServices;

namespace OCR
{
    class ImgToText
    {
        private static TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);


        public static string TextReader(string imagePath)
        {
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            string text = page.GetText();
            return text;

        }

        public static Point GetWindowCords()
        {
            IntPtr hWnd = NativeMethods.GetForegroundWindow();
            Point windowOffset = new Point { X = 0, Y = 0 };
            NativeMethods.ClientToScreen(hWnd, ref windowOffset);
            return windowOffset;
        }
        public static void TakeScreenshotof(Rectangle box, string filepath)
        {
            //HeaderText(390, 650, 110, 134, "Header.png");
            System.Console.WriteLine("RecBox X= " + box.X);
            System.Console.WriteLine("RecBox Y= " + box.Y);
            System.Console.WriteLine("RecBox Width" + box.Width);
            System.Console.WriteLine("RecBox Heigh" + box.Height);
            Point windowcords = GetWindowCords();

            int width = Math.Abs(box.X - box.Width);
            System.Console.WriteLine(width);
            int height = Math.Abs(box.Y - box.Height);
            System.Console.WriteLine(height);
            box.X += windowcords.X;
            box.Y += windowcords.Y;
            using Bitmap bmp = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(box.X, box.Y, 0, 0, bmp.Size);
            bmp.Save(filepath);
        }

        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);
        }

    }








}