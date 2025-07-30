using System;
using System.Drawing;
using System.Runtime.InteropServices;
using OCR;





namespace NavigationAndLocations
{

    public class LocationInGame
    {
        public int x_left;

        public int y_top;

        public LocationInGame()
        {

        }

        
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
        public static CivTextBox ChatText = new CivTextBox(50, 410, 663, 638, "ChatSS.png");
    }
    public class CivButton : LocationInGame
    {
        public CivButton(int x1, int y1)
        {
            x_left = x1;
            y_top = y1;
        }
        public static CivButton AmericaLeaderChoice = new CivButton(350, 600);
        public static CivButton LeaderChoice = new CivButton(350, 265);
        public static CivButton LeaderChoiceScroll = new CivButton(430, 635);
        public static CivButton DifficultyBox = new CivButton(500, 215);
        public static CivButton DifficultyEmperor = new CivButton(500, 365);
        public static CivButton LobbyNameInputField = new CivButton(410, 195);
        public static CivButton Backtrack = new CivButton(0, 0);
        public static CivButton Multiplayer = new CivButton(510, 340);
        public static CivButton Internet = new CivButton(510, 280);
        public static CivButton Standard = new CivButton(510, 280);
        public static CivButton HostLobby = new CivButton(840, 785);
        public static CivButton Loadgame = new CivButton(430, 785);
        public static CivButton GameConfigfile = new CivButton(714, 278);
        public static CivButton Loadgame_hostgame = new CivButton(840, 720);
        public static CivButton Chatinput = new CivButton(200, 713);
        public static bool IsEqual(CivButton one, CivButton two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }
    public class Location : LocationInGame
    {
        public Location PreviousScreen;
        public CivButton ButtonToPress;
        public Location(int x1, int y1, Location previous, CivButton button)
        {
            x_left = x1;
            y_top = y1;
            PreviousScreen = previous;
            ButtonToPress = button;

        }
        public static Location Screen_NullLobby = new Location(0, 0, null, CivButton.Backtrack);
        public static Location ScreenLocation_error = new Location(0, 0, Screen_NullLobby, CivButton.Backtrack);
        public static Location ScreenMenu_Main = new Location(0, 0, Screen_NullLobby, CivButton.Backtrack);
        public static Location ScreenMenu_HotOrStandard = new Location(0, 0, Location.ScreenMenu_Main, CivButton.Multiplayer);
        public static Location ScreenMenu_InternetOrLocal = new Location(0, 0, Location.ScreenMenu_HotOrStandard, CivButton.Standard);
        public static Location Screen_InternetLobbies = new Location(0, 0, ScreenMenu_InternetOrLocal, CivButton.Internet);
        public static Location Screen_SetupMulti = new Location(0, 0, Screen_InternetLobbies, CivButton.HostLobby);
        public static Location Screen_LoadGames1 = new Location(0, 0, Screen_SetupMulti, CivButton.Loadgame);
        public static Location Screen_LoadGames2 = new Location(0, 0, Screen_LoadGames1, CivButton.GameConfigfile);
        public static Location Screen_StagingRoom = new Location(0, 0, Screen_LoadGames2, CivButton.Loadgame_hostgame);

        public static bool IsEqual(Location one, Location two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }





}