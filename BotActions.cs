using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NavigationAndLocations;



namespace Gamebot
{
    public static class CivBot
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    
    public static bool IsCivActive()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        StringBuilder windowTitle = new StringBuilder(256);
        GetWindowText(foregroundWindow, windowTitle, windowTitle.Capacity);
        
        string title = windowTitle.ToString();
        return title.Contains("Civilization V") || title.Contains("Sid Meier");
    }
    
    public static void EnsureCivActive()
    {
        if (!IsCivActive())
        {
            while (!IsCivActive())
            {
                CivBot.Sleep(2000);
            }
        }
    }

        public static void Sleep(int x)
        {
            int modifier = Program.settings.Botspeed;
            Thread.Sleep(x/modifier);
            EnsureCivActive();
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

        public static void QuickInputtext(string txt)
        {

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "SendText.exe");
            string arg = $"\"{txt}\"";
            Process.Start(scriptPath, arg);
            Sleep(100);
        }

        public static void MoveMouseTo(LocationInGame cords)
        {
            int x = cords.x_left;
            int y = cords.y_top;
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "MoveMouseTo.exe");
            string args = $"{x} {y}";
            Process.Start(scriptPath, args);
            Sleep(250);

        }
        public static void Click()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string scriptPath = Path.Combine(projectRoot, "AHK scripts", "click.exe");
            Process.Start(scriptPath);
            Sleep(150);
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
            Sleep(150);
            Click();
            Sleep(300);
        }



    }



}