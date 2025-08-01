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
        public bool AlwaysConfirmLocationBeforeInput;

    

        public static string filepath_settings = SettingsPath();
        public Settings()
        {

            string[] settings_rows = File.ReadAllLines(filepath_settings);
            for (int i = 0; i < settings_rows.Length; i++)
            {
                (string name, string param) = ParseIntoNameAndParameter(settings_rows[i]);
                if (name == "msg")
                {
                    Messages.Add(param);
                }
                else if (name == "LobbyName")
                {
                    LobbyName = param;
                }
                else if (name == "BotSteamUserName")
                {
                    Botname = param;
                }
                else if (name == "RespondIf" && param.Contains(";"))
                {
                    ConditionalAndResponse.Add(ConditionalAndResponse_Parse(param)); 
                }
                else if (name == "DefaultSleep")
                {
                    SleepBetweenMsgs = int.Parse(param);
                }
                else if (name == "WaitAfterConnected")
                {
                    timeWaitAfterConnected = int.Parse(param);
                }
                else if (name == "OnlyAdvertiseOnConnected")
                {
                    if (param.Contains("true")) { OnlyAdvertiseOnConnected = true; } else { OnlyAdvertiseOnConnected = false; }

                }
                else if (name == "AdverTiseOnConnected")
                {
                    if (param.Contains("true")) { AdverTiseOnConnected = true; } else { AdverTiseOnConnected = false; }
                }
                else if (name == "Botspeed")
                {
                    Botspeed = int.Parse(param);
                }
                else if (name == "ScanChatEvery")
                {
                    ScanChatEvery = int.Parse(param);

                }
                else if (name == "AlwaysConfirmLocationBeforeInput")
                {
                   if (param.Contains("true")) { AlwaysConfirmLocationBeforeInput = true; } else { AlwaysConfirmLocationBeforeInput = false; }
                }
            }
        
        }
     
        public static (string settingname, string param) ParseIntoNameAndParameter(string line)
        {
            if (line.Contains(":::"))
            {
                int IndexOfBreaker = line.IndexOf(":::");
                string settingsname = line.Substring(0, IndexOfBreaker);
                string setting_param = line.Substring(IndexOfBreaker + 3).Trim('"');
                return (settingsname.Trim(), setting_param.Trim()); 
            }
            return ("emptyline", "emptyline");

        }
        public static string SettingsPath()
        {

            if (File.Exists("settings.txt"))
                return "settings.txt";
                
            string appDirPath = Path.Combine(AppContext.BaseDirectory, "settings.txt");
            if (File.Exists(appDirPath))
                return appDirPath;
                
            string projectPath = Path.Combine("..", "..", "..", "settings.txt");
            if (File.Exists(projectPath))
                return projectPath;

            return "settings.txt";
        }

        public static (string conditional, string response) ConditionalAndResponse_Parse(string param)
        {
            int indexofbreaker = param.IndexOf(";");
            string conditionaltext = param.Substring(0, indexofbreaker).Trim().Trim('"');
            string response = param.Substring(indexofbreaker + 1).Trim().Trim('"');
            return (conditionaltext, response);
        }

    }


}