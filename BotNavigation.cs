using NavigationAndLocations;
using OCR;
using System.Runtime.InteropServices;
using System.Text;

namespace Gamebot
{

    class BotLocaliztation
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static bool IsCivForeground()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            StringBuilder windowTitle = new StringBuilder(256);
            GetWindowText(foregroundWindow, windowTitle, windowTitle.Capacity);

            string title = windowTitle.ToString();
            return title.Contains("Civilization V") || title.Contains("Sid Meier");
        }

        public static bool ConfirmLocation(ScreenLocation location, bool geterrorlocal = false)
        {
            if (!geterrorlocal)
            {
                if (ScreenLocation.IsEqual(location, GetCurrentScreen()))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (ScreenLocation.IsEqual(location, GetCurrentScreen(keepCheckingTilFind: false)))
                {
                    return true;
                }
                return false;
            }


        }
        public static ScreenLocation GetCurrentScreen(bool keepCheckingTilFind = true)
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
                    try
                    {
                        return GetQuitgameconfirmation();

                    }
                    catch
                    {

                        if (keepCheckingTilFind)
                        {
                            System.Console.WriteLine("Cant identify current screen");
                            CivBot.Sleep(5000);
                            return GetCurrentScreen();
                        }
                        return ScreenLocation.Location_error;

                    }

                }

            }
        }
        public static string GetTextAt(CivTextBox place)
        {
            Program.EnsureCivForegroundWindow();
            return ImgToText.TextAt(place.GetRectanglePictureBox(), place.filename).TrimEnd();
        }
        public static ScreenLocation GetMenuBasedLocations()
        {
            string Menutxt = GetTextAt(CivTextBox.MenuText);
            switch (Menutxt)
            {
                case "SINGLE PLAYER":
                    System.Console.WriteLine("Identified currentscreen Main menu");
                    return ScreenLocation.Menu_Main;
                case "STANDARD":
                    System.Console.WriteLine("Identified currentscreen Hot Or standard");
                    return ScreenLocation.Menu_HotOrStandard;
                case "INTERNET":
                    System.Console.WriteLine("Identified currentscreen Internet menu");
                    return ScreenLocation.Menu_InternetOrLocal;
                case "[Nl":
                    System.Console.WriteLine("Identified currentscreen Internet menu");
                    return ScreenLocation.Menu_InternetOrLocal;
                default:
                    throw new Exception();
            }
        }
        public static ScreenLocation GetHeaderBasedLocations()
        {
 
            string txt = GetTextAt(CivTextBox.HeaderText);
            switch (txt)
            {
                case "INTERNET GAMES":
                    System.Console.WriteLine("Identified currentscreen Internet Lobbies");
                    return ScreenLocation.InternetLobbies;
                case "SETUP MULTIPLAYER GAME":
                    System.Console.WriteLine("Identified currentscreen Lobbysetup screen");
                    return ScreenLocation.SetupMulti;
                case "LOAD GAME":
                    System.Console.WriteLine("Identified currentscreen Load game screen");
                    return ScreenLocation.LoadGames1;
                case "STAGING ROOM":
                    return ScreenLocation.StagingRoom;
                case "KEK MOD V1.4":
                    return ScreenLocation.StagingRoom;
                default:
                    throw new Exception();
            }


        }

        public static ScreenLocation GetQuitgameconfirmation()
        {
            string txt = GetTextAt(CivTextBox.ConfirmQuitgame);
            switch (txt)
            {
                case "Yes":
                    return ScreenLocation.Confirmquitscreen;
                default: throw new Exception("No match on quitgame screen");
            }


        }

    }



    class Pathgetter
    {


        public static List<CivButton> GetPath(ScreenLocation goal, ScreenLocation startscreen)
        {
            List<CivButton> EmptyPathingList = new List<CivButton>();
            List<CivButton> PathToGoal = GetPathFromTo(startscreen, goal, EmptyPathingList);
            if (CivButton.IsEqual(PathToGoal[PathToGoal.Count - 1], CivButton.Backtrack))
            {
                List<CivButton> BacktrackingSteps = new List<CivButton>();
                BacktrackingSteps = GoToMainMenu(startscreen, BacktrackingSteps);
                PathToGoal.AddRange(BacktrackingSteps);
            }
            return PathToGoal;
        }

        static List<CivButton> GoToMainMenu(ScreenLocation CurrentScreen, List<CivButton> BacktrackingList)
        {
            if (!ScreenLocation.IsEqual(CurrentScreen, ScreenLocation.Menu_Main))
            {
                BacktrackingList.Add(CivButton.Backtrack);
                GoToMainMenu(CurrentScreen.PreviousScreen, BacktrackingList);
                if (ScreenLocation.IsEqual(CurrentScreen, ScreenLocation.Menu_Main))
                {
                    BacktrackingList.Add(CurrentScreen.ButtonToPress_PreviousScreen);
                    return BacktrackingList;
                }
            }
            return BacktrackingList;
        }


        static List<CivButton> GetPathFromTo(ScreenLocation StartScreen, ScreenLocation Goal, List<CivButton> path)
        {
            bool test = System.Object.ReferenceEquals(StartScreen, Goal);
            if (!ScreenLocation.IsEqual(StartScreen, Goal))
            {
                path.Add(Goal.ButtonToPress_PreviousScreen);
                if (ScreenLocation.IsEqual(Goal, ScreenLocation.Menu_Main))
                {
                    path.Add(Goal.ButtonToPress_PreviousScreen);
                    return path;
                }
                else
                {
                    GetPathFromTo(StartScreen, Goal.PreviousScreen, path);
                }
            }
            return path;
        }
    }
}