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
                if (ScreenLocation.IsEqual(location, GetCurrentScreen(keepCheckingTilFind: false)))
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
        public static ScreenLocation GetCurrentScreen(bool keepCheckingTilFind = false)
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
                        return GetScreenLocationCreditScreen();

                    }
                    catch
                    {
                        if (keepCheckingTilFind)
                        {
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

        public static ScreenLocation GetScreenLocationCreditScreen()
        {
            string txt = GetTextAt(CivTextBox.CreditScreen).ToLower();
            System.Console.WriteLine($"Credit Screen OCR text: '{txt}'");
            
            // Check if text contains "click" or "continue" to handle OCR variations
            if (txt.Contains("click") || txt.Contains("continue"))
            {
                System.Console.WriteLine("Identified currentscreen Credit Screen - clicking to continue");
                var button = CivButton.CreditScreenbutton;
                System.Console.WriteLine($"Credit button coordinates: X={button.x_left}, Y={button.y_top}");
                
                // Don't use MoveAndClick - it checks location which causes infinite recursion!
                CivBot.MoveMouseTo(button);
                CivBot.Sleep(300);
                CivBot.Click();
                
                System.Console.WriteLine("Clicked credit screen button, waiting for transition...");
                CivBot.Sleep(2000);
                
                return ScreenLocation.CreditScreen;
            }
            
            throw new Exception($"Credit screen text didn't match. Got: '{txt}'");
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