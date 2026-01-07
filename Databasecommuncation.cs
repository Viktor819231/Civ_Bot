using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Gamebot
{

public class Databasecommuncation
{
    private static readonly HttpClient client = new HttpClient();
    private const string FIREBASE_URL = "https://civbot-954eb-default-rtdb.europe-west1.firebasedatabase.app";
    private static string? _botId = null;
    
    // Get or create a persistent bot ID
    public static async Task<string> GetOrCreateBotId(string region = "EU-West", string botName = "FFACIV Bot")
    {
        if (_botId != null) return _botId;
        
        string botIdFile = "bot-id.txt";
        
        // Get current machine's hash
        string machineGuid = Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", 
            "MachineGuid", 
            Guid.NewGuid().ToString()
        )?.ToString() ?? Guid.NewGuid().ToString();
        
        string hashedId = HashString(machineGuid).Substring(0, 12);
        string expectedBotId = $"bot-{hashedId}";
        
        // Check if we already have a saved bot ID
        if (File.Exists(botIdFile))
        {
            string existingId = File.ReadAllText(botIdFile).Trim();
            if (!string.IsNullOrWhiteSpace(existingId))
            {
                // Validate that the saved ID matches this machine
                if (existingId == expectedBotId)
                {
                    Console.WriteLine($"Using existing bot ID: {existingId}");
                    _botId = existingId;
                    
                    // Update bot info in Firebase with current settings (in case they changed)
                    await UpdateBotInfo(_botId, region, botName);
                    
                    return _botId;
                }
                else
                {
                    Console.WriteLine($"⚠ bot-id.txt contains ID from different machine: {existingId}");
                    Console.WriteLine($"⚠ Generating new bot ID for this machine...");
                }
            }
        }
        
        // Generate new ID for this machine
        Console.WriteLine($"Generated new bot ID: {expectedBotId}");
        
        // Save to file
        File.WriteAllText(botIdFile, expectedBotId);
        _botId = expectedBotId;
        
        // Initialize bot in Firebase
        await InitializeBot(_botId, region, botName);
        
        return _botId;
    }
    
    // Update bot info (name, region) - called on every startup
    private static async Task UpdateBotInfo(string botId, string region, string botName)
    {
        try
        {
            var botInfo = new {
                botName = botName,
                region = region
            };
            
            await UpdateData($"bot-stats/{botId}/info", botInfo);
            Console.WriteLine($"✓ Updated bot info: {botName} ({region})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Could not update bot info: {ex.Message}");
        }
    }
    
    // Initialize bot in Firebase if it doesn't exist
    private static async Task<bool> InitializeBot(string botId, string region, string botName)
    {
        string existingBot = await GetData($"bot-stats/{botId}/info");
        
        if (existingBot != null && existingBot != "null")
        {
            Console.WriteLine($"Bot {botId} already exists in Firebase, updating info...");
            await UpdateBotInfo(botId, region, botName);
            return false;
        }
        
        Console.WriteLine($"Initializing bot {botId} in Firebase...");
        
        var botInfo = new {
            botId = botId,
            botName = botName,
            region = region,
            registeredAt = DateTime.UtcNow.ToString("o")
        };
        
        await UpdateData($"bot-stats/{botId}/info", botInfo);
        
        var botStats = new {
            amountOfConnected = 0,
            lastGameStart = "",
            lastPing = DateTime.UtcNow.ToString("o"),
            lastRelobby = ""
        };
        
        await UpdateData($"bot-stats/{botId}/stats", botStats);
        Console.WriteLine($"Bot {botId} initialized successfully!");
        return true;
    }
    
    // Helper method to update bot stats
    public static async Task UpdateBotStats(string field, object value)
    {
        try
        {
            string botId = await GetOrCreateBotId();
            var update = new Dictionary<string, object> { { field, value } };
            await UpdateData($"bot-stats/{botId}/stats", update);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating bot stats: {ex.Message}");
        }
    }
    
    // Ping the database to show bot is active
    public static async Task PingBot()
    {
        await UpdateBotStats("lastPing", DateTime.UtcNow.ToString("o"));
    }
    
    // Log a game restart
    public static async Task LogGameRestart()
    {
        await UpdateBotStats("lastGameStart", DateTime.UtcNow.ToString("o"));
        Console.WriteLine("Game restart logged to Firebase");
    }
    
    // Log a relobby
    public static async Task LogRelobby()
    {
        await UpdateBotStats("lastRelobby", DateTime.UtcNow.ToString("o"));
        Console.WriteLine("Relobby logged to Firebase");
    }
    
    // Log a player connection
    public static async Task LogPlayerConnection(string playerName)
    {
        try
        {
            string botId = await GetOrCreateBotId();
            
            // Increment connection count
            string statsJson = await GetData($"bot-stats/{botId}/stats");
            int currentCount = 0;
            
            if (statsJson != null && statsJson != "null")
            {
                var stats = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(statsJson);
                if (stats != null && stats.ContainsKey("amountOfConnected"))
                {
                    currentCount = stats["amountOfConnected"].GetInt32();
                }
            }
            
            await UpdateBotStats("amountOfConnected", currentCount + 1);
            
            // Add to history
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string historyPath = $"bot-stats/{botId}/history/{timestamp}";
            await ReplaceData(historyPath, playerName);
            
            Console.WriteLine($"Player connection logged: {playerName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging player connection: {ex.Message}");
        }
    }
    
    private static string HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
    
    public static async Task<string> GetData(string path)
    {
        try
        {
            string url = $"{FIREBASE_URL}/{path}.json";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting data: {ex.Message}");
            return null!;
        }
    }

    public static async Task<string> PostData(string path, object data)
    {
        try
        {
            string url = $"{FIREBASE_URL}/{path}.json";
            string jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error posting data: {ex.Message}");
            return null!;
        }
    }

    public static async Task<bool> ReplaceData(string path, object data)
    {
        try
        {
            string url = $"{FIREBASE_URL}/{path}.json";
            string jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await client.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error replacing data: {ex.Message}");
            return false;
        }
    }
    
    public static async Task<bool> UpdateData(string path, object data)
    {
        try
        {
            string url = $"{FIREBASE_URL}/{path}.json";
            string jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await client.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating data: {ex.Message}");
            return false;
        }
    }
    
    public static async Task<bool> DeleteData(string path)
    {
        try
        {
            string url = $"{FIREBASE_URL}/{path}.json";
            HttpResponseMessage response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting data: {ex.Message}");
            return false;
        }
    }
}

}