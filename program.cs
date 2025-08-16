using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NavigationAndLocations;
using OCR;
using System.Net;


namespace Gamebot
{

    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public static IntPtr CivWindowHandle = IntPtr.Zero;
        public static Settings settings;
        public static bool pausebot = false;
        [STAThread]
        static void Main(string[] args)
        {
            SetProcessDPIAware();
            BotFrontendLauncher.StartFrontend();
        }

        public static void Initilizebot(CancellationToken cancellationToken = default)
        {
            //This will be the function that is called on start bot from frontend start button

            Console.WriteLine("Initilizebot...");
            settings = new Settings();
            settings.Validatesettings();

            if (!IsCivGameRunning())
            {
                startCivdx9();
            }
            SetForegroundWindow(CivWindowHandle);
            CivBot.Sleep(50);
            while(!BotLocaliztation.ConfirmLocation(ScreenLocation.Menu_Main)){
                SetForegroundWindow(CivWindowHandle);
                CivBot.HitEscapeKey();
            }
            TestOCR();
            TestAutoHokeyScripts();

            System.Console.WriteLine("Initilization complete and tests passed. Starting Bot");
            Console.WriteLine("Setting up lobby...");
            SetupNewLobby();
            RunMainBotLoop();
        }




        public static void RunMainBotLoop()
        {
            System.Console.WriteLine("Starting Main bot loop");
            while (true)
            {
                if (Timekeeping.ShouldRestartGame)
                {
                    RestartGameIfNeccesary();
                }
                if (BotLocaliztation.ConfirmLocation(ScreenLocation.StagingRoom))
                {
                    CivBotChatter.ScanChat_AndRespond();
                }
                if (!BotLocaliztation.ConfirmLocation(ScreenLocation.StagingRoom) || Timekeeping.ShouldRehostLobby)
                {
                    CivBotNavigation.NavigateTo(ScreenLocation.Menu_Main);
                    SetupNewLobby();
                }

            }

        }

        public static void SetupNewLobby()
        {
            bool setupcomplete = false;
            while (!setupcomplete)
            {
                while (true)
                {
                    CivBot.Sleep(1000);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti)))
                    {
                        Console.WriteLine("Not in staging room, navigating to SetupMulti...");
                        CivBotNavigation.NavigateTo(ScreenLocation.SetupMulti);
                    }
                    CivBot.MoveAndClick(CivButton.LobbyNameInputField);
                    CivBot.EraseExistingText();
                    CivBot.Inputtext(settings.LobbyName);
                    System.Console.WriteLine("Loading game with template lobby");
                    CivBot.MoveAndClick(CivButton.Loadgame);
                    CivBot.MoveAndClick(CivButton.GameConfigfile);
                    CivBot.MoveAndClick(CivButton.Loadgame_hostgame);
                    CivBot.Sleep(100);
                    CivBot.backtrack();
                    CivBot.Sleep(100);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti)))
                    {
                        CivBot.Sleep(3000);
                        if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti)))
                        {
                            break;
                        }
                    }
                    CivBot.MoveAndClick(CivButton.HostLobby);
                    CivBot.Sleep(1000);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.StagingRoom))) { break; }
                    ChooseLeaderInLobbyEtc();
                    //To get the text in right place we print some msgs to make bottom row be the one that shows on connect
                    CivBotChatter.justloopthrubasicadds(sleepbetweenmsgs: 3000);
                    if ((BotLocaliztation.ConfirmLocation(location: ScreenLocation.StagingRoom)))
                    {
                        setupcomplete = true;
                        System.Console.WriteLine("Lobby SetupComplete, can now start advertising");
                        break;
                    }

                }

            }


        }

        public static void ChooseLeaderInLobbyEtc()
        {
            System.Console.WriteLine("Setting up lobby");
            CivBot.MoveAndClick(CivButton.DifficultyBox);
            CivBot.MoveAndClick(CivButton.DifficultyEmperor);
            CivBot.MoveAndClick(CivButton.LeaderChoice);
            CivBot.MoveAndClick(CivButton.LeaderChoiceScroll);
            CivBot.MoveAndClick(CivButton.AmericaLeaderChoice);
            CivBot.MoveAndClick(CivButton.Chatinput);

        }


        public static void quitgame()
        {
            System.Console.WriteLine("exiting the game manually");
            CivBotNavigation.NavigateTo(ScreenLocation.Menu_Main);
            CivBot.MoveAndClick(CivButton.Exitgame);
            CivBot.MoveMouseTo(CivButton.Confirmexitgame);
            CivBot.SimpleClick();

        }

        public static bool IsCivGameRunning()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("CivilizationV");
                if (processes.Length > 0)
                {
                    foreach (Process process in processes)
                    {
                        if (!process.HasExited && process.Responding)
                        {
                            CivWindowHandle = process.MainWindowHandle;
                            return true;
                        }
                    }
                }
                CivWindowHandle = IntPtr.Zero;
                Console.WriteLine("CivilizationV process not found");
                return false;
            }
            catch (Exception ex)
            {
                CivWindowHandle = IntPtr.Zero;
                Console.WriteLine($"Error checking if game is running: {ex.Message}");
                return false;
            }
        }
        public static void startCivdx9()
        {
            System.Console.WriteLine("Starting Civ");
            try
            {
                if (!File.Exists(settings.Civfilepath))
                {
                    Console.WriteLine($"Error: File not found at {settings.Civfilepath}");
                    return;
                }

                Console.WriteLine($"Game found in files");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = settings.Civfilepath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(settings.Civfilepath)
                };

                try
                {
                    Process.Start(startInfo);
                    Console.WriteLine("Game launch command sent");

                    WaitForGameToCompleteLaunch();
                    return;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Failed to Launch exe");
                    Console.WriteLine($"CantLaunchCivFromFiles");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting game: {ex.Message}");
            }
        }
        public static void WaitForGameToCompleteLaunch()
        {
            System.Console.WriteLine("Waiting for 60 sec for game to launch");
            for (int i = 0; i < 6; i++)
            {
                int tracker = i * 10;
                System.Console.WriteLine(60 - tracker + "...");
                Thread.Sleep(10000);
            }
            bool isCivUp = false;
            while (!isCivUp)
            {

                if (IsCivGameRunning())
                {
                    try
                    {
                        SetForegroundWindow(CivWindowHandle);
                        Thread.Sleep(2000);
                        CivBot.HitEscapeKey();
                        CivBot.HitEscapeKey();
                        isCivUp = true;
                        System.Console.WriteLine("Game sucessfully launched, waiting for main menu");
                        Thread.Sleep(5000);
                    }
                    catch
                    {
                        System.Console.WriteLine("Cant pull Civ to the forground");
                        System.Console.WriteLine("Waiting 20seconds to try again");
                        Thread.Sleep(20000);
                    }

                }
                else
                {
                    System.Console.WriteLine("Cant detect Civ 5, waiting another 20 sec for civ to launch");
                }
                Thread.Sleep(20000);
            }


        }

        public static void TestOCR()
        {
            SetForegroundWindow(CivWindowHandle);
            Console.WriteLine("Testing Tesseract OCR...");
            try
            {
                string testText = ImgToText.TextAt(CivTextBox.MenuText.GetRectanglePictureBox(), CivTextBox.MenuText.filename);
                Console.WriteLine($"OCR successful");
            }
            catch (Exception e)
            {
                Console.WriteLine($"✗ OCR failed: {e.Message}");
                if (e.InnerException != null)
                    Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                Console.WriteLine($"Stack trace: {e.StackTrace}");
            }



        }
        public static void TestAutoHokeyScripts()
        {

            Console.WriteLine("Testing AHK scripts...");
            CivBot.Sleep(500);
            try
            {
                CivBot.MoveMouseTo(CivButton.outoftheway);
                Console.WriteLine("AHK mouse movement successful");
            }
            catch (Exception e)
            {
                Console.WriteLine($"AHK failed: {e.Message}");
            }
        }

        public static void RestartGameIfNeccesary()
        {
            if (Timekeeping.ShouldRestartGame)
            {
                Restartgame();
            }

        }
        public static void Restartgame()
        {
            System.Console.WriteLine("Restarting The Game");
            while (IsCivGameRunning())
            {
                quitgame();
                Thread.Sleep(2000);
            }
            startCivdx9();
            Timekeeping.ShouldRestartGame = false;

        }

    }

}

