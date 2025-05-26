using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class CityTemperatureData
{
    public string City { get; set; } = string.Empty;
    public double[] MonthlyTemperatures { get; set; } = new double[12];
    public double AverageTemperature { get; set; }
    public double HighestTemperature { get; set; }
    public double LowestTemperature { get; set; }
}

// OpenWeatherMap Current Weather API Response Models
public class CurrentWeatherResponse
{
    [JsonPropertyName("coord")]
    public Coordinates Coord { get; set; } = new();

    [JsonPropertyName("weather")]
    public Weather[] Weather { get; set; } = Array.Empty<Weather>();

    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("main")]
    public MainWeatherData Main { get; set; } = new();

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; } = new();

    [JsonPropertyName("clouds")]
    public CloudData Clouds { get; set; } = new();

    [JsonPropertyName("dt")]
    public long DateTime { get; set; }

    [JsonPropertyName("sys")]
    public SystemData Sys { get; set; } = new();

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("cod")]
    public int Cod { get; set; }
}

public class Coordinates
{
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}

public class Weather
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class MainWeatherData
{
    [JsonPropertyName("temp")]
    public double Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("sea_level")]
    public int? SeaLevel { get; set; }

    [JsonPropertyName("grnd_level")]
    public int? GroundLevel { get; set; }
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Degree { get; set; }

    [JsonPropertyName("gust")]
    public double? Gust { get; set; }
}

public class CloudData
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}

public class SystemData
{
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}

// Error response model
public class WeatherApiErrorResponse
{
    [JsonPropertyName("cod")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

// DTOs for your API responses
public class WeatherComparisonDto
{
    public string City { get; set; } = string.Empty;
    public double CurrentTemperature { get; set; }
    public double[] HistoricalMonthlyTemperatures { get; set; } = new double[12];
    public double HistoricalAverageTemperature { get; set; }
    public double[] MonthlyDifferences { get; set; } = new double[12];
    public double AverageDifference { get; set; }
    public DateTime LastUpdated { get; set; }
    public string WeatherDescription { get; set; } = string.Empty;
}

public class CityAnalysisDto
{
    public string City { get; set; } = string.Empty;
    public double[] MonthlyTemperatures { get; set; } = new double[12];
    public double AverageTemperature { get; set; }
    public double HighestTemperature { get; set; }
    public double LowestTemperature { get; set; }
    public int HighestTemperatureMonth { get; set; }
    public int LowestTemperatureMonth { get; set; }
}

public class ApiErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
