using System;
using System.Drawing;
using Tesseract;
using System.Runtime.InteropServices;



namespace OCR_logic
{


    public class LocationInGame
    {
        public int x;

        public int y;

        public LocationInGame()
        {

        }

        //coordinates relative to 1024x768 aspect ratio
    }
    public class TextBox : LocationInGame
    {
        public int x_right;
        public int y_bottom;
        public TextBox(int x_l, int x_r, int y_t, int y_b)
        {
            x_right = Math.Max(x_l, x_r);
            x = Math.Min(x_l, x_r);
            y = Math.Min(y_b, y_t);
            y_bottom = Math.Max(y_b, y_t);

        }
        public static TextBox Location_header = new TextBox(390, 650, 110, 134);
        public static TextBox Location_Menu = new TextBox(445, 500, 240, 270);
        public static TextBox Chat = new TextBox(50, 410, 660, 640);
    }
    public class Button : LocationInGame
    {
        public Button(int x1, int y1)
        {
            x = x1;
            y = y1;
        }
        public static Button Button_LobbyNameInputField = new Button(300, 195);
        public static Button button_Backtrack = new Button(0, 0);
        public static Button Button_Multiplayer = new Button(510, 340);
        public static Button Button_Internet = new Button(510, 280);
        public static Button Button_Standard = new Button(510, 280);
        public static Button Button_HostLobby = new Button(840, 785);
        public static Button Button_Loadgame = new Button(430, 785);
        public static Button Button_GameConfigfile = new Button(714, 278);
        public static Button Button_Loadgame_hostgame = new Button(840, 720);
        public static Button Button_Chatinput = new Button(200, 713);
        public static bool IsEqual(Button one, Button two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }
    public class ScreenLocation : LocationInGame
    {
        public ScreenLocation PreviousScreen;
        public Button ButtonToPress;
        public ScreenLocation(int x1, int y1, ScreenLocation previous, Button button)
        {
            x = x1;
            y = y1;
            PreviousScreen = previous;
            ButtonToPress = button;

        }
        public static ScreenLocation Screen_PreMainlobby = new ScreenLocation(0, 0, null, Button.button_Backtrack);
        public static ScreenLocation ScreenMenu_Main = new ScreenLocation(0, 0, Screen_PreMainlobby, Button.button_Backtrack);
        public static ScreenLocation ScreenMenu_HotOrStandard = new ScreenLocation(0, 0, ScreenLocation.ScreenMenu_Main, Button.Button_Multiplayer);
        public static ScreenLocation ScreenMenu_InternetOrLocal = new ScreenLocation(0, 0, ScreenLocation.ScreenMenu_HotOrStandard, Button.Button_Standard);
        public static ScreenLocation Screen_InternetLobbies = new ScreenLocation(0, 0, ScreenMenu_InternetOrLocal, Button.Button_Internet);
        public static ScreenLocation Screen_SetupMulti = new ScreenLocation(0, 0, Screen_InternetLobbies, Button.Button_HostLobby);

        public static ScreenLocation Screen_LoadGames1 = new ScreenLocation(0, 0, Screen_SetupMulti, Button.Button_Loadgame);
        public static ScreenLocation Screen_LoadGames2 = new ScreenLocation(0, 0, Screen_LoadGames1, Button.Button_GameConfigfile);
        public static ScreenLocation Screen_StagingRoom = new ScreenLocation(0, 0, Screen_LoadGames2, Button.Button_Loadgame_hostgame);

        public static bool IsEqual(ScreenLocation one, ScreenLocation two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }
    class OcrReader
    {
        public static string filepath_HeaderSS = "HeaderLocation.png";
        public static string filepath_MenuSS = "MenuLocation.png";
        private static TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

        public static string TextReader(TextBox location)
        {
            string imagePath = Getfilelocation(location);
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            string text = page.GetText();
            return text;

        }
        public static void TakeScreenshotof(TextBox Location)
        {
            LocationInGame windowcords = GetWindowCords();
            int x = windowcords.x + Location.x;
            int y = windowcords.y + Location.y;
            int width = Location.x_right - Location.x;
            int height = Location.y_bottom - Location.y;

            string filepath = Getfilelocation(Location);
            using Bitmap bmp = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            bmp.Save(filepath);
        }
        public static LocationInGame GetWindowCords()
        {
            IntPtr hWnd = NativeMethods.GetForegroundWindow();
            POINT windowOffset = new POINT { X = 0, Y = 0 };
            NativeMethods.ClientToScreen(hWnd, ref windowOffset);
            Button WindowCords = new Button(windowOffset.X, windowOffset.Y);
            return WindowCords;
        }
        public static string Getfilelocation(TextBox location)
        {
            string filepath = "temp.png";
            if (location == TextBox.Location_Menu)
            {
                filepath = OcrReader.filepath_MenuSS;
            }
            else if (location == TextBox.Location_header)
            {
                filepath = OcrReader.filepath_HeaderSS;
            }
            return filepath;
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