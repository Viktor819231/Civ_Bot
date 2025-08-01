using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NavigationAndLocations;
using OCR;


namespace Gamebot
{
    
    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        public static Settings settings = new Settings();

        [STAThread]
        static void Main(string[] args)
        {
            SetProcessDPIAware();
            
            // Launch the frontend instead of running bot directly
            BotFrontendLauncher.StartFrontend();
        }

        public static void RunBareBonesBot(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("RunBareBonesBot starting...");
            Console.WriteLine($"Lobby name: {settings.LobbyName}");
            
            Console.WriteLine("Calling SetupNewLobby...");
            SetupNewLobby(settings.LobbyName);
            Console.WriteLine("SetupNewLobby completed.");
            
            bool lobbycheck = true;
            bool loopforever = true;
            while (loopforever && !cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Checking if in staging room...");
                if (!(BotLocaliztation.ConfirmLocation(ScreenLocation.StagingRoom)))
                {
                    Console.WriteLine("Not in staging room, navigating to SetupMulti...");
                    CivBotNavigation.NavigateTo(ScreenLocation.SetupMulti);
                    SetupNewLobby(settings.LobbyName);
                }
                if (BotLocaliztation.ConfirmLocation(ScreenLocation.StagingRoom))
                {
                    Console.WriteLine("In staging room, starting lobby monitoring...");
                    lobbycheck = true;
                }
                while (lobbycheck && !cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Scanning chat and responding...");
                    CivBot.Sleep(200);
                    CivBotChatter.LoopMsgs_ScanAndRespond();
                    lobbycheck = BotLocaliztation.ConfirmLocation(ScreenLocation.StagingRoom);
                }

            }
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        

        public static void SetupNewLobby(string LobbyName)
        {
            bool setupcomplete = false;
            while (!setupcomplete)
            {
                while (true)
                {
                    CivBot.Sleep(1000);
                    CivBotNavigation.NavigateTo(ScreenLocation.SetupMulti);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti))) { break; }
                    CivBot.MoveAndClick(CivButton.LobbyNameInputField);
                    CivBot.EraseExistingText();
                    CivBot.Inputtext(LobbyName);
                    CivBot.MoveAndClick(CivButton.Loadgame);
                    CivBot.MoveAndClick(CivButton.GameConfigfile);
                    CivBot.MoveAndClick(CivButton.Loadgame_hostgame);
                    CivBot.Sleep(100);
                    CivBot.backtrack();
                    CivBot.Sleep(100);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti))) {
                        CivBot.Sleep(3000);
                        if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.SetupMulti))){
                           break; } 
                        }
                    CivBot.MoveAndClick(CivButton.HostLobby);
                    CivBot.Sleep(1000);
                    if (!(BotLocaliztation.ConfirmLocation(location: ScreenLocation.StagingRoom))) { break; }
                    CivBot.MoveAndClick(CivButton.DifficultyBox);
                    CivBot.MoveAndClick(CivButton.DifficultyEmperor);
                    CivBot.MoveAndClick(CivButton.LeaderChoice);
                    CivBot.MoveAndClick(CivButton.LeaderChoiceScroll);
                    CivBot.MoveAndClick(CivButton.AmericaLeaderChoice);
                    CivBot.MoveAndClick(CivButton.Chatinput);
                    CivBot.Inputtext("Ocr only scans bottom row");
                    CivBot.Enter();
                    CivBot.Inputtext("Long names and russians can be a problem for detection");
                    CivBot.Enter();
                    CivBot.Inputtext("Change settings and text responses at settings.txt folder");
                    CivBot.Enter();
                    CivBot.Inputtext("Starting chat spam, can change it on loop in settings instead of on Connected");
                    CivBot.Enter();
                    CivBot.Inputtext("Connected");
                    CivBot.Enter();
                    if ((BotLocaliztation.ConfirmLocation(location: ScreenLocation.StagingRoom)))
                    {
                        setupcomplete = true;
                        break;
                    }

                }

            }


        }
        public static void Lobbyspam(List<string> text, int delay)
        {
            CivBot.MoveAndClick(CivButton.Chatinput);
            CivBot.Enter();
            CivBot.Sleep(200);
            for (int i = 0; i < text.Count; i++)
            {
                CivBot.Inputtext(text[i]);
                CivBot.Sleep(delay - 1000);
                CivBot.Enter();
                CivBot.Sleep(1000);
            }

        }
    }





}

