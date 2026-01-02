using System;
using System.Threading;

namespace Gamebot
{
    class Timekeeping
    {
        // Shared flags for communication between threads
        public static bool ShouldRehostLobby = false;
        public static bool ShouldRestartGame = false;
        public static DateTime LatestLobbyrehost = DateTime.Now;
        public static DateTime LatestGamerestart = DateTime.Now;

        public static int GetTimeToNextRestart()
        {
            int timepassedsincelast = (int)(DateTime.Now - LatestGamerestart).TotalMinutes;
            int timetonextrestart = Program.settings.TimeBetweenGamerestart - timepassedsincelast;
            return timetonextrestart;
        }
        public static int GetTimeToNextRelobby()
        {
            int timepassedsincelobbycreated = (int)(DateTime.Now - LatestLobbyrehost).TotalMinutes;
            int timetonextrestart = Program.settings.TimeBetweenRelobby- timepassedsincelobbycreated;
            return timetonextrestart;

        }

        public static void ResetLobbyTimer()
        {
            LatestLobbyrehost = DateTime.Now;
            ShouldRehostLobby = false;
        }

        public static void ResetGameRestartTimer()
        {
            LatestGamerestart = DateTime.Now;
            ShouldRestartGame = false;

        }
        static public void Starttimers()
        {
            Thread timer = new Thread(() =>
            {
                Thread.CurrentThread.Name = "TimerThread";
                LatestLobbyrehost = DateTime.Now;
                LatestGamerestart = DateTime.Now;

                while (true)
                {
                    DateTime now = DateTime.Now;
                    if ((now - LatestGamerestart).TotalMinutes >= Program.settings.TimeBetweenGamerestart)
                    {
                        System.Console.WriteLine("setting restargame to true");
                        ShouldRestartGame = true;
                    }

                    if ((now - LatestLobbyrehost).TotalMinutes >= Program.settings.TimeBetweenRelobby)
                    {
                        System.Console.WriteLine("setting relobby to true");
                        ShouldRehostLobby = true;

                    }


                    Thread.Sleep(15000);
                }
            });

            timer.IsBackground = true;
            timer.Start();
        }
    }
}