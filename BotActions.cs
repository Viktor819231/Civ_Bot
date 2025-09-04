using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NavigationAndLocations;
using OCR;



namespace Gamebot
{


    class CivBotNavigation
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
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
            if (CivButton.IsEqual(button, CivButton.Confirmexitgame))
            {
                System.Console.WriteLine("its true");
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

            if (Program.pausebot)
            {
                System.Console.WriteLine("bot is paused");
                while (Program.pausebot)
                {
                    Thread.Sleep(3000);
                }
                System.Console.WriteLine("bot is unpaused");
            }
            while (!Program.IsCivGameRunning())
            {
                System.Console.WriteLine("cant detect game running");
                Thread.Sleep(5000);
                if (Program.IsCivGameRunning())
                {
                    break;
                }
                Program.startCivdx9();
            }
            if (!BotLocaliztation.IsCivForeground())
            {

            }
            while (!BotLocaliztation.IsCivForeground())
            {
                Thread.Sleep(1000);
                while (!Program.IsCivGameRunning())
                {
                    System.Console.WriteLine("Game cant be detected. will try start game");
                    Program.startCivdx9();
                    Thread.Sleep(5000);
                    break;
                }
                System.Console.WriteLine("Civ not in focus, will pull civ in focus in 5 sec");
                Thread.Sleep(3000);
                Program.SetForegroundWindow(Program.CivWindowHandle);

            }




        }
        public static void backtrack()
        {
            try
            {
                Sleep(50);
                string scriptpath = GetScriptFolderPath("Backtrack.exe");
                var process = Process.Start(scriptpath);
                Sleep(400);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing Backtrack.exe: {ex.Message}");
            }
        }
        public static void EraseExistingText(int CharactersToErase = 15)
        {
            System.Console.WriteLine("Erasingtext");
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
            Sleep(50);
            string scriptpath = GetScriptFolderPath("Enter.exe");
            Process.Start(scriptpath);
            Sleep(300);
        }
        public static void Inputtext(string txt)
        {
            Sleep(50);
            string scriptpath = GetScriptFolderPath("SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptpath, arg);
            Sleep(1000);
        }

        public static void QuickInputtext(string txt)
        {
            Sleep(50);
            string scriptpath = GetScriptFolderPath("SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptpath, arg);
            Sleep(100);
        }

        public static void MoveMouseTo(CivButton button)
        {
            int headerOffset = ImgToText.getheaderheight();
            int x = button.x_left;
            int y = button.y_top;
            string scriptpath = GetScriptFolderPath("MoveMouseTo.exe");
            string args = $"{x} {y}";
            Process.Start(scriptpath, args);
            Sleep(250);

        }

        public static void SimpleClick()
        {
            //Normal click expects civ to be in focus or will sleep
            try
            {
                string scriptpath = GetScriptFolderPath("click.exe");
                var process = Process.Start(scriptpath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing click.exe: {ex.Message}");
            }
        }


        public static void Click()
        {
            try
            {
                Sleep(50);
                string scriptpath = GetScriptFolderPath("click.exe");
                var process = Process.Start(scriptpath);
                Sleep(400);
                MoveMouseTo(CivButton.outoftheway);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing click.exe: {ex.Message}");
            }
        }
        public static void HitEscapeKey()
        {
            Sleep(50);
            string scriptpath = GetScriptFolderPath("Backtrack.exe");
            Process.Start(scriptpath);
            Sleep(400);
        }

        public static void MoveAndClick(CivButton button)
        {
            if (!Program.settings.AlwaysConfirmLocationBeforeInput)
            {
                Sleep(50);
                MoveMouseTo(button);
                Sleep(300);
                Click();
                Sleep(300);
            }
            else
            {

                ConfirmLocation_ThenMoveAndClick(button);


            }

        }

        public static void indirectclicktest(CivButton button)
        {
            int headerOffset = ImgToText.getheaderheight();
            int x = button.x_left;
            int y = button.y_top + headerOffset;
            string args = $"{Program.CivWindowHandle} {x} {y}";
            string scriptpath = GetScriptFolderPath("indirectclicktest.exe");
            var process = Process.Start(scriptpath, args);

        }

        public static void ConfirmLocation_ThenMoveAndClick(CivButton button)
        {
            bool isButtonOnScreen = CivBotNavigation.isButtonOnScreen(button);
            if (!isButtonOnScreen)
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
                if (!CivBotNavigation.isButtonOnScreen(button))
                {
                    Sleep(50);
                    HitEscapeKey();
                    System.Console.WriteLine("Bot is lost, going back to main screen");
                    CivBotNavigation.NavigateTo(ScreenLocation.Menu_Main);

                }
            }

            if (isButtonOnScreen)
            {
                Sleep(50);
                MoveMouseTo(button);
                Sleep(150);
                Click();
                Sleep(300);
            }
        }

        public static string GetScriptFolderPath(string scriptName)
        {

            string localPath = Path.Combine(AppContext.BaseDirectory, "AHK scripts", scriptName);
            if (File.Exists(localPath))
            {
                return localPath;
            }


            string devPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..", "AHK scripts", scriptName);
            if (File.Exists(devPath))
            {
                return Path.GetFullPath(devPath);
            }

            return localPath;
        }

    }



}