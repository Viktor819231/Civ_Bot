using Tesseract;
using System.Runtime.InteropServices;

//Classes that handle the OCR, will take filepaths or coordinates and return picutres
namespace OCR
{
    class ImgToText
    {
        private static string FindTessDataPath()
        {
            string outputPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
            if (Directory.Exists(outputPath))
                return outputPath;

            string projectPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "tessdata");
            if (Directory.Exists(projectPath))
                return Path.GetFullPath(projectPath);

            throw new DirectoryNotFoundException("Cannot find tessdata directory");
        }

        private static TesseractEngine engine = new TesseractEngine(
            FindTessDataPath(),
            "eng",
            EngineMode.Default
        );

        public static string TextAt(Rectangle location,string filename)
        {
            string absolutePath = Path.Combine(AppContext.BaseDirectory, filename);
            ImgToText.TakeScreenshotof(location, absolutePath);
            string results = ImgToText.TextReader(absolutePath);
            return results;
        }

        public static string TextReader(string imagePath)
        {
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            string text = page.GetText();
            return text;
        }

        public static Point GetWindowCords()
        {
            IntPtr hWnd = Windowsmethods.GetForegroundWindow();
            Point windowOffset = new Point { X = 0, Y = 0 };
            Windowsmethods.ClientToScreen(hWnd, ref windowOffset);
            return windowOffset;
        }
        public static void TakeScreenshotof(Rectangle box, string filepath)
        {
            
            try
            {
                Point windowcords = GetWindowCords();
                int width = Math.Abs(box.X - box.Width);
                int height = Math.Abs(box.Y - box.Height);
                box.X += windowcords.X;
                box.Y += windowcords.Y;

                using Bitmap bmp = new Bitmap(width, height);
                using Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(box.X, box.Y, 0, 0, bmp.Size);
                string? directory = Path.GetDirectoryName(filepath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                bmp.Save(filepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Screenshot error: {ex.Message}");
            }

            
        }
        public static int getheaderheight()
        {
            IntPtr hWnd = Windowsmethods.GetForegroundWindow();
            Windowsmethods.RECT rectWithHeader;
            Windowsmethods.RECT rectWithoutHeader;
            Windowsmethods.GetWindowRect(hWnd, out rectWithHeader);
            Windowsmethods.GetClientRect(hWnd, out rectWithoutHeader);
            int headerHeight = (rectWithHeader.Bottom - rectWithHeader.Top) - (rectWithoutHeader.Bottom - rectWithoutHeader.Top);
            return headerHeight;
        }
        public static class Windowsmethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);

            [DllImport("user32.dll")]
            public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
            
            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;  
        public int Bottom; 
    }

        }

        

    }








}