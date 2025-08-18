using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NavigationAndLocations;
using OCR;
using System.Net;
using System.Transactions;
using System.Net.WebSockets;


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
            Timekeeping.Starttimers();
            SetForegroundWindow(CivWindowHandle);
            CivBot.Sleep(50);
            while (!BotLocaliztation.ConfirmLocation(ScreenLocation.Menu_Main))
            {
                SetForegroundWindow(CivWindowHandle);
                CivBot.HitEscapeKey();
                CivBot.Sleep(200);
            }
            TestOCR();
            TestAutoHokeyScripts();

            settings.Printsettings();
            CivBot.Sleep(1000);
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
                System.Console.WriteLine("Restarting game in: " + Timekeeping.GetTimeToNextRestart() + " Minutes");
                RestartGameIfNeccesary();
                if (BotLocaliztation.ConfirmLocation(ScreenLocation.StagingRoom))
                {
                    System.Console.WriteLine(Timekeeping.GetTimeToNextRelobby() + "Minutes to next relobby");
                    CivBotChatter.LoopMsgs_ScanAndRespond();

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

                    Timekeeping.ResetLobbyTimer();
                    CivBot.Sleep(3000);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti)))
                    {
                        Console.WriteLine("Not in staging room, navigating to SetupMulti...");
                        CivBotNavigation.NavigateTo(ScreenLocation.SetupMulti);
                        if (!BotLocaliztation.ConfirmLocation(ScreenLocation.SetupMulti))
                        {
                            break;
                        }
                    }
                    CivBot.MoveAndClick(CivButton.LobbyNameInputField);
                    CivBot.EraseExistingText();
                    CivBot.Inputtext(settings.LobbyName);
                    CivBot.Sleep(2000);
                    CivBot.Enter();
                    Thread.Sleep(300);
                    System.Console.WriteLine("Loading game with template lobby");
                    CivBot.MoveAndClick(CivButton.Loadgame);
                    Thread.Sleep(300);
                    CivBot.MoveAndClick(CivButton.GameConfigfile);
                    Thread.Sleep(300);
                    CivBot.MoveAndClick(CivButton.Loadgame_hostgame);
                    CivBot.Sleep(500);
                    CivBot.backtrack();
                    CivBot.Sleep(500);
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
                    CivBotChatter.justloopthrubasicadds(sleepbetweenmsgs: 1000);
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


        public static void QuitGame()
        {
            try
            {
                var processes = Process.GetProcessesByName("CivilizationV");
                foreach (var process in processes)
                {
                    if (!process.HasExited)
                    {
                        Console.WriteLine("Force killing CivilizationV process...");
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error force quitting CivilizationV: {ex.Message}");
            }
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
                        if (!process.HasExited)
                        {
                            CivWindowHandle = process.MainWindowHandle;
                            // Wait until the process is responding
                            while (!process.Responding)
                            {
                                Console.WriteLine("CivilizationV found but not responding. Waiting 10 sec before restart...");
                                Thread.Sleep(10000);
                                process.Refresh();
                                if (process.HasExited)
                                {
                                    CivWindowHandle = IntPtr.Zero;
                                    Console.WriteLine("CivilizationV process exited while waiting for response.");
                                    return false;
                                }
                            }
                            // Process is running and responding
                            return true;
                        }
                    }
                }
                CivWindowHandle = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                CivWindowHandle = IntPtr.Zero;
                Console.WriteLine($"Error checking if game is running: {ex.Message}");
                Thread.Sleep(2000);
            }
            return false;
        }
        public static void startCivdx9()
        {
            System.Console.WriteLine("Starting Civ");
            try
            {
                Timekeeping.ResetGameRestartTimer();
                Timekeeping.ResetLobbyTimer();
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
                    Timekeeping.ResetGameRestartTimer();
                    Timekeeping.ResetLobbyTimer();
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
            int timetowait = settings.WaittimeafterLaunch;
            System.Console.WriteLine("Waiting up to " + timetowait / 1000 + "sec for game to launch");
            for (int i = 0; i < timetowait / 10000; i++)
            {
                int tracker = i * 10;
                System.Console.WriteLine(timetowait / 1000 - tracker + "...");
                Thread.Sleep(10000);
                if (IsCivGameRunning())
                {

                    try
                    {
                        SetForegroundWindow(CivWindowHandle);

                        if (BotLocaliztation.ConfirmLocation(ScreenLocation.Menu_Main, geterrorlocal: true))
                        {
                            System.Console.WriteLine("Game sucessfully launched");
                            Thread.Sleep(2000);
                            break;
                        }
                        else
                        {

                            System.Console.WriteLine("Continuing to wait for main menu to load");
                            if (i % 3 == 1)
                            {
                                CivBot.MoveMouseTo(CivButton.outoftheway);
                                CivBot.SimpleClick();
                                System.Console.WriteLine("Clicked screen, waiting 30 sec for main menu to load");
                            }


                        }


                    }
                    catch
                    {
                        return;
                    }

                }



            }
                System.Console.WriteLine("failed to launch");
                QuitGame();
                startCivdx9();
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
                System.Console.WriteLine("Game been active more then: " + settings.TimeBetweenGamerestart + " Restarting Game");
                Restartgame();
            }

        }
        public static void Restartgame()
        {
            System.Console.WriteLine("Restarting The Game");
            while (IsCivGameRunning())
            {
                Program.QuitGame();
                Thread.Sleep(10000);
            }
            startCivdx9();
            Timekeeping.ShouldRestartGame = false;

        }

    }

}

