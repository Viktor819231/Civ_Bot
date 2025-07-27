using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using OCR_logic;


namespace Gamebot
{
    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();


        static void Main(string[] args)
        {
            Sleep(2000);
            SetProcessDPIAware();
            NavigateTo(ScreenLocation.Screen_SetupMulti);
            MoveAndClick(Button.Button_LobbyNameInputField);
            Sleep(50);
            EraseExistingText();
            Sleep(50);
            Inputtext("FFACIV.com");
        }

        static void NavigateTo(ScreenLocation Goal)
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
                        HitEscapeKey();
                        break;
                    default:
                        MoveAndClick(command);
                        break;
                }
                Thread.Sleep(1000);
            }
        }
        static List<Button> GetPath(ScreenLocation goal)
        {
            System.Console.WriteLine("GetPath");
            List<Button> EmptyPathingList = new List<Button>();
            ScreenLocation startscreen = ReturnCurrentPage();
            List<Button> PathToGoal = GetPathFromTo(startscreen, goal, EmptyPathingList);
            if (Button.IsEqual(PathToGoal[PathToGoal.Count - 1] ,Button.button_Backtrack))
            {
                System.Console.WriteLine("getting path GetPath backtrack");
                List<Button> BacktrackingSteps = new List<Button>();
                System.Console.WriteLine(BacktrackingSteps.Count);
                BacktrackingSteps = GoToMainMenu(startscreen, BacktrackingSteps);
                PathToGoal.AddRange(BacktrackingSteps);
            }
            System.Console.WriteLine("done getting path");
            return PathToGoal;
        }

        static ScreenLocation ReturnCurrentPage()
        {
            System.Console.WriteLine("returncurrentpage");
            OcrReader.TakeScreenshotof(TextBox.Location_Menu);
            string menu_results = OcrReader.TextReader(TextBox.Location_Menu);
            string FirstTwoLetters = "XXX";
            if (menu_results.Length > 1)
            {
                FirstTwoLetters = menu_results.Substring(0, 2);
            }
            switch (FirstTwoLetters)
            {
                case "SN":
                    return ScreenLocation.ScreenMenu_Main;
                case "ST":
                    return ScreenLocation.ScreenMenu_HotOrStandard;
                case "IN":
                    return ScreenLocation.ScreenMenu_InternetOrLocal;
                default:
                    return HeaderBasedLocations();
            }
        }
        static ScreenLocation HeaderBasedLocations()
        {
            OcrReader.TakeScreenshotof(TextBox.Location_header);
            string header_picture_results = OcrReader.TextReader(TextBox.Location_header).TrimEnd();
            System.Console.WriteLine(header_picture_results);
            switch (header_picture_results)
            {
                case "INTERNET GAMES":
                    return ScreenLocation.Screen_InternetLobbies;
                case "SETUP MULTIPLAYER GAME":
                    return ScreenLocation.Screen_SetupMulti;
                case "LOAD GAME":
                    return ScreenLocation.Screen_LoadGames1;
                case "STAGING ROOM":
                    return ScreenLocation.Screen_StagingRoom;

            }
            return ScreenLocation.Screen_InternetLobbies;
        }

        static List<Button> GoToMainMenu(ScreenLocation CurrentScreen, List<Button> BacktrackingList)
        {
            if (!ScreenLocation.IsEqual(CurrentScreen , ScreenLocation.ScreenMenu_Main))
            {
                System.Console.WriteLine(BacktrackingList.Count);
                BacktrackingList.Add(Button.button_Backtrack);
                GoToMainMenu(CurrentScreen.PreviousScreen, BacktrackingList);
                if (ScreenLocation.IsEqual(CurrentScreen ,ScreenLocation.ScreenMenu_Main))
                {
                    BacktrackingList.Add(CurrentScreen.ButtonToPress);
                    return BacktrackingList;
                }
            }
            return BacktrackingList;
        }


        static List<Button> GetPathFromTo(ScreenLocation StartScreen, ScreenLocation Goal, List<Button> path)
        {
            //Assuming it exsists in its path 
            bool test = System.Object.ReferenceEquals(StartScreen, Goal);
            System.Console.WriteLine("Before loop: " + test);
            if (!ScreenLocation.IsEqual(StartScreen, Goal))
            {

                int size = path.Count;
                Console.WriteLine("The list contains " + size + " items.");
                path.Add(Goal.ButtonToPress);
                if (ScreenLocation.IsEqual(Goal,ScreenLocation.ScreenMenu_Main))
                {
                    System.Console.WriteLine("it hit main menu");
                    path.Add(Goal.ButtonToPress);
                    return path;
                }else{
                    GetPathFromTo(StartScreen, Goal.PreviousScreen, path);
                }
            }
            return path;
            //otherwise i need other function that gets path to main and go from main to the page
        }

        static void EraseExistingText()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Backspace.exe");
            for (int i = 0; i < 30; i++)
            {
                Process.Start(scriptPath);
                Thread.Sleep(50);
            }

        }
        static void Inputtext(string txt)
        {

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "SendText.exe");
            string arg = $"{txt}";
            Process.Start(scriptPath, arg);
        }

        static void MoveMouseTo(LocationInGame cords)
        {
            int x = cords.x;
            int y = cords.y;
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "MoveMouseTo.exe");
            string args = $"{x} {y}";
            Process.Start(scriptPath, args);
        
        }
        static void Click()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "click.exe");
            Process.Start(scriptPath);
        }
        static void HitEscapeKey()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Backtrack.exe");
            Process.Start(scriptPath);
        }
        static void Sleep(int x)
        {
            Thread.Sleep(x);
        }
        static void MoveAndClick(LocationInGame cords)
        {
            MoveMouseTo(cords);
            System.Threading.Thread.Sleep(100);
            Click();
        }

    }
}
