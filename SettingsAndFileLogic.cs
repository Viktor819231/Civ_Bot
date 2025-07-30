using System.Drawing.Drawing2D;
using System.IO;


namespace Gamebot
{

    class Filereader
    {

        public static string filepath_settings = GetSettingsPath();
        public static Settings Getsettings()
        {

            string[] settings_rows = ParseSettingText();
            Settings settings = new Settings();
            for (int i = 0; i < settings_rows.Length; i++)
            {
                (string name, string param) = ParseLine(settings_rows[i]);
                if (name == "msg")
                {
                    settings.Messages.Add(param);
                }
                else if (name == "LobbyName")
                {
                    settings.LobbyName = param;
                }
                else if (name == "BotSteamUserName")
                {
                    settings.Botname = param;
                }
                else if (name == "RespondIf" && param.Contains(";"))
                {
                    settings.ConditionalAndResponse.Add(ParseConditional(param)); // Use the right function!
                }
                else if (name == "DefaultSleep")
                {
                    settings.SleepBetweenMsgs = int.Parse(param);
                }
                else if (name == "WaitAfterConnected")
                {
                    settings.timeWaitAfterConnected = int.Parse(param);
                }
                else if (name == "OnlyAdvertiseOnConnected")
                {
                    if (param.Contains("true")) { settings.OnlyAdvertiseOnConnected = true; } else { settings.OnlyAdvertiseOnConnected = false; }

                }
                else if (name == "AdverTiseOnConnected")
                {
                    if (param.Contains("true")) { settings.AdverTiseOnConnected = true; } else { settings.AdverTiseOnConnected = false; }
                }
                else if (name == "Botspeed")
                {
                    settings.Botspeed = int.Parse(param);
                }
                else if (name == "ScanChatEvery")
                {
                    settings.ScanChatEvery = int.Parse(param);

                }
            }
            return settings;
        }
        public static string[] ParseSettingText()
        {
            string[] lines = File.ReadAllLines(filepath_settings);
            return lines;
        }
        public static (string conditional, string response) ParseConditional(string param)
        {
            int indexofbreaker = param.IndexOf(";");
            string conditionaltext = param.Substring(0, indexofbreaker).Trim().Trim('"');
            string response = param.Substring(indexofbreaker + 1).Trim().Trim('"');
            return (conditionaltext, response);
        }

        public static (string settingname, string param) ParseLine(string line)
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
        public static string GetSettingsPath()
        {

            if (File.Exists("settings.txt"))
                return "settings.txt";

            // Try project root
            string projectPath = Path.Combine("..", "..", "..", "settings.txt");
            if (File.Exists(projectPath))
                return projectPath;

            return "settings.txt";
        }


    }


}