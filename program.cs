using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using OCR_logic;


namespace Gamebot
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();


        static void Main(string[] args)
        {

            OCR_logic.Coordinates coordinates = new Coordinates();
            OcrReader picturecords = new OcrReader();
            System.Threading.Thread.Sleep(2000);
            string filePath = "test_screenshot.png";
            SetProcessDPIAware();
            picturecords.TakeWindowRelativeScreenshot(filePath, picturecords.Location_header);
            string test = OcrReader.OCR_reading(filePath);
            System.Console.WriteLine(test);
            /*System.Threading.Thread.Sleep(2000);
            MoveMouseTo(Coordinates.Multiplayer);
            System.Threading.Thread.Sleep(500);
            Click();
            //Process.Start(scriptPath, $"{x} {y}");
            //OCR_logic.OcrReader.testing();*/
        }
        static void MoveMouseTo(Coordinates cords)
        {
            int x = cords.x;
            int y = cords.y;
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "test.ahk");
            System.Console.WriteLine(scriptPath);
            Process.Start(@"C:\Program Files\AutoHotkey\v2\AutoHotkey64.exe", $"\"{scriptPath}\" {x} {y}");
        }
        static void Click()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "click.ahk");
            System.Console.WriteLine(scriptPath);
            Process.Start(@"C:\Program Files\AutoHotkey\v2\AutoHotkey64.exe", $"\"{scriptPath}\"");
        }

    }
}
