
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
            int defaultsleep = Program.settings.SleepBetweenMsgs;
            bool OnlyOnConnect = Program.settings.OnlyAdvertiseOnConnected;
            bool OnConnect = Program.settings.AdverTiseOnConnected;
            int Howlongbetweenscans = Program.settings.ScanChatEvery;

            if (!OnlyOnConnect)
            {
                int ScantimesBetwenAds = Math.Max(1, (int)Math.Floor((double)defaultsleep / 1000 / msgcount));
                for (int i = 0; i < msgcount; i++)
                {
                    CivBot.MoveAndClick(CivButton.Chatinput);
                    for (int j = 0; j < ScantimesBetwenAds; j++)
                    {
                        ScanChat_AndRespond();
                        CivBot.Sleep(Howlongbetweenscans);
                        Debug.Write(j);
                    }
                    CivBot.Enter();
                    CivBot.Inputtext(Program.settings.Messages[i]);
                    CivBot.Enter();
                }
            }
            else
            {
                Debug.Write("Its in chat logic");
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
        public static void ScanChat_AndRespond()
        {
            try
            {
                UpdateMsgAndUser();
                if (CheckIfPlayer(current_msgs.Last().player) && Verify_NewMsg())
                {

                    string lastMsg = current_msgs.Last().msg;

                    (bool conditional, string response) = GetResponseIfConditional(lastMsg);

                    if (conditional)
                    {
                        CivBot.Enter();
                        CivBot.Inputtext(response);
                        CivBot.Enter();
                        UpdateMsgAndUser();
                    }
                    else
                    {

                    }
                }
                else
                {
                    
                }
                
                if (Program.settings.AdverTiseOnConnected && current_msgs.Last().msg == "Connected")
                {
                    justloopthrubasicadds(sleepbetweenmsgs: 3000);
                }
            }
            catch (Exception e)
            {

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
        public static bool CheckIfPlayer(string player)
        {
            return player != Program.settings.Botname;
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

