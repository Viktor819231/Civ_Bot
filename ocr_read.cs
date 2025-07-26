using System;
using System.Drawing;
using System.Diagnostics;
using Tesseract;
using System.Runtime.CompilerServices;
using System.Configuration.Assemblies;
using System.Dynamic;

namespace OCR_logic
{
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
    class OcrReader
    {
        private static ChatMessage lastest_msg = new ChatMessage("xxx", "xxx");
        private static TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        public static void testing()
        {
            get_info_and_store();
        }
        static void get_info_and_store()
        {
            string test = OCR_reading(@"C:\Users\Vikto\OneDrive\Pictures\Screenshots\Screenshot 2025-07-25 145811.png");
            var OCR_text = OcrParseIntoStringTouple(test);

            if (OCR_text.Count > 0)
            {
                ChatMessage latest_from_ocr = OCR_text[OCR_text.Count - 1];
                if (lastest_msg.Verify_Unique_Msg(latest_from_ocr) && lastest_msg.Verify_not_bot_msg(latest_from_ocr))
                {
                    lastest_msg.replacelatest_msg(latest_from_ocr);
                    Console.WriteLine(lastest_msg.GetPlayer());
                    Console.WriteLine(lastest_msg.GetMsg()); 
                }
            }
        }
        static void TakeScreenshot(string filePath)
        {
            int x = 100, y = 100, width = 500, height = 200;
            using Bitmap bmp = new Bitmap(width, height);
            using Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            bmp.Save(filePath);
        }
        static List<ChatMessage> OcrParseIntoStringTouple(string ocrText)
        {
            var lines = ocrText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            var messages = new List<ChatMessage>();

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    string player = parts[0].Trim();
                    string msg = parts[1].Trim();
                    messages.Add(new ChatMessage(msg, player));
                }
            }
            return messages;
        }
        static string OCR_reading(string imagePath)
        {
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            string text = page.GetText();
            return text;

        }

    }
}