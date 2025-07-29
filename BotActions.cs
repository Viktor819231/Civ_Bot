using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Emit;
using NavigationAndLocations;
using OCR;

namespace Gamebot
{
    class CivBot
    {
        public static void Sleep(int x)
        {
            Thread.Sleep(x);
        }
        public static void backtrack()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Backtrack.exe");
            Process.Start(scriptPath);
            Sleep(300);
        }
        public static void EraseExistingText(int CharactersToErase = 15)
        {

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "HitBackspace.exe");
            for (int i = 0; i < CharactersToErase; i++)
            {
                Process.Start(scriptPath);
                Sleep(200);
            }
            Sleep(200);
        }
        public static void Enter()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Enter.exe");
            Process.Start(scriptPath);
            Sleep(200);
        }
        public static void Inputtext(string txt)
        {

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptPath, arg);
            Sleep(1000);
        }

        public static void MoveMouseTo(LocationInGame cords)
        {
            int x = cords.x_left;
            int y = cords.y_top;
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "MoveMouseTo.exe");
            string args = $"{x} {y}";
            Process.Start(scriptPath, args);
            Sleep(200);

        }
        public static void Click()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "click.exe");
            Process.Start(scriptPath);
            Sleep(200);
        }
        public static void HitEscapeKey()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "Backtrack.exe");
            Process.Start(scriptPath);
            Sleep(100);
        }

        public static void MoveAndClick(LocationInGame cords)
        {
            MoveMouseTo(cords);
            System.Threading.Thread.Sleep(150);
            Click();
            Sleep(500);
        }



    }



}