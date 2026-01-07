
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using NavigationAndLocations;
using OCR;

namespace Gamebot
{
    public struct MsgAndUser
    {
        public string player;
        public string msg;
        public MsgAndUser(string playerarg, string msgarg)
        {
            if (string.IsNullOrWhiteSpace(playerarg))
            {
                player = "ruskie";
            }
            else
            {
                player = playerarg;
            }
            if (string.IsNullOrWhiteSpace(msgarg))
            {
                msg = "ruskieMessage";
            }
            else
            {
                msg = msgarg;
            }


        }
    }
    class CivBotChatter
    {

        public static List<MsgAndUser> current_msgs = new List<MsgAndUser>();
        public static List<MsgAndUser> latest_msgs = new List<MsgAndUser>();
        public static void LoopMsgs_ScanAndRespond()
        {
            int msgcount = Program.settings.Messages.Count();
            int defaultsleep = Program.settings.SleepBetweenMsgCycles;
            bool OnlyOnConnect = Program.settings.OnlyAdvertiseOnConnected;
            int Howlongbetweenscans = Program.settings.ScanChatEvery;

            if (!OnlyOnConnect)
            {
                bool postmsgs = true;
                int Scantimes = defaultsleep / Howlongbetweenscans;
                CivBot.MoveAndClick(CivButton.Chatinput);
                System.Console.WriteLine("Will Post again in: " + defaultsleep/1000 + "seconds");
                for (int j = 0; j < Scantimes; j++)
                {
                    if (ScanChat_AndRespond())
                    {
                        postmsgs = false;
                        break;
                    }
                    CivBot.Sleep(Howlongbetweenscans);
                }
                if (postmsgs)
                {
                justloopthrubasicadds(sleepbetweenmsgs: 250);
                }


            }
            else
            {
                ScanChat_AndRespond();
                CivBot.Sleep(Howlongbetweenscans);

            }
        }

        public static void justloopthrubasicadds(int sleepbetweenmsgs)
        {
            foreach (var item in Program.settings.Messages)
            {
                CivBot.Enter();
                CivBot.Inputtext(item);
                CivBot.Enter();
                CivBot.Sleep(sleepbetweenmsgs);
            }

        }
        public static bool ScanChat_AndRespond()
        {
            try
            {
                UpdateMsgAndUser();
                
                // Check if we have any messages
                if (current_msgs.Count == 0)
                {
                    return false;
                }
                
                if (Verify_NewMsg())
                {
                    string lastMsg = current_msgs.Last().msg;

                    (bool conditional, string response) = GetResponseIfConditional(lastMsg);

                    if (conditional)
                    {
                        CivBot.Enter();
                        CivBot.Inputtext(response);
                        CivBot.Enter();
                        UpdateMsgAndUser();
                        return true;
                    }
                }
                // Check for player connection (always check, not just in else)
                if (Program.settings.AdverTiseOnConnected && current_msgs.Count > 0 && current_msgs.Last().msg == "Connected")
                {
                    string playerName = current_msgs.Last().player;
                    System.Console.WriteLine("Connected Recognized, will post in:" + Program.settings.timeWaitAfterConnected / 1000 + " seconds");
                    Logger.LogStat($"Player connection detected: {playerName}");
                    BotStats.IncrementConnections();
                    
                    // Log player connection to Firebase (non-blocking)
                    Task.Run(async () => await Databasecommuncation.LogPlayerConnection(playerName));
                    
                    CivBot.Sleep(Program.settings.timeWaitAfterConnected);

                    System.Console.WriteLine($"Posting {Program.settings.Messages.Count} messages from Firebase/settings...");
                    justloopthrubasicadds(sleepbetweenmsgs: 250);
                    System.Console.WriteLine("Finished posting messages after connection");
                    return true;
                }
                
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ScanChat_AndRespond: {e.Message}");
                Console.WriteLine($"Stack trace: {e.StackTrace}");
                return false;
            }
        }

        public static (bool containcheck, string response) GetResponseIfConditional(string usermsg)
        {

            for (int i = 0; i < Program.settings.ConditionalAndResponse.Count; i++)
            {

                (string cond, string response) = Program.settings.ConditionalAndResponse[i];
                if (usermsg == cond)
                {
                    return (true, response);
                }


            }

            return (false, "");

        }

        public static bool Verify_NewMsg()
        {
            if (latest_msgs.Count == 0 || current_msgs.Count == 0)
                return current_msgs.Count > latest_msgs.Count;

            var latestLast = latest_msgs.Last();
            var currentLast = current_msgs.Last();

            return !(latestLast.player == currentLast.player && latestLast.msg == currentLast.msg);
        }

        public static bool Respondif_Connectedmsg()
        {
            if (current_msgs.Last().msg == "Connected")
            {
                return true;
            }
            return false;

        }
        public static void UpdateMsgAndUser()
        {

            Rectangle RectForPictureArea = CivTextBox.ChatText.GetRectanglePictureBox();
            string chattext = ImgToText.TextAt(RectForPictureArea, CivTextBox.ChatText.filename).TrimEnd();
            Debug.Write(chattext);
            latest_msgs = current_msgs;
            current_msgs = GetChat(chattext);

        }
        public static List<MsgAndUser> GetChat(string chatmsgs)
        {

            string[] AllRowsOfText = chatmsgs.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<MsgAndUser> MsgsInChat = new List<MsgAndUser>();
            for (int i = 0; i < AllRowsOfText.Length; i++)
            {
                if (AllRowsOfText[i].Contains(":"))
                {
                    MsgsInChat.Add(GetMsgAndUser(AllRowsOfText[i]));
                }

            }
            return MsgsInChat;


        }
        public static MsgAndUser GetMsgAndUser(string line)
        {
            int indexofcolon = line.IndexOf(":");
            string player = line.Substring(0, indexofcolon).Trim();
            string msgfromplayer = line.Substring(indexofcolon + 1).Trim();
            return new MsgAndUser { player = player, msg = msgfromplayer };

        }

    }

}

