using NavigationAndLocations;
using OCR;
namespace Gamebot
{
    class Navigation
    {
        public static bool ConfirmLocation(Location location)
        {
            if (Location.IsEqual(location, GetCurrentScreen()))
            {
                return true;
            }
            return false;
        }
        public static Location GetCurrentScreen()
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
        public static string takepic(CivTextBox box)
        {
            return TextAt(box).TrimEnd();
        }
        public static Location GetMenuBasedLocations()
        {
            string Menutxt = TextAt(CivTextBox.MenuText).TrimEnd();
            switch (Menutxt)
            {
                case "SINGLE PLAYER":
                    return Location.ScreenMenu_Main;
                case "STANDARD":
                    return Location.ScreenMenu_HotOrStandard;
                case "INTERNET":
                    System.Console.WriteLine("success now");
                    return Location.ScreenMenu_InternetOrLocal;
                default:
                    throw new Exception();
            }
        }
        public static Location GetHeaderBasedLocations()
        {

            string txt = TextAt(CivTextBox.HeaderText).TrimEnd();
            switch (txt)
            {
                case "INTERNET GAMES":
                    return Location.Screen_InternetLobbies;
                case "SETUP MULTIPLAYER GAME":
                    return Location.Screen_SetupMulti;
                case "LOAD GAME":
                    return Location.Screen_LoadGames1;
                case "STAGING ROOM":
                    return Location.Screen_StagingRoom;
                default:
                    throw new Exception("No match on HeaderBasedLocations");
            }


        }
        public static string TextAt(CivTextBox location)
        {
            ImgToText.TakeScreenshotof(location.GetRectanglePictureBox(), location.filename);
            string results = ImgToText.TextReader(location.filename);
            return results;
        }
        public static void NavigateTo(Location Goal)
        {
            Location startscreen = Navigation.GetCurrentScreen();
            if (!Location.IsEqual(Goal, startscreen))
            {
                List<CivButton> path = GetPath(Goal, startscreen);
                ExcecuteNavigation(path);
            }

        }

        static void ExcecuteNavigation(List<CivButton> ListofCommands)
        {

            for (int i = ListofCommands.Count - 1; i >= 0; i--)
            {
                CivButton command = ListofCommands[i];
                switch (command)
                {
                    case var b when b == CivButton.Backtrack:
                        CivBot.HitEscapeKey();
                        break;
                    default:
                        CivBot.MoveAndClick(command);
                        break;
                }
                Thread.Sleep(1000);
            }
        }
        static List<CivButton> GetPath(Location goal, Location startscreen)
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



        static List<CivButton> GoToMainMenu(Location CurrentScreen, List<CivButton> BacktrackingList)
        {
            if (!Location.IsEqual(CurrentScreen, Location.ScreenMenu_Main))
            {
                BacktrackingList.Add(CivButton.Backtrack);
                GoToMainMenu(CurrentScreen.PreviousScreen, BacktrackingList);
                if (Location.IsEqual(CurrentScreen, Location.ScreenMenu_Main))
                {
                    BacktrackingList.Add(CurrentScreen.ButtonToPress);
                    return BacktrackingList;
                }
            }
            return BacktrackingList;
        }


        static List<CivButton> GetPathFromTo(Location StartScreen, Location Goal, List<CivButton> path)
        {
            bool test = System.Object.ReferenceEquals(StartScreen, Goal);
            if (!Location.IsEqual(StartScreen, Goal))
            {
                path.Add(Goal.ButtonToPress);
                if (Location.IsEqual(Goal, Location.ScreenMenu_Main))
                {
                    path.Add(Goal.ButtonToPress);
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