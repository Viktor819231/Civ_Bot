using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using NavigationAndLocations;
using OCR;
using System.Windows.Forms;
using winform;

namespace Gamebot
{
    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main(string[] args)
        {
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());


            /*
            CivBot.Sleep(2000);

            SetupNewLobby("FFACIV.com");
            int timedelay = 5000;
            string first = "PlaceHolder SomeThingSomething Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something ";
            string second = "PlaceHolder Placeholder! PlaceHolder [Color_Green] Placeholder. Placeholder";
            string third = "PlaceHolder Placeholder! PlaceHolder [Color_Green] Placeholder. Placeholder";
            string[] messages = { first, second, third };
            Lobbyspam(messages, timedelay);*/
        }
        public static void SetupNewLobby(string LobbyName)
        {
            System.Diagnostics.Debug.WriteLine("Reached here");
            CivBot.Sleep(1000);
            CivBot.NavigateTo(CivScreenLocation.Screen_SetupMulti);
            CivBot.MoveAndClick(CivButton.Button_LobbyNameInputField);
            CivBot.EraseExistingText();
            CivBot.Inputtext(LobbyName);
            CivBot.MoveAndClick(CivButton.Button_Loadgame);
            CivBot.MoveAndClick(CivButton.Button_GameConfigfile);
            CivBot.MoveAndClick(CivButton.Button_Loadgame_hostgame);
            CivBot.backtrack();
            CivBot.MoveAndClick(CivButton.Button_HostLobby);
            CivBot.MoveAndClick(CivButton.Button_DifficultyBox);
            CivBot.MoveAndClick(CivButton.Button_DifficultyEmperor);
            CivBot.MoveAndClick(CivButton.Button_LeaderChoice);
            CivBot.MoveAndClick(CivButton.Button_LeaderChoiceScroll);
            CivBot.MoveAndClick(CivButton.Button_AmericaLeaderChoice);
        }
        public static void Lobbyspam(string[] text, int delay)
        {
            CivBot.MoveAndClick(CivButton.Button_Chatinput);
            CivBot.EraseExistingText(20);
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

