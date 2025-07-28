using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using NavigationAndLocations;


namespace Gamebot
{
    class Program
    {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();


        static void Main(string[] args)
        {
            Bot.Sleep(2000);
            SetProcessDPIAware();
            Bot.NavigateTo(ScreenLocation.Screen_SetupMulti);
            Bot.MoveAndClick(Button.Button_LobbyNameInputField);
            Bot.MoveAndClick(Button.Button_Chatinput);
            Bot.Inputtext("PlaceHolder Placeholder! PlaceHolder [Color_Green] Placeholder. Placeholder");
            Bot.Enter();
            Bot.Sleep(3000);
            Bot.Inputtext("PlaceHolder Placeholder! PlaceHolder [Color_Green] Placeholder. Placeholder");
            Bot.Enter();
            Bot.Sleep(3000);
            Bot.Inputtext("PlaceHolder SomeThingSomething Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something Something ");
            Bot.Enter();
            Bot.Sleep(1000);
            static void SetupNewLobby(string LobbyName)
            {
                Bot.NavigateTo(ScreenLocation.Screen_SetupMulti);
                Bot.EraseExistingText();
                Bot.Inputtext("FFACIV.com");
                Bot.MoveAndClick(Button.Button_Loadgame);
                Bot.MoveAndClick(Button.Button_GameConfigfile);
                Bot.MoveAndClick(Button.Button_Loadgame_hostgame);
                Bot.backtrack();
                Bot.MoveAndClick(Button.Button_HostLobby);
                Bot.MoveAndClick(Button.Button_DifficultyBox);
                Bot.MoveAndClick(Button.Button_DifficultyEmperor);
                Bot.MoveAndClick(Button.Button_LeaderChoice);
                Bot.MoveAndClick(Button.Button_LeaderChoiceScroll);
                Bot.MoveAndClick(Button.Button_AmericaLeaderChoice);

            }
        }



       

    }
}
