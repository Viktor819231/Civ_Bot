using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using NavigationAndLocations;


namespace Gamebot
{
    class Bot
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

        public static void NavigateTo(ScreenLocation Goal)
        {
            List<Button> path = GetPath(Goal);
            ExcecuteNavigation(path);
        }

        static void ExcecuteNavigation(List<Button> ListofCommands)
        {

            for (int i = ListofCommands.Count - 1; i >= 0; i--)
            {
                Button command = ListofCommands[i];
                switch (command)
                {
                    case var b when b == Button.button_Backtrack:
                        Bot.HitEscapeKey();
                        break;
                    default:
                        Bot.MoveAndClick(command);
                        break;
                }
                Thread.Sleep(1000);
            }
        }
        static List<Button> GetPath(ScreenLocation goal)
        {
            List<Button> EmptyPathingList = new List<Button>();
            ScreenLocation startscreen = Navigation.GetCurrentScreen();
            List<Button> PathToGoal = GetPathFromTo(startscreen, goal, EmptyPathingList);
            if (Button.IsEqual(PathToGoal[PathToGoal.Count - 1], Button.button_Backtrack))
            {
                List<Button> BacktrackingSteps = new List<Button>();
                BacktrackingSteps = GoToMainMenu(startscreen, BacktrackingSteps);
                PathToGoal.AddRange(BacktrackingSteps);
            }
            return PathToGoal;
        }



        static List<Button> GoToMainMenu(ScreenLocation CurrentScreen, List<Button> BacktrackingList)
        {
            if (!ScreenLocation.IsEqual(CurrentScreen, ScreenLocation.ScreenMenu_Main))
            {
                BacktrackingList.Add(Button.button_Backtrack);
                GoToMainMenu(CurrentScreen.PreviousScreen, BacktrackingList);
                if (ScreenLocation.IsEqual(CurrentScreen, ScreenLocation.ScreenMenu_Main))
                {
                    BacktrackingList.Add(CurrentScreen.ButtonToPress);
                    return BacktrackingList;
                }
            }
            return BacktrackingList;
        }


        static List<Button> GetPathFromTo(ScreenLocation StartScreen, ScreenLocation Goal, List<Button> path)
        {
            bool test = System.Object.ReferenceEquals(StartScreen, Goal);
            if (!ScreenLocation.IsEqual(StartScreen, Goal))
            {
                path.Add(Goal.ButtonToPress);
                if (ScreenLocation.IsEqual(Goal, ScreenLocation.ScreenMenu_Main))
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