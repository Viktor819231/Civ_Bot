using System.CodeDom;

namespace Gamebot
{

    public class Settings
    {

        public List<(string, string)> ConditionalAndResponse = new List<(string, string)>();
        public string Civfilepath;
        public string BotName;
        public string BotRegion;
        public int Botspeed;
        public bool AlwaysConfirmLocationBeforeInput;
        public int WaittimeafterLaunch;     
        public string LobbyName;
        public int TimeBetweenRelobby;
        public int TimeBetweenGamerestart;
        public List<string> Messages = new List<string>();
        public int SleepBetweenMsgCycles;
        public int ScanChatEvery;
        public bool AdverTiseOnConnected;
        public bool OnlyAdvertiseOnConnected;
        public int timeWaitAfterConnected;
        public int OcrOffset;
        public int creditxleft;
        public int creditxright;
        public int creditytop;
        public int creditybottom;
        public int creditbuttonx;
        public int creditbuttony;

        public bool debugmode;
        
        // Public property for debug mode
        public bool DebugMode => debugmode;







        public static string filepath_settings = SettingsPath();
        public Settings()
        {
            // Load from settings.txt first
            string[] settings_rows = File.ReadAllLines(filepath_settings);
            for (int i = 0; i < settings_rows.Length; i++)
            {
                (string name, string param) = ParseIntoNameAndParameter(settings_rows[i]);
                if (name == "path")
                {
                    Civfilepath = param;
                }
                else if (name == "BotName")
                {
                    BotName = param;
                }
                else if (name == "Bot-Region")
                {
                    BotRegion = param;
                }
                else if (name == "Botspeed")
                {
                    Botspeed = int.Parse(param);
                }
                else if (name == "AlwaysConfirmLocationBeforeInput")
                {
                    if (param.Contains("true")) { AlwaysConfirmLocationBeforeInput = true; } else { AlwaysConfirmLocationBeforeInput = false; }
                }
                else if (name == "WaittimeafterLaunch")
                {
                    WaittimeafterLaunch = int.Parse(param);
                }
                else if (name == "LobbyName")
                {
                    LobbyName = param;
                }
                else if (name == "TimeBetweenRelobby")
                {
                    TimeBetweenRelobby = int.Parse(param);

                }
                else if (name == "TimeBetweenGamerestart")
                {
                    TimeBetweenGamerestart = int.Parse(param);
                }
                else if (name == "msg")
                {
                    Messages.Add(param);
                }
                else if (name == "SleepBetweenMsgCycles")
                {
                    SleepBetweenMsgCycles = int.Parse(param);
                }
                else if (name == "ScanChatEvery")
                {
                    ScanChatEvery = int.Parse(param);

                }
                else if (name == "AdverTiseOnConnected")
                {
                    if (param.Contains("true")) { AdverTiseOnConnected = true; } else { AdverTiseOnConnected = false; }
                }
                else if (name == "OnlyAdvertiseOnConnected")
                {
                    if (param.Contains("true")) { OnlyAdvertiseOnConnected = true; } else { OnlyAdvertiseOnConnected = false; }

                }
                else if (name == "WaitAfterConnected")
                {
                    timeWaitAfterConnected = int.Parse(param);
                }
                else if (name == "RespondIf")
                {
                    ConditionalAndResponse.Add(ConditionalAndResponse_Parse(param));
                }
                  else if (name == "OCRoffset")
                {
                    OcrOffset = int.Parse(param);
                }
                else if (name == "creditxleft")
                {
                    creditxleft = int.Parse(param);
                }
                else if (name == "creditxright")
                {
                    creditxright = int.Parse(param);
                }
                else if (name == "creditytop")
                {
                    creditytop = int.Parse(param);
                }
                else if (name == "creditybottom")
                {
                    creditybottom = int.Parse(param);
                }
                else if (name == "creditbuttonx")
                {
                    creditbuttonx = int.Parse(param);
                }
                else if (name == "creditbuttony")
                {
                    creditbuttony = int.Parse(param);
                }
                else if (name == "debugmode")
                {
                    if (param.Contains("true")) { debugmode = true; } else { debugmode = false; }

                }
            }
            
            // Try to override with Firebase config (async call)
            Task.Run(async () => await LoadFromFirebase()).Wait();

        }
        
        private async Task LoadFromFirebase()
        {
            try
            {
                string lobbyNameJson = await Databasecommuncation.GetData("bot-config/lobbyName");
              
                if (lobbyNameJson != null && lobbyNameJson != "null")
                {
                    string? firebaseLobbyName = System.Text.Json.JsonSerializer.Deserialize<string>(lobbyNameJson);
                    if (!string.IsNullOrWhiteSpace(firebaseLobbyName))
                    {
                        LobbyName = firebaseLobbyName;
                    }
                }
                
                // Get advertising texts
                string adsJson = await Databasecommuncation.GetData("bot-config/advertisingTexts");
                if (adsJson != null && adsJson != "null")
                {
                    try
                    {
                        // Try to parse as array first
                        var firebaseMessages = System.Text.Json.JsonSerializer.Deserialize<List<string>>(adsJson);
                        if (firebaseMessages != null && firebaseMessages.Count > 0)
                        {
                            Messages = firebaseMessages;
                        }
                    }
                    catch
                    {
                        // If array parsing fails, try parsing as object with keys
                        try
                        {
                            var firebaseMessagesObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(adsJson);
                            if (firebaseMessagesObj != null && firebaseMessagesObj.Count > 0)
                            {
                                Messages = firebaseMessagesObj.Values.ToList();
                            }
                        }
                        catch
                        {
                            // Silently keep existing settings if parse fails
                        }
                    }
                }
            }
            catch
            {
                // Silently keep existing settings if Firebase fails
            }
        }
        
        // Public method to refresh settings from Firebase (called periodically)
        public async Task RefreshFromFirebase()
        {
            await LoadFromFirebase();
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

        public void Validatesettings()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(BotName))
            {
                errors.Add("BotName not set in settings.txt - Add line: BotName:::\"YourBotName\"");
            }
            
            if (string.IsNullOrWhiteSpace(BotRegion))
            {
                errors.Add("Bot-Region not set in settings.txt - Add line: Bot-Region:::\"EU-West\" (or your region)");
            }
            
            if (string.IsNullOrWhiteSpace(LobbyName))
            {
                errors.Add("LobbyName not set in settings.txt");
            }
            if (string.IsNullOrWhiteSpace(Civfilepath))
            {
                errors.Add("Filepath to civ launcher not set");
            }
            if (Botspeed <= 0)
            {
                errors.Add("Botspeed must be greater than 0.");
            }

            if (ScanChatEvery <= 0)
            {
                errors.Add("ScanChatEvery must be greater than 0.");
            }

            if (Messages == null || Messages.Count == 0)
            {
                errors.Add("No advertising messages found in settings.txt");
            }
            
            if (errors.Count > 0)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("SETTINGS VALIDATION FAILED:");
                Console.WriteLine("========================================");
                foreach (var error in errors)
                {
                    Console.WriteLine(" ✗ " + error);
                }
                Console.WriteLine("\nPlease fix the errors in settings.txt and restart the bot.");
                Console.WriteLine("========================================\n");
                
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("✓ Settings successfully loaded and validated from settings.txt");
            }

        }

        public void Printsettings()
        {
            Console.WriteLine(Civfilepath + " Filepath");
            Console.WriteLine(Botspeed + " Botspeed");
            Console.WriteLine(AlwaysConfirmLocationBeforeInput + " AlwaysConfirmLocationBeforeInput");
            Console.WriteLine(WaittimeafterLaunch + " WaittimeafterLaunch");
            Console.WriteLine(LobbyName + " lobbyname");
            Console.WriteLine(TimeBetweenRelobby + " TimeBetweenRelobby");
            Console.WriteLine(TimeBetweenGamerestart + " TimeBetweenGamerestart");
            Console.WriteLine(OnlyAdvertiseOnConnected + " Only on connect bool");
            Console.WriteLine(SleepBetweenMsgCycles + " SleepBetweenMsgCycles");
            Console.WriteLine(ScanChatEvery + " ScanChatEvery");
            Console.WriteLine(AdverTiseOnConnected +  " AdvertiseOnConnected");
            Console.WriteLine(timeWaitAfterConnected + " timeWaitafterConnected");
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