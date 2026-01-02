using System;

public static class BotStats
{
    private static int totalConnections = 0;
    private static int totalRestarts = 0;
    private static int totalRelobbies = 0;

    public static int TotalConnections => totalConnections;
    public static int TotalRestarts => totalRestarts;
    public static int TotalRelobbies => totalRelobbies;

    public static void IncrementConnections()
    {
        totalConnections++;
        Console.WriteLine($"[STATS] Total Connections: {totalConnections}");
    }

    public static void IncrementRestarts()
    {
        totalRestarts++;
        Console.WriteLine($"[STATS] Total Restarts: {totalRestarts}");
    }

    public static void IncrementRelobbies()
    {
        totalRelobbies++;
        Console.WriteLine($"[STATS] Total Relobbies: {totalRelobbies}");
    }

    public static void PrintStats()
    {
        Console.WriteLine("=== Bot Statistics ===");
        Console.WriteLine($"Total Connections Detected: {totalConnections}");
        Console.WriteLine($"Total Game Restarts: {totalRestarts}");
        Console.WriteLine($"Total Lobby Recreations: {totalRelobbies}");
        Console.WriteLine("=====================");
    }
}
