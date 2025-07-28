using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OCR;





namespace NavigationAndLocations
{

    public interface Icord
    {
        int Cord { get; set; }
    }
    public class LocationInGame
    {
        public int x_left;

        public int y_top;

        public LocationInGame()
        {

        }

        //coordinates relative to 1024x768 aspect ratio
    }
    public class TextBox : LocationInGame
    {
        public int x_right;
        public int y_bottom;
        public string filename;
        public TextBox(int x_l, int x_r, int y_t, int y_b, string filenameArg)
        {
            x_right = Math.Max(x_l, x_r);
            x_left = Math.Min(x_l, x_r);
            y_top = Math.Min(y_b, y_t);
            y_bottom = Math.Max(y_b, y_t);
            filename = filenameArg;

        }
        public Rectangle GetRectanglePictureBox()
        {
            Rectangle picbox = new Rectangle(x: this.x_left, y: this.y_top, width: this.x_right, height: this.y_bottom);
            return picbox;
        }
        public static TextBox HeaderText = new TextBox(390, 650, 110, 134, "HeaderSS.png");
        public static TextBox MenuText = new TextBox(445, 500, 240, 270, "ManuSS.png");
        public static TextBox ChatText = new TextBox(50, 410, 660, 640, "ChatSS.png");
    }
    public class Button : LocationInGame
    {
        public Button(int x1, int y1)
        {
            x_left = x1;
            y_top = y1;
        }
        public static Button Button_AmericaLeaderChoice = new Button(350, 600);
        public static Button Button_LeaderChoice = new Button(350, 265);
        public static Button Button_LeaderChoiceScroll = new Button(430, 635);
        public static Button Button_DifficultyBox = new Button(500, 215);
        public static Button Button_DifficultyEmperor = new Button(500, 365);
        public static Button Button_LobbyNameInputField = new Button(410, 195);
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
            x_left = x1;
            y_top = y1;
            PreviousScreen = previous;
            ButtonToPress = button;

        }
        public static ScreenLocation Screen_NullLobby = new ScreenLocation(0, 0, null, Button.button_Backtrack);
        public static ScreenLocation ScreenLocation_error = new ScreenLocation(0, 0, Screen_NullLobby, Button.button_Backtrack);
        public static ScreenLocation ScreenMenu_Main = new ScreenLocation(0, 0, Screen_NullLobby, Button.button_Backtrack);
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

    class Navigation
    {
        public static ScreenLocation GetCurrentScreen()
        {
            try
            {
                return GetHeaderBasedLocations();
            }
            catch 
            {
                try
                {
                    return GetMenuBasedLocations();
                }
                catch
                {
                    System.Console.WriteLine("Cant identify area, will sleep 5 seconds and try again");
                    System.Console.WriteLine("Make sure you have Civ5 in focus on 1024x768");
                    Thread.Sleep(5000);
                    return GetCurrentScreen();
                }

            }
        }
        public static ScreenLocation GetMenuBasedLocations()
        {
            string Menutxt = TextAt(TextBox.MenuText);

                string FirstTwoLetters = Menutxt.Substring(0, 2);
                switch (FirstTwoLetters)
                {
                    case "SN":
                        return ScreenLocation.ScreenMenu_Main;
                    case "ST":
                        return ScreenLocation.ScreenMenu_HotOrStandard;
                    case "IN":
                        return ScreenLocation.ScreenMenu_InternetOrLocal;
                    default:
                        throw new Exception();
                }
        }

        public static ScreenLocation GetHeaderBasedLocations()
        {

            string txt = TextAt(TextBox.HeaderText);
            switch (txt)
            {
                case "INTERNET GAMES":
                    return ScreenLocation.Screen_InternetLobbies;
                case "SETUP MULTIPLAYER GAME":
                    return ScreenLocation.Screen_SetupMulti;
                case "LOAD GAME":
                    return ScreenLocation.Screen_LoadGames1;
                case "STAGING ROOM":
                    return ScreenLocation.Screen_StagingRoom;
                default:
                    throw new Exception("No match on HeaderBasedLocations");
                }
            

        }
        public static string TextAt(TextBox location)
        {
            ImgToText.TakeScreenshotof(TextBox.MenuText.GetRectanglePictureBox(), TextBox.MenuText.filename);
            string results = ImgToText.TextReader(TextBox.MenuText.filename);
            return results;
        }




    }




}