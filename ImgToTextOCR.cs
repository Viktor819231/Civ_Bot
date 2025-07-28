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
            Point windowcords = GetWindowCords();
            box.X += windowcords.X;
            box.Y += windowcords.Y;
            int width = box.X - box.Width;
            int height = box.Y - box.Height;
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