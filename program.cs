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
        private static extern IntPtr GetForegroundWindow();

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
            settings = new Settings();
            settings.Validatesettings();
            Task.Run(() => Initilizebot());
            if(settings.debugmode){
            BotFrontendLauncher.StartFrontend();
            }

        }

        public static void Initilizebot(CancellationToken cancellationToken = default)
        {
            Logger.Initialize();
            Console.WriteLine("Initilizebot...");
            
            // Initialize and test Firebase
            TestFirebase();
            
            if (!IsCivGameRunning())
            {
                startCivdx9();
            }
            Timekeeping.Starttimers();
            SetForegroundWindow(CivWindowHandle);
            TestOCR();
            TestAutoHokeyScripts();
            //settings.Printsettings();
            CivBot.Sleep(1000);
            //System.Console.WriteLine("Initilization complete and tests passed. Starting Bot");
            //Console.WriteLine("Setting up lobby...");
            RunMainBotLoop();
        }




        public static void RunMainBotLoop()
        {
            System.Console.WriteLine("Starting Main bot loop");
            Console.WriteLine(DateTime.Now);
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

                    //To get the text in right place we print some msgs to make bottom row be the one that shows on connect
                    CivBotChatter.justloopthrubasicadds(sleepbetweenmsgs: 250);
                    if ((BotLocaliztation.ConfirmLocation(location: ScreenLocation.StagingRoom)))
                    {
                        setupcomplete = true;
                        System.Console.WriteLine("Lobby SetupComplete, can now start advertising");
                        Logger.LogStat("New lobby created");
                        BotStats.IncrementRelobbies();
                        
                        // Log relobby to Firebase (non-blocking)
                        Task.Run(async () => await Databasecommuncation.LogRelobby());
                        
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
            while (IsCivGameRunning())
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
                Thread.Sleep(5000);
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
                            int maxwait = 0;
                            while (!process.Responding)
                            {
                                if (maxwait >= 12)
                                {
                                    return false;
                                }
                                Console.WriteLine("CivilizationV found but not responding. Waiting up" + (120 - (maxwait * 5)) + " seconds before restart...");
                                Thread.Sleep(10000);
                                maxwait += 1;
                                process.Refresh();
                                if (process.HasExited)
                                {
                                    CivWindowHandle = IntPtr.Zero;
                                    Console.WriteLine("CivilizationV process exited while waiting for response.");
                                    return false;
                                }
                            }
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
            System.Console.WriteLine("Civ Not Found among processes");
            return false;
        }
        public static void startCivdx9()
        {
            System.Console.WriteLine("Starting Civ");
            try
            {
                Timekeeping.ResetGameRestartTimer();
                Timekeeping.ResetLobbyTimer();
                Task.Run(async () => await Databasecommuncation.LogGameRestart());
                
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
            System.Console.WriteLine("Waiting 45 sec");
            Thread.Sleep(45000);
            CivBot.SimpleClick();
            System.Console.WriteLine("Waiting up to " + timetowait / 1000 + "sec for game to launch");
            for (int i = 0; i < timetowait / 10000; i++)
            {
                if (i >= (timetowait / 10000) - 1)
                {
                    System.Console.WriteLine("failed to launch");
                    QuitGame();
                    startCivdx9();
                }
                int tracker = i * 10;
                System.Console.WriteLine(timetowait / 1000 - tracker + "...");
                Thread.Sleep(10000);
                if (IsCivGameRunning())
                {
                    try
                    {
                        SetForegroundWindow(CivWindowHandle);
                        Thread.Sleep(2000);
                        if (GetForegroundWindow() == CivWindowHandle)
                        {
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
                        else
                        {
                            System.Console.WriteLine("Failed to pull Civ into focus");
                            CivBot.SimpleClick();
                        }
                    }
                    catch
                    {
                        System.Console.WriteLine("Something failed with setting Civ as focus");
                        return;
                    }

                }


            }

        }



        public static void TestFirebase()
        {
            Console.WriteLine("Testing Firebase connection...");
            try
            {

                string botId = Task.Run(async () => await Databasecommuncation.GetOrCreateBotId(settings.BotRegion, settings.BotName)).Result;
                Console.WriteLine($"✓ Bot ID: {botId}");
                Console.WriteLine($"✓ Bot Name: {settings.BotName}");
                Console.WriteLine($"✓ Bot Region: {settings.BotRegion}");
                string lobbyNameJson = Task.Run(async () => await Databasecommuncation.GetData("bot-config/lobbyName")).Result;
                if (lobbyNameJson != null && lobbyNameJson != "null")
                {
                    Console.WriteLine($"✓ Firebase config accessible");
                }
                else
                {
                    Console.WriteLine("⚠ Firebase config not found (will use settings.txt)");
                }
                Task.Run(async () => await Databasecommuncation.PingBot()).Wait();
                Console.WriteLine("✓ Firebase ping successful");
                Console.WriteLine("✓ Firebase tests passed");
            }
            catch (Exception e)
            {
                Console.WriteLine($"✗ Firebase test failed: {e.Message}");
                Console.WriteLine("⚠ Bot will continue with settings.txt only");
            }
        }

        public static void TestOCR()
        {
            SetForegroundWindow(CivWindowHandle);
            Console.WriteLine("Testing Tesseract OCR...");
            try
            {
                string testText = ImgToText.TextAt(CivTextBox.MenuText.GetRectanglePictureBox(), CivTextBox.MenuText.filename);
                Console.WriteLine($"✓ OCR successful");
            }
            catch (Exception e)
            {
                Console.WriteLine($"✗ OCR failed: {e.Message}");
                if (e.InnerException != null)
                    Console.WriteLine($"Inner exception: {e.InnerException.Message}");
                Console.WriteLine($"Stack trace: {e.StackTrace}");
            }
        }


        public static void EnsureCivForegroundWindow()
        {
            
            while (true)
            {
                if (IsCivGameRunning())
                {
                    Thread.Sleep(50);
                    SetForegroundWindow(CivWindowHandle);

                    if (GetForegroundWindow() == CivWindowHandle)
                    {
                        break;
                    }

                    System.Console.WriteLine("Civ is running but failed pulling into focus");
                    Thread.Sleep(5000);
                }
                else
                {
                    System.Console.WriteLine("cant detect Civ");
                    break;
                }
                

            }
        }
        public static void TestAutoHokeyScripts()
        {

            CivBot.Sleep(500);
            try
            {
                CivBot.MoveMouseTo(CivButton.outoftheway);
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
            Logger.LogStat("Game restarted");
            BotStats.IncrementRestarts();
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

