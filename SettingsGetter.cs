using System.IO;


namespace Gamebot
{
    
    class Filereader
    {

        public static string filepath_settings = GetSettingsPath();
        public static Settings Getsettings()
        {

            string[] settings_rows = ParseSettingText();
            Settings settings = new Settings(settings_rows.Length - 1);
            int msg_indx = 0;
            for (int i = 0; i < settings_rows.Length; i++)
            {
                (string name, string param) = ParseLine(settings_rows[i]);
                if (name == "msg")
                {
                    settings.Messages[msg_indx] = param;
                    msg_indx += 1;
                }
                else if (name == "LobbyName")
                {
                    settings.LobbyName = param;
                }


            }
            return settings;
        }
        public static string[] ParseSettingText()
        {
            string[] lines = File.ReadAllLines(filepath_settings);
            return lines;
        }

        public static (string settingname, string param) ParseLine(string line)
        {
            if (!(line == ""))
            {
                int IndexOfBreaker = line.IndexOf(":::");
                int lenght = line.Length;
                string settingsname = line.Substring(0, IndexOfBreaker);
                string setting_param = line.Substring(IndexOfBreaker + 3).Trim('"');
                return (settingsname, setting_param);

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