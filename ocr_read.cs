using System;
using System.Drawing;
using System.Diagnostics;
using Tesseract;
using System.Runtime.CompilerServices;
using System.Configuration.Assemblies;
using System.Dynamic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;


namespace OCR_logic
{
    class ChatMessage
    {
        private string msg;
        private string player;

        public ChatMessage(string text, string playername)
        {
            this.msg = text;
            this.player = playername;
        }
        public string GetMsg()
        {
            return msg;
        }

        public string GetPlayer()
        {
            return player;
        }

        public void replacelatest_msg(ChatMessage message)
        {
            if (Verify_not_bot_msg(message) && Verify_Unique_Msg(message))
            {
                this.msg = message.msg;
                this.player = message.player;
            }
        }

        public bool Verify_not_bot_msg(ChatMessage message)
        {
            if (message.player != "Host_placeholder_name")
            {
                return true;
            }
            return false;
        }
        public bool Verify_Unique_Msg(ChatMessage message)
        {
            if (message.player != "Host_placeholder_name" && message.msg != this.msg)
            {
                return true;
            }
            return false;
        }

        public bool Verify_not_null(string text)
        {
            if (text != null)
            {
                return true;
            }
            return false;

        }
    }

    class Coordinates
    {
        public int x;
        public int x_right;
        public int y;
        public int y_bottom;
        public Coordinates(int x_l, int x_r, int y_t, int y_b)
        {
            x_right = Math.Max(x_l, x_r);
            x = Math.Min(x_l, x_r);
            y = Math.Min(y_b, y_t);
            y_bottom = Math.Max(y_b, y_t);

        }
        public Coordinates(int x1, int y1)
        {
            x = x1;
            y = y1;
        }

        public Coordinates()
        {

        }
        public static Coordinates Multiplayer = new Coordinates(510, 340);
        public static Coordinates Internet = new Coordinates(510, 280);
        public static Coordinates Standard = new Coordinates(510, 280);
        public static Coordinates HostLobby = new Coordinates(840, 785);
        public static Coordinates Loadgame = new Coordinates(430, 785);
        public static Coordinates GameConfigfile = new Coordinates(714, 278);
        public static Coordinates Loadgame_hostgame = new Coordinates(840, 720);
        public static Coordinates Chatinput = new Coordinates(200, 713);




    }
    class OcrReader
    {
        public Coordinates Location_header = new Coordinates(390, 500, 110, 130);
        public Coordinates Location_Menu = new Coordinates(445, 500, 240, 270);
        public Coordinates Chat = new Coordinates(50, 410, 660, 640);
  



        private static ChatMessage lastest_msg = new ChatMessage("xxx", "xxx");
        private static TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        
        static void Locate()
        {

        }
        public static string OCR_reading(string imagePath)
        {

            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            string text = page.GetText();
            return text;

        }
        public void TakeWindowRelativeScreenshot(string filePath, Coordinates picture_coordinates)
        {
            IntPtr hWnd = NativeMethods.GetForegroundWindow();
            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("No active window.");
                return;
            }
            POINT windowOffset = new POINT { X = 0, Y = 0 };
            NativeMethods.ClientToScreen(hWnd, ref windowOffset);
            int x = windowOffset.X + picture_coordinates.x;
            int y = windowOffset.Y + picture_coordinates.y;
            int width = picture_coordinates.x_right - picture_coordinates.x;
            int height = picture_coordinates.y_bottom - picture_coordinates.y;
            using Bitmap bmp = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            bmp.Save(filePath);
        }


    }

    public struct POINT
    {
        public int X;
        public int Y;
    }

    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);
    }
}