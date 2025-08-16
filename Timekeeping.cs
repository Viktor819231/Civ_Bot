using System;
using System.Threading;

namespace Gamebot
{
    class Timekeeping
    {
        // Shared flags for communication between threads
        public static bool ShouldRehostLobby = false;
        public static bool ShouldRestartGame = false;
        
        
        static public void Starttimers()
        {
            Thread timer = new Thread(() =>
            {
                Thread.CurrentThread.Name = "TimerThread";
                DateTime Latestgamcheck = DateTime.Now;
                DateTime LatestLobbyrehost = DateTime.Now;
                DateTime LatestGamerestart = DateTime.Now;

                while (true)
                {
                    DateTime now = DateTime.Now;
                    if ((now - LatestLobbyrehost).TotalMinutes >= 180)
                    {
                        ShouldRehostLobby = true;
                        LatestLobbyrehost = now;

                    }

                    if ((now - LatestGamerestart).TotalMinutes >= 300)
                    {
                        ShouldRestartGame = true;
                        LatestGamerestart = now;
                    }

                    Thread.Sleep(10000);
                }
            });

            timer.IsBackground = true; // Dies when main program exits
            timer.Start();
        }
    }
}