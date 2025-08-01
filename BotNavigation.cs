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

        public static bool IsCivActive()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            StringBuilder windowTitle = new StringBuilder(256);
            GetWindowText(foregroundWindow, windowTitle, windowTitle.Capacity);

            string title = windowTitle.ToString();
            return title.Contains("Civilization V") || title.Contains("Sid Meier");
        }

        public static void EnsureCivActive()
        {

                while (!IsCivActive())
                {
                    CivBot.Sleep(2000);
                }

        }
        public static bool ConfirmLocation(ScreenLocation location)
        {
            if (ScreenLocation.IsEqual(location, GetCurrentScreen()))
            {
                return true;
            }
            return false;
        }
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
                    CivBot.Sleep(5000);
                    return GetCurrentScreen();
                }

            }
        }
        public static string GetTextAt(CivTextBox place)
        {
            return ImgToText.TextAt(place.GetRectanglePictureBox(), place.filename).TrimEnd();
            
        } 
        public static ScreenLocation GetMenuBasedLocations()
        {
            string Menutxt = GetTextAt(CivTextBox.MenuText);
            switch (Menutxt)
            {
                case "SINGLE PLAYER":
                    return ScreenLocation.Menu_Main;
                case "STANDARD":
                    return ScreenLocation.Menu_HotOrStandard;
                case "INTERNET":
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
                    return ScreenLocation.InternetLobbies;
                case "SETUP MULTIPLAYER GAME":
                    return ScreenLocation.SetupMulti;
                case "LOAD GAME":
                    return ScreenLocation.LoadGames1;
                case "STAGING ROOM":
                    return ScreenLocation.StagingRoom;
                default:
                    throw new Exception("No match on HeaderBasedLocations");
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