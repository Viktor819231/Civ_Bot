using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using NavigationAndLocations;
using OCR;


namespace Gamebot
{
    class Navigation
    {
        public static CivScreenLocation GetCurrentScreen()
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
        public static CivScreenLocation GetMenuBasedLocations()
        {
            string Menutxt = TextAt(CivTextBox.MenuText).TrimEnd();
            switch (Menutxt)
            {
                case "SINGLE PLAYER":
                    return CivScreenLocation.ScreenMenu_Main;
                case "STANDARD":
                    return CivScreenLocation.ScreenMenu_HotOrStandard;
                case "INTERNET":
                    System.Console.WriteLine("success now");
                    return CivScreenLocation.ScreenMenu_InternetOrLocal;
                default:
                    throw new Exception();
            }
        }

        public static CivScreenLocation GetHeaderBasedLocations()
        {

            string txt = TextAt(CivTextBox.HeaderText).TrimEnd();
            switch (txt)
            {
                case "INTERNET GAMES":
                    return CivScreenLocation.Screen_InternetLobbies;
                case "SETUP MULTIPLAYER GAME":
                    return CivScreenLocation.Screen_SetupMulti;
                case "LOAD GAME":
                    return CivScreenLocation.Screen_LoadGames1;
                case "STAGING ROOM":
                    return CivScreenLocation.Screen_StagingRoom;
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
    }
    class CivBot
    {
        public static void Sleep(int x)
        {
            Thread.Sleep(x);
        }
        public static void backtrack()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Backtrack.exe");
            Process.Start(scriptPath);
            Sleep(300);
        }
        public static void EraseExistingText(int CharactersToErase = 15)
        {

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "HitBackspace.exe");
            for (int i = 0; i < CharactersToErase; i++)
            {
                Process.Start(scriptPath);
                Sleep(150);
            }
            Sleep(200);
        }
        public static void Enter()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Enter.exe");
            Process.Start(scriptPath);
            Sleep(200);
        }
        public static void Inputtext(string txt)
        {

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptPath, arg);
            Sleep(1000);
        }

        public static void MoveMouseTo(LocationInGame cords)
        {
            int x = cords.x_left;
            int y = cords.y_top;
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "MoveMouseTo.exe");
            string args = $"{x} {y}";
            Process.Start(scriptPath, args);
            Sleep(200);

        }
        public static void Click()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "click.exe");
            Process.Start(scriptPath);
            Sleep(200);
        }
        public static void HitEscapeKey()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Backtrack.exe");
            Process.Start(scriptPath);
            Sleep(100);
        }

        public static void MoveAndClick(LocationInGame cords)
        {
            MoveMouseTo(cords);
            System.Threading.Thread.Sleep(150);
            Click();
            Sleep(500);
        }

        public static void NavigateTo(CivScreenLocation Goal)
        {
            CivScreenLocation startscreen = Navigation.GetCurrentScreen();
            if (!CivScreenLocation.IsEqual(Goal, startscreen))
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
                    case var b when b == CivButton.button_Backtrack:
                        CivBot.HitEscapeKey();
                        break;
                    default:
                        CivBot.MoveAndClick(command);
                        break;
                }
                Thread.Sleep(1000);
            }
        }
        static List<CivButton> GetPath(CivScreenLocation goal, CivScreenLocation startscreen)
        {
            List<CivButton> EmptyPathingList = new List<CivButton>();
            List<CivButton> PathToGoal = GetPathFromTo(startscreen, goal, EmptyPathingList);
            if (CivButton.IsEqual(PathToGoal[PathToGoal.Count - 1], CivButton.button_Backtrack))
            {
                List<CivButton> BacktrackingSteps = new List<CivButton>();
                BacktrackingSteps = GoToMainMenu(startscreen, BacktrackingSteps);
                PathToGoal.AddRange(BacktrackingSteps);
            }
            return PathToGoal;
        }



        static List<CivButton> GoToMainMenu(CivScreenLocation CurrentScreen, List<CivButton> BacktrackingList)
        {
            if (!CivScreenLocation.IsEqual(CurrentScreen, CivScreenLocation.ScreenMenu_Main))
            {
                BacktrackingList.Add(CivButton.button_Backtrack);
                GoToMainMenu(CurrentScreen.PreviousScreen, BacktrackingList);
                if (CivScreenLocation.IsEqual(CurrentScreen, CivScreenLocation.ScreenMenu_Main))
                {
                    BacktrackingList.Add(CurrentScreen.ButtonToPress);
                    return BacktrackingList;
                }
            }
            return BacktrackingList;
        }


        static List<CivButton> GetPathFromTo(CivScreenLocation StartScreen, CivScreenLocation Goal, List<CivButton> path)
        {
            bool test = System.Object.ReferenceEquals(StartScreen, Goal);
            if (!CivScreenLocation.IsEqual(StartScreen, Goal))
            {
                path.Add(Goal.ButtonToPress);
                if (CivScreenLocation.IsEqual(Goal, CivScreenLocation.ScreenMenu_Main))
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