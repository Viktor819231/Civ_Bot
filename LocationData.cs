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
    public class CivTextBox : LocationInGame
    {
        public int x_right;
        public int y_bottom;
        public string filename;
        public CivTextBox(int x_l, int x_r, int y_t, int y_b, string filenameArg)
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
        public static CivTextBox HeaderText = new CivTextBox(390, 650, 110, 134, "Header.png");
        public static CivTextBox MenuText = new CivTextBox(390, 640, 230, 275, "ManuSS.png");
        public static CivTextBox ChatText = new CivTextBox(50, 410, 660, 640, "ChatSS.png");
    }
    public class CivButton : LocationInGame
    {
        public CivButton(int x1, int y1)
        {
            x_left = x1;
            y_top = y1;
        }
        public static CivButton Button_AmericaLeaderChoice = new CivButton(350, 600);
        public static CivButton Button_LeaderChoice = new CivButton(350, 265);
        public static CivButton Button_LeaderChoiceScroll = new CivButton(430, 635);
        public static CivButton Button_DifficultyBox = new CivButton(500, 215);
        public static CivButton Button_DifficultyEmperor = new CivButton(500, 365);
        public static CivButton Button_LobbyNameInputField = new CivButton(410, 195);
        public static CivButton button_Backtrack = new CivButton(0, 0);
        public static CivButton Button_Multiplayer = new CivButton(510, 340);
        public static CivButton Button_Internet = new CivButton(510, 280);
        public static CivButton Button_Standard = new CivButton(510, 280);
        public static CivButton Button_HostLobby = new CivButton(840, 785);
        public static CivButton Button_Loadgame = new CivButton(430, 785);
        public static CivButton Button_GameConfigfile = new CivButton(714, 278);
        public static CivButton Button_Loadgame_hostgame = new CivButton(840, 720);
        public static CivButton Button_Chatinput = new CivButton(200, 713);
        public static bool IsEqual(CivButton one, CivButton two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }
    public class CivScreenLocation : LocationInGame
    {
        public CivScreenLocation PreviousScreen;
        public CivButton ButtonToPress;
        public CivScreenLocation(int x1, int y1, CivScreenLocation previous, CivButton button)
        {
            x_left = x1;
            y_top = y1;
            PreviousScreen = previous;
            ButtonToPress = button;

        }
        public static CivScreenLocation Screen_NullLobby = new CivScreenLocation(0, 0, null, CivButton.button_Backtrack);
        public static CivScreenLocation ScreenLocation_error = new CivScreenLocation(0, 0, Screen_NullLobby, CivButton.button_Backtrack);
        public static CivScreenLocation ScreenMenu_Main = new CivScreenLocation(0, 0, Screen_NullLobby, CivButton.button_Backtrack);
        public static CivScreenLocation ScreenMenu_HotOrStandard = new CivScreenLocation(0, 0, CivScreenLocation.ScreenMenu_Main, CivButton.Button_Multiplayer);
        public static CivScreenLocation ScreenMenu_InternetOrLocal = new CivScreenLocation(0, 0, CivScreenLocation.ScreenMenu_HotOrStandard, CivButton.Button_Standard);
        public static CivScreenLocation Screen_InternetLobbies = new CivScreenLocation(0, 0, ScreenMenu_InternetOrLocal, CivButton.Button_Internet);
        public static CivScreenLocation Screen_SetupMulti = new CivScreenLocation(0, 0, Screen_InternetLobbies, CivButton.Button_HostLobby);
        public static CivScreenLocation Screen_LoadGames1 = new CivScreenLocation(0, 0, Screen_SetupMulti, CivButton.Button_Loadgame);
        public static CivScreenLocation Screen_LoadGames2 = new CivScreenLocation(0, 0, Screen_LoadGames1, CivButton.Button_GameConfigfile);
        public static CivScreenLocation Screen_StagingRoom = new CivScreenLocation(0, 0, Screen_LoadGames2, CivButton.Button_Loadgame_hostgame);

        public static bool IsEqual(CivScreenLocation one, CivScreenLocation two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }





}