using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NavigationAndLocations;



namespace Gamebot
{

    class CivBotNavigation
    {
        public static ScreenLocation goal;
        public static List<CivButton> Path = new List<CivButton>();

        public static void NavigateTo(ScreenLocation Goalarg)
        {
            goal = Goalarg;
            ScreenLocation startscreen = BotLocaliztation.GetCurrentScreen();
            if (!ScreenLocation.IsEqual(goal, startscreen))
            {
                ExcecuteNavigation(Pathgetter.GetPath(goal: goal, startscreen: startscreen));
            }

        }

        static void ExcecuteNavigation(List<CivButton> ListofBUTTONS)
        {

            for (int i = ListofBUTTONS.Count - 1; i >= 0; i--)
            {
                CivButton BUTTON = ListofBUTTONS[i];
                if (CivButton.IsEqual(CivButton.Backtrack, BUTTON))
                {
                    CivBot.HitEscapeKey();
                }
                else
                {
                    CivBot.MoveAndClick(BUTTON);
                }

            }
        }
        public static bool isButtonOnScreen(CivButton button)
        {
            ScreenLocation current = BotLocaliztation.GetCurrentScreen();
            if (current.AvailableButtons.Contains(button))
            {
                return true;
            }
            return false;
        }

    }
    public static class CivBot
    {


        public static void Sleep(int x)
        {
            int modifier = Program.settings.Botspeed;
            Thread.Sleep(x / modifier);
            BotLocaliztation.EnsureCivActive();
        }
        public static void backtrack()
        {
            try
            {
                string scriptpath = GetScriptFolderPath("Backtrack.exe");
                var process = Process.Start(scriptpath);
                Sleep(300);
                Console.WriteLine($"Executed: {scriptpath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing Backtrack.exe: {ex.Message}");
            }
        }
        public static void EraseExistingText(int CharactersToErase = 15)
        {
            string scriptpath = GetScriptFolderPath("HitBackspace.exe");
            for (int i = 0; i < CharactersToErase; i++)
            {
                Process.Start(scriptpath);
                Sleep(200);
            }
            Sleep(200);
        }
        public static void Enter()
        {
            string scriptpath = GetScriptFolderPath("Enter.exe");
            Process.Start(scriptpath);
            Sleep(200);
        }
        public static void Inputtext(string txt)
        {
            string scriptpath = GetScriptFolderPath("SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptpath, arg);
            Sleep(1000);
        }

        public static void QuickInputtext(string txt)
        {
            string scriptpath = GetScriptFolderPath("SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptpath, arg);
            Sleep(100);
        }

        public static void MoveMouseTo(LocationInGame cords)
        {
            int x = cords.x_left;
            int y = cords.y_top;
            string scriptpath = GetScriptFolderPath("MoveMouseTo.exe");
            string args = $"{x} {y}";
            Process.Start(scriptpath, args);
            Sleep(250);

        }
        public static void Click()
        {
            try
            {
                string scriptpath = GetScriptFolderPath("click.exe");
                var process = Process.Start(scriptpath);
                Sleep(150);
                MoveMouseTo(CivButton.outoftheway);
                Console.WriteLine($"Executed: {scriptpath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing click.exe: {ex.Message}");
            }
        }
        public static void HitEscapeKey()
        {
            string scriptpath = GetScriptFolderPath("Backtrack.exe");
            Process.Start(scriptpath);
            Sleep(100);
        }

        public static void MoveAndClick(CivButton button)
        {
            if (!Program.settings.AlwaysConfirmLocationBeforeInput)
            {
                MoveMouseTo(button);
                Sleep(150);
                Click();
                Sleep(300);
            }
            else
            {
                ConfirmLocation_ThenMoveAndClick(button);


            }

        }

        public static void ConfirmLocation_ThenMoveAndClick(CivButton button)
        {
            if (!CivBotNavigation.isButtonOnScreen(button))
            {
                int waittimesbefore = 5;
                for (int i = 0; i < waittimesbefore; i++)
                {
                    Sleep(1000);
                    if (CivBotNavigation.isButtonOnScreen(button))
                    {
                        break;
                    }
                }
            }//Bit sphaggetti but should work to backtrack if it fails confirmation after a while
            if (!CivBotNavigation.isButtonOnScreen(button))
            {
                HitEscapeKey();
                HitEscapeKey();

            }
            if (CivBotNavigation.isButtonOnScreen(button))
            {
                MoveMouseTo(button);
                Sleep(150);
                Click();
                Sleep(300);
            }
        }

        public static string GetScriptFolderPath(string scriptName)
        {
            // Try current directory first (for exe distribution)
            string localPath = Path.Combine(AppContext.BaseDirectory, "AHK scripts", scriptName);
            if (File.Exists(localPath)) 
            {
                Console.WriteLine($"Found script at: {localPath}");
                return localPath;
            }

            // Fallback to development path
            string devPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..", "AHK scripts", scriptName);
            if (File.Exists(devPath))
            {
                Console.WriteLine($"Found script at: {devPath}");
                return Path.GetFullPath(devPath);
            }
            
            // Log error if script not found
            Console.WriteLine($"ERROR: Script not found: {scriptName}");
            Console.WriteLine($"Tried: {localPath}");
            Console.WriteLine($"Tried: {Path.GetFullPath(devPath)}");
            Console.WriteLine($"Base directory: {AppContext.BaseDirectory}");
            
            return localPath; // Return local path anyway (will fail but we'll see the error)
        }

    }



}