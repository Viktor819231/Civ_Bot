    class ChatMessage
    {
        private string msg;
        private string player;

        public ChatMessage(string text, string playername)
        {
            this.msg = text;
            this.player = playername;
        }
        public string GetMsg()
        {
            return msg;
        }

        public string GetPlayer()
        {
            return player;
        }

        public void replacelatest_msg(ChatMessage message)
        {
            if (Verify_not_bot_msg(message) && Verify_Unique_Msg(message))
            {
                this.msg = message.msg;
                this.player = message.player;
            }
        }

        public bool Verify_not_bot_msg(ChatMessage message)
        {
            if (message.player != "Host_placeholder_name")
            {
                return true;
            }
            return false;
        }
        public bool Verify_Unique_Msg(ChatMessage message)
        {
            if (message.player != "Host_placeholder_name" && message.msg != this.msg)
            {
                return true;
            }
            return false;
        }

        public bool Verify_not_null(string text)
        {
            if (text != null)
            {
                return true;
            }
            return false;

        }
    }