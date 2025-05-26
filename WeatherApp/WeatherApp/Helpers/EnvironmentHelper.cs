using System.Collections.Concurrent;
using System.Text;

namespace WeatherApp.Helpers;

public static class EnvironmentHelper
{
    public static string WeatherApiKey => GetEnvironmentVariable("WEATHER_API_KEY");
    
    private static readonly ConcurrentDictionary<string, string> Cache = new();
    
    private static string GetEnvironmentVariable(string key)
    {
        return Cache.GetOrAdd(key, k => Environment.GetEnvironmentVariable(k) 
            ?? throw new InvalidOperationException($"Required environment variable '{k}' is not set."));
    }
    
    public static void ValidateEnvironmentVariables()
    {
        var missingVariables = new List<string>();
        var requiredVariables = new[]
        {
            "WEATHER_API_KEY"
        };
        
        foreach (var variable in requiredVariables)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variable)))
            {
                missingVariables.Add(variable);
            }
        }
        
        if (missingVariables.Count > 0)
        {
            var message = new StringBuilder("Missing required environment variables:");
            foreach (var variable in missingVariables)
            {
                message.AppendLine($"- {variable}");
            }
            
            throw new InvalidOperationException(message.ToString());
        }
    }
}