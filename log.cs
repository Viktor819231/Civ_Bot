using System.Text;

public static class Logger
{
    private static StreamWriter? logWriter;
    private static StreamWriter? statsWriter;
    private static readonly object lockObj = new object();
    private static DateTime sessionStartTime;

    public static void Initialize(string logFilePath = "bot_log.txt", string statsFilePath = "bot_stats.txt")
    {
        try
        {
            sessionStartTime = DateTime.Now;
            
            logWriter = new StreamWriter(logFilePath, append: true)
            {
                AutoFlush = true
            };

            statsWriter = new StreamWriter(statsFilePath, append: true)
            {
                AutoFlush = true
            };

            var multiWriter = new MultiTextWriter(Console.Out, logWriter);
            Console.SetOut(multiWriter);

            Console.WriteLine($"--- Log started at {DateTime.Now} ---");
            LogStat($"=== Session started: {sessionStartTime:yyyy-MM-dd HH:mm:ss} ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize logger: {ex.Message}");
        }
    }
    public static void LogStat(string stat)
    {
        if (statsWriter != null)
        {
            lock (lockObj)
            {
                statsWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {stat}");
            }
        }
    }

    public static void LogToFile(string message)
    {
        if (logWriter != null)
        {
            lock (lockObj)
            {
                logWriter.Write(message);
            }
        }
    }

    public static void LogLineToFile(string? message)
    {
        if (logWriter != null)
        {
            lock (lockObj)
            {
                logWriter.WriteLine(message);
            }
        }
    }

    public static void Close()
    {
        try
        {
            var sessionEnd = DateTime.Now;
            var sessionLength = sessionEnd - sessionStartTime;
            if (statsWriter != null)
            {
                lock (lockObj)
                {
                    statsWriter.WriteLine($"Session ended: {sessionEnd:yyyy-MM-dd HH:mm:ss}");
                    statsWriter.WriteLine($"Session length: {sessionLength.Hours}h {sessionLength.Minutes}m {sessionLength.Seconds}s");
                    statsWriter.WriteLine($"Connected recognized: {BotStats.TotalConnections}");
                    statsWriter.WriteLine($"Restarts: {BotStats.TotalRestarts}");
                    statsWriter.WriteLine($"Relobbies: {BotStats.TotalRelobbies}");
                    statsWriter.WriteLine($"=== End of Session ===");
                    statsWriter.WriteLine();
                }
            }
        }
        catch { }
        
        logWriter?.Close();
        statsWriter?.Close();
    }

    private class MultiTextWriter : TextWriter
    {
        private readonly TextWriter console;
        private readonly TextWriter file;

        public MultiTextWriter(TextWriter console, TextWriter file)
        {
            this.console = console;
            this.file = file;
        }

        public override Encoding Encoding => console.Encoding;

        public override void Write(char value)
        {
            lock (lockObj)
            {
                console.Write(value);
                file.Write(value);
            }
        }

        public override void Write(string? value)
        {
            lock (lockObj)
            {
                console.Write(value);
                file.Write(value);
            }
        }

        public override void WriteLine(string? value)
        {
            lock (lockObj)
            {
                console.WriteLine(value);
                file.WriteLine(value);
            }
        }

        public override void WriteLine()
        {
            lock (lockObj)
            {
                console.WriteLine();
                file.WriteLine();
            }
        }

        public override void Flush()
        {
            lock (lockObj)
            {
                console.Flush();
                file.Flush();
            }
        }
    }


    
}