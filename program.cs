using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NavigationAndLocations;


namespace Gamebot
{
    public class Settings
    {
        public string[] Messages;
        public string LobbyName;
        public Settings(int Amountofmsgs)
        {
            Messages = new string[Amountofmsgs];
        }

    }
    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        static bool stopRequested = false;
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
            int timedelay = 5000;
            bool lobbycheck = Navigation.ConfirmLocation(Location.Screen_StagingRoom);
            bool loopforever = true;
            while (loopforever)
            {
                while (lobbycheck)
                {
                    Lobbyspam(settings.Messages, timedelay);
                    lobbycheck = Navigation.ConfirmLocation(Location.Screen_StagingRoom);
                }
            }


        }

        public static void SetupNewLobby(string LobbyName)
        {

            CivBot.Sleep(1000);
            Navigation.NavigateTo(Location.Screen_SetupMulti);
            CivBot.MoveAndClick(CivButton.LobbyNameInputField);
            CivBot.EraseExistingText();
            CivBot.Inputtext(LobbyName);
            CivBot.MoveAndClick(CivButton.Loadgame);
            CivBot.MoveAndClick(CivButton.GameConfigfile);
            CivBot.MoveAndClick(CivButton.Loadgame_hostgame);
            CivBot.backtrack();
            CivBot.MoveAndClick(CivButton.HostLobby);
            CivBot.MoveAndClick(CivButton.DifficultyBox);
            CivBot.MoveAndClick(CivButton.DifficultyEmperor);
            CivBot.MoveAndClick(CivButton.LeaderChoice);
            CivBot.MoveAndClick(CivButton.LeaderChoiceScroll);
            CivBot.MoveAndClick(CivButton.AmericaLeaderChoice);
        }
        public static void Lobbyspam(string[] text, int delay)
        {
            CivBot.MoveAndClick(CivButton.Chatinput);
            CivBot.Enter();
            CivBot.Sleep(200);
            for (int i = 0; i < text.Length; i++)
            {
                CivBot.Inputtext(text[i]);
                CivBot.Sleep(delay - 1000);
                CivBot.Enter();
                CivBot.Sleep(1000);
            }

        }
    }





}

