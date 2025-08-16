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
        public static CivTextBox MenuText = new CivTextBox(390, 640, 230, 278, "ManuSS.png");
        public static CivTextBox SecondMenuText = new CivTextBox(390, 640, 280, 325, "secondManuSS.png");
        public static CivTextBox ChatText = new CivTextBox(50, 410, 663, 638, "ChatSS.png");
        public static CivTextBox ConfirmQuitgame = new CivTextBox(470, 550 ,380,415, "Quitgameoption.png");
    }
    public class CivButton : LocationInGame
    {
        public CivButton(int x1, int y1)
        {
            x_left = x1;
            y_top = y1;
        }
        public static CivButton outoftheway = new CivButton(50, 50);
        public static CivButton AmericaLeaderChoice = new CivButton(350, 560);
        public static CivButton LeaderChoice = new CivButton(350, 225);
        public static CivButton LeaderChoiceScroll = new CivButton(430, 595);
        public static CivButton DifficultyBox = new CivButton(500, 175);
        public static CivButton DifficultyEmperor = new CivButton(500, 325);
        public static CivButton LobbyNameInputField = new CivButton(410, 155);
        public static CivButton Backtrack = new CivButton(0, 0);
        public static CivButton MenuMultiplayer = new CivButton(x1: 600, 300);
        public static CivButton Exitgame = new CivButton(600, 600);
        public static CivButton Confirmexitgame = new CivButton(600, 375);
        public static CivButton MenuInternet = new CivButton(600, 240);
        public static CivButton MenuStandard = new CivButton(600, 240);
        public static CivButton HostLobby = new CivButton(840, 745);
        public static CivButton Loadgame = new CivButton(430, 745);
        public static CivButton GameConfigfile = new CivButton(714, 238);
        public static CivButton Loadgame_hostgame = new CivButton(840, 680);
        public static CivButton Chatinput = new CivButton(200, 673);

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
                CivButton.Backtrack,
                CivButton.outoftheway
            }
        );
        public static ScreenLocation Menu_Main = new ScreenLocation(0, 0, NullLobby, CivButton.Backtrack,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.outoftheway,
               CivButton.MenuMultiplayer,
               CivButton.Exitgame
            }
        );

        public static ScreenLocation Confirmquitscreen = new ScreenLocation(0, 0, Menu_Main, CivButton.Exitgame,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.outoftheway,
               CivButton.Confirmexitgame
            }
        );
        public static ScreenLocation Menu_HotOrStandard = new ScreenLocation(0, 0, Menu_Main, CivButton.MenuMultiplayer,
                new List<CivButton>
            {
                CivButton.Backtrack,
                CivButton.MenuStandard,
                CivButton.outoftheway
            }
        );
        public static ScreenLocation Menu_InternetOrLocal = new ScreenLocation(0, 0, Menu_HotOrStandard, CivButton.MenuStandard,
                new List<CivButton>
            {
                CivButton.Backtrack,
                CivButton.MenuInternet,
                CivButton.outoftheway
            }
        );
        public static ScreenLocation InternetLobbies = new ScreenLocation(0, 0, Menu_InternetOrLocal, CivButton.MenuInternet,
                new List<CivButton>
            {
                CivButton.Backtrack,
                CivButton.HostLobby,
                CivButton.outoftheway
            }
        );
        public static ScreenLocation SetupMulti = new ScreenLocation(0, 0, InternetLobbies, CivButton.HostLobby,
                new List<CivButton>
            {
                CivButton.Backtrack
                ,CivButton.LobbyNameInputField,
                CivButton.Loadgame,
                CivButton.HostLobby,
                CivButton.outoftheway
            }
        );
        public static ScreenLocation LoadGames1 = new ScreenLocation(0, 0, SetupMulti, CivButton.Loadgame,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.GameConfigfile,
               CivButton.Loadgame_hostgame,
               CivButton.outoftheway
            }
        );
        public static ScreenLocation LoadGames2 = new ScreenLocation(0, 0, LoadGames1, CivButton.GameConfigfile,
                new List<CivButton>
            {
               CivButton.Backtrack,
               CivButton.GameConfigfile,
               CivButton.Loadgame_hostgame,
               CivButton.outoftheway
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
                CivButton.outoftheway
            }
        );

        public static bool IsEqual(ScreenLocation one, ScreenLocation two)
        {
            return System.Object.ReferenceEquals(one, two);
        }
    }
    

}