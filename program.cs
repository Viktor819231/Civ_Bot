using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NavigationAndLocations;
using OCR;


namespace Gamebot
{
    public class Settings
    {
        public List<string> Messages = new List<string>();
        public List<(string, string)> ConditionalAndResponse = new List<(string, string)>();
        public string LobbyName = "LobbyNameNotSet";
        public string Botname = "BotNameNotSet";
        public bool OnlyAdvertiseOnConnected;
        public bool AdverTiseOnConnected;
        public int timeWaitAfterConnected;
        public int SleepBetweenMsgs;
        public int timebetweenscans;
        public int ScanChatEvery;
        public int Botspeed;

    }
    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        public static Settings settings = Filereader.Getsettings();

        static void Main(string[] args)
        {
            CivBot.Sleep(2000);
            SetProcessDPIAware();
            RunBareBonesBot();

        }

        static void RunBareBonesBot()
        {
            SetupNewLobby(settings.LobbyName);
            bool lobbycheck = true;
            bool loopforever = true;
            while (loopforever)
            {

                if (!(CivBotNavigation.ConfirmLocation(Location.Screen_StagingRoom)))
                {
                    CivBotNavigation.NavigateTo(Location.Screen_SetupMulti);
                    SetupNewLobby(settings.LobbyName);
                }
                if (CivBotNavigation.ConfirmLocation(Location.Screen_StagingRoom))
                {
                    lobbycheck = true;
                }
                while (lobbycheck)
                {
                    CivBot.Sleep(200);
                    CivBotChatter.LoopMsgs_ScanAndRespond();
                    lobbycheck = CivBotNavigation.ConfirmLocation(Location.Screen_StagingRoom);
                }

            }
        }
        
        

        public static void SetupNewLobby(string LobbyName)
        {
            bool setupcomplete = false;
            while (!setupcomplete)
            {
                while (true)
                {
                    CivBot.Sleep(1000);
                    CivBotNavigation.NavigateTo(Location.Screen_SetupMulti);
                    if (!(CivBotNavigation.ConfirmLocation(location: Location.Screen_SetupMulti))) { break; }
                    CivBot.MoveAndClick(CivButton.LobbyNameInputField);
                    CivBot.EraseExistingText();
                    CivBot.Inputtext(LobbyName);
                    CivBot.MoveAndClick(CivButton.Loadgame);
                    CivBot.MoveAndClick(CivButton.GameConfigfile);
                    CivBot.MoveAndClick(CivButton.Loadgame_hostgame);
                    CivBot.Sleep(100);
                    CivBot.backtrack();
                    CivBot.Sleep(100);
                    if (!(CivBotNavigation.ConfirmLocation(location: Location.Screen_SetupMulti))) {
                        CivBot.Sleep(3000);
                        if (!(CivBotNavigation.ConfirmLocation(location: Location.Screen_SetupMulti))){
                           break; } 
                        }
                    CivBot.MoveAndClick(CivButton.HostLobby);
                    CivBot.Sleep(1000);
                    if (!(CivBotNavigation.ConfirmLocation(location: Location.Screen_StagingRoom))) { break; }
                    CivBot.MoveAndClick(CivButton.DifficultyBox);
                    CivBot.MoveAndClick(CivButton.DifficultyEmperor);
                    CivBot.MoveAndClick(CivButton.LeaderChoice);
                    CivBot.MoveAndClick(CivButton.LeaderChoiceScroll);
                    CivBot.MoveAndClick(CivButton.AmericaLeaderChoice);
                    CivBot.MoveAndClick(CivButton.Chatinput);
                    CivBot.Inputtext("Ocr only scans bottom row every two sec");
                    CivBot.Enter();
                    CivBot.Inputtext("Long names and russians can be a problem for detection");
                    CivBot.Enter();
                    CivBot.Inputtext("Change settings at settings.txt folder");
                    CivBot.Enter();
                    CivBot.Inputtext("Starting chat spam will post every" +settings.SleepBetweenMsgs+"seconds");
                    CivBot.Enter();
                    if ((CivBotNavigation.ConfirmLocation(location: Location.Screen_StagingRoom)))
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

