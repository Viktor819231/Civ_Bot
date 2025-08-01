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
        public static CivTextBox SecondMenuText = new CivTextBox(390, 640, 280, 325, "secondManuSS.png");
        public static CivTextBox ChatText = new CivTextBox(50, 410, 663, 638, "ChatSS.png");
    }
    public class CivButton : LocationInGame
    {
        public ScreenLocation requiredscreen { get; set; }
        public CivButton(int x1, int y1)
        {
            x_left = x1;
            y_top = y1;
        }
        public static CivButton outoftheway = new CivButton(50, 50);
        public static CivButton AmericaLeaderChoice = new CivButton(350, 600);
        public static CivButton LeaderChoice = new CivButton(350, 265);
        public static CivButton LeaderChoiceScroll = new CivButton(430, 635);
        public static CivButton DifficultyBox = new CivButton(500, 215);
        public static CivButton DifficultyEmperor = new CivButton(500, 365);
        public static CivButton LobbyNameInputField = new CivButton(410, 195);
        public static CivButton Backtrack = new CivButton(0, 0);
        public static CivButton MenuMultiplayer = new CivButton(x1: 600, 340);
        public static CivButton MenuInternet = new CivButton(600, 280);
        public static CivButton MenuStandard = new CivButton(600, 280);
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
    public class ScreenLocation : LocationInGame
    {
        public ScreenLocation PreviousScreen;
        public CivButton ButtonToPress_PreviousScreen;
        public List<CivButton> AvailableButtons = new List<CivButton>();
        public ScreenLocation(int x1, int y1, ScreenLocation previous, CivButton button,List<CivButton> Buttons)
        {
            x_left = x1;
            y_top = y1;
            PreviousScreen = previous;
            ButtonToPress_PreviousScreen = button;
            AvailableButtons = Buttons;

        }
        public static ScreenLocation NullLobby = new ScreenLocation(0, 0, null, CivButton.Backtrack,null);
        public static ScreenLocation Location_error = new ScreenLocation(0, 0, NullLobby, CivButton.Backtrack,
               new List<CivButton>
            {
                CivButton.Backtrack
            }
        );
        public static ScreenLocation Menu_Main = new ScreenLocation(0, 0, NullLobby, CivButton.Backtrack,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.MenuMultiplayer
            }
        );
        public static ScreenLocation Menu_HotOrStandard = new ScreenLocation(0, 0, Menu_Main, CivButton.MenuMultiplayer,
                new List<CivButton>
            {
                CivButton.Backtrack,
                CivButton.MenuStandard
            }
        );
        public static ScreenLocation Menu_InternetOrLocal = new ScreenLocation(0, 0, Menu_HotOrStandard, CivButton.MenuStandard,
                new List<CivButton>
            {
                CivButton.Backtrack,
                CivButton.MenuInternet
            }
        );
        public static ScreenLocation InternetLobbies = new ScreenLocation(0, 0, Menu_InternetOrLocal, CivButton.MenuInternet,
                new List<CivButton>
            {
                CivButton.Backtrack,
                CivButton.HostLobby
            }
        );
        public static ScreenLocation SetupMulti = new ScreenLocation(0, 0, InternetLobbies, CivButton.HostLobby,
                new List<CivButton>
            {
                CivButton.Backtrack
                ,CivButton.LobbyNameInputField,
                CivButton.Loadgame,
                CivButton.HostLobby
            }
        );
        public static ScreenLocation LoadGames1 = new ScreenLocation(0, 0, SetupMulti, CivButton.Loadgame,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.GameConfigfile,
               CivButton.Loadgame_hostgame
            }
        );
        public static ScreenLocation LoadGames2 = new ScreenLocation(0, 0, LoadGames1, CivButton.GameConfigfile,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.GameConfigfile,
               CivButton.Loadgame_hostgame
            }
        );
        public static ScreenLocation StagingRoom = new ScreenLocation(0, 0, LoadGames2, CivButton.Loadgame_hostgame,
                new List<CivButton>
            {
                CivButton.AmericaLeaderChoice,
                CivButton.LeaderChoice,
                CivButton.LeaderChoiceScroll,
                CivButton.DifficultyBox,
                CivButton.DifficultyEmperor,
                CivButton.Chatinput,
            }
        );

        public static bool IsEqual(ScreenLocation one, ScreenLocation two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }
    

}