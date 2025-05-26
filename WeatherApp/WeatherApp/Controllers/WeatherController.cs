using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Helpers;
using WeatherApp.Models;

namespace WeatherApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherController> _logger;
    private readonly string _apiKey;

    // Historical data
    private readonly Dictionary<string, double[]> _historicalData = new()
    {
        { "Amsterdam", new[] { 4.2, 3.0, 4.1, 5.0, 7.9, 9.1, 12.0, 15.3, 15.1, 10.1, 8.7, 5.9 } },
        { "Dublin", new[] { 1.2, 2.9, 2.0, 5.9, 8.9, 8.1, 12.9, 15.1, 15.9, 7.1, 4.3, -1.9 } },
        { "London", new[] { 1.9, 4.9, 5.1, 7.1, 8.9, 10.1, 11.9, 13.5, 17.1, 12.1, 6.7, -0.9 } },
        { "Barcelona", new[] { 8.9, 5.6, 6.9, 7.5, 8.9, 17.4, 21.7, 23.9, 18.9, 14.8, 11.6, 9.9 } },
        { "Paris", new[] { 10.40, 6.99, 5.41, 7.12, 8.99, 8.13, 16.71, 16.94, 17.92, 13.72, 11.43, 9.21 } },
        { "Reykjavik", new[] { -9.2, -8.9, 2.5, 4.1, 6.9, 6.8, 8.3, 10.1, 7.4, 2.0, -1.7, -8.2 } },
        { "Vienna", new[] { -9.2, -7.5, -2.5, 7.8, 14.8, 20.3, 20.3, 21.2, 14.3, 7.7, 3.4, -6.7 } },
        { "Rome", new[] { 0.2, 9.5, 12.5, 21.1, 22.5, 25.3, 28.2, 23.3, 18.1, 12.4, 9.8, 5.7 } }
    };

    public WeatherController(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherController> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = EnvironmentHelper.WeatherApiKey;
    }

    [HttpGet("cities/analysis")]
    public ActionResult<IEnumerable<CityAnalysisDto>> GetCitiesAnalysis()
    {
        try
        {
            var analysis = _historicalData.Select(kvp => new CityAnalysisDto
            {
                City = kvp.Key,
                MonthlyTemperatures = kvp.Value,
                AverageTemperature = Math.Round(kvp.Value.Average(), 2),
                HighestTemperature = kvp.Value.Max(),
                LowestTemperature = kvp.Value.Min(),
                HighestTemperatureMonth = Array.IndexOf(kvp.Value, kvp.Value.Max()) + 1,
                LowestTemperatureMonth = Array.IndexOf(kvp.Value, kvp.Value.Min()) + 1
            }).ToList();

            return Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while analyzing city temperature data");
            return StatusCode(500, new ApiErrorResponse 
            { 
                Success = false, 
                Message = "Internal server error occurred while analyzing data" 
            });
        }
    }

    [HttpGet("amsterdam/comparison")]
    public async Task<ActionResult<WeatherComparisonDto>> GetAmsterdamComparison()
    {
        try
        {
            // Get current weather for Amsterdam
            var currentWeather = await GetCurrentWeatherAsync("Amsterdam");
            if (currentWeather == null)
            {
                return StatusCode(500, new ApiErrorResponse 
                { 
                    Success = false, 
                    Message = "Failed to fetch current weather data for Amsterdam" 
                });
            }

            var historicalData = _historicalData["Amsterdam"];
            var currentMonth = DateTime.Now.Month - 1; // 0-based index
            var historicalAverage = Math.Round(historicalData.Average(), 2);
            
            // Calculate differences between current and historical for each month
            var monthlyDifferences = new double[12];
            for (int i = 0; i < 12; i++)
            {
                monthlyDifferences[i] = Math.Round(currentWeather.Main.Temperature - historicalData[i], 2);
            }

            var comparison = new WeatherComparisonDto
            {
                City = "Amsterdam",
                CurrentTemperature = Math.Round(currentWeather.Main.Temperature, 2),
                HistoricalMonthlyTemperatures = historicalData,
                HistoricalAverageTemperature = historicalAverage,
                MonthlyDifferences = monthlyDifferences,
                AverageDifference = Math.Round(currentWeather.Main.Temperature - historicalAverage, 2),
                LastUpdated = DateTime.UtcNow,
                WeatherDescription = currentWeather.Weather.FirstOrDefault()?.Description ?? "N/A"
            };

            return Ok(comparison);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred while fetching weather data");
            return StatusCode(503, new ApiErrorResponse 
            { 
                Success = false, 
                Message = "Weather service is currently unavailable",
                Details = "Please try again later"
            });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing weather API response");
            return StatusCode(502, new ApiErrorResponse 
            { 
                Success = false, 
                Message = "Invalid response from weather service" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while comparing Amsterdam weather");
            return StatusCode(500, new ApiErrorResponse 
            { 
                Success = false, 
                Message = "Internal server error occurred" 
            });
        }
    }

    [HttpGet("current/{city}")]
    public async Task<ActionResult<CurrentWeatherResponse>> GetCurrentWeather(string city)
    {
        try
        {
            var currentWeather = await GetCurrentWeatherAsync(city);
            if (currentWeather == null)
            {
                return NotFound(new ApiErrorResponse 
                { 
                    Success = false, 
                    Message = $"Weather data not found for {city}" 
                });
            }

            return Ok(currentWeather);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred while fetching weather data for {City}", city);
            return StatusCode(503, new ApiErrorResponse 
            { 
                Success = false, 
                Message = "Weather service is currently unavailable" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching weather data for {City}", city);
            return StatusCode(500, new ApiErrorResponse 
            { 
                Success = false, 
                Message = "Internal server error occurred" 
            });
        }
    }

    private async Task<CurrentWeatherResponse?> GetCurrentWeatherAsync(string city)
    {
        try
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<WeatherApiErrorResponse>(content);
                _logger.LogWarning("OpenWeatherMap API error: {Code} - {Message}", errorResponse?.Code, errorResponse?.Message);
                return null;
            }

            var weatherData = JsonSerializer.Deserialize<CurrentWeatherResponse>(content);
            return weatherData;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize weather API response for {City}", city);
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for weather data: {City}", city);
            throw;
        }
    }
}