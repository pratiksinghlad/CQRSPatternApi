using System.ComponentModel;
using System.Text.Json;
using CQRSPattern.Application.Features.Models;
using CQRSPattern.Application.Repositories.Read;
using ModelContextProtocol.Server;

namespace CQRSPattern.Api.Features.Mcp.Tools;

/// <summary>
/// MCP tool definitions for Weather Forecast operations.
/// Each tool delegates to the existing IWeatherForecastRepository — no logic duplication.
/// </summary>
[McpServerToolType]
public static class WeatherForecastTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Retrieves all weather forecasts
    /// </summary>
    [McpServerTool(Name = "get_all_weather_forecasts")]
    [Description("Retrieve all weather forecasts. Returns a list of forecast records with ID, date, temperature (C/F), and summary.")]
    public static string GetAllWeatherForecasts(
        [Description("The weather forecast repository")] IWeatherForecastRepository repository)
    {
        var forecasts = repository.GetAll();
        return JsonSerializer.Serialize(forecasts, JsonOptions);
    }

    /// <summary>
    /// Retrieves a weather forecast by its ID
    /// </summary>
    [McpServerTool(Name = "get_weather_forecast_by_id")]
    [Description("Retrieve a specific weather forecast by its unique ID. Returns the forecast details or an error if not found.")]
    public static string GetWeatherForecastById(
        [Description("The unique identifier of the weather forecast")] int id,
        [Description("The weather forecast repository")] IWeatherForecastRepository repository)
    {
        var forecast = repository.GetById(id);

        if (forecast is null)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Weather forecast with ID {id} not found"
            }, JsonOptions);
        }

        return JsonSerializer.Serialize(forecast, JsonOptions);
    }

    /// <summary>
    /// Creates a new weather forecast
    /// </summary>
    [McpServerTool(Name = "create_weather_forecast")]
    [Description("Create a new weather forecast entry. Requires date, temperature in Celsius, and an optional summary.")]
    public static string CreateWeatherForecast(
        [Description("Forecast date in ISO 8601 format (e.g., '2025-06-15')")] DateTime date,
        [Description("Temperature in Celsius (integer)")] int temperatureC,
        [Description("Weather summary (e.g., 'Sunny', 'Rainy', 'Cloudy')")] string summary,
        [Description("The weather forecast repository")] IWeatherForecastRepository repository)
    {
        var forecast = new WeatherForecast
        {
            Id = 0,             // Will be assigned by the repository
            Date = date,
            TemperatureC = temperatureC,
            Summary = summary ?? string.Empty
        };

        var result = repository.Add(forecast);

        return JsonSerializer.Serialize(new
        {
            success = true,
            message = "Weather forecast created successfully",
            forecast = result
        }, JsonOptions);
    }

    /// <summary>
    /// Updates an existing weather forecast
    /// </summary>
    [McpServerTool(Name = "update_weather_forecast")]
    [Description("Update an existing weather forecast. All fields are required — this is a full replacement.")]
    public static string UpdateWeatherForecast(
        [Description("The unique identifier of the weather forecast to update")] int id,
        [Description("Forecast date in ISO 8601 format")] DateTime date,
        [Description("Temperature in Celsius (integer)")] int temperatureC,
        [Description("Weather summary (e.g., 'Sunny', 'Rainy', 'Cloudy')")] string summary,
        [Description("The weather forecast repository")] IWeatherForecastRepository repository)
    {
        var forecast = new WeatherForecast
        {
            Id = id,
            Date = date,
            TemperatureC = temperatureC,
            Summary = summary ?? string.Empty
        };

        var result = repository.Update(forecast);

        if (result is null)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Weather forecast with ID {id} not found"
            }, JsonOptions);
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            message = $"Weather forecast {id} updated successfully",
            forecast = result
        }, JsonOptions);
    }

    /// <summary>
    /// Deletes a weather forecast
    /// </summary>
    [McpServerTool(Name = "delete_weather_forecast")]
    [Description("Delete a weather forecast by its unique ID. Returns success or failure.")]
    public static string DeleteWeatherForecast(
        [Description("The unique identifier of the weather forecast to delete")] int id,
        [Description("The weather forecast repository")] IWeatherForecastRepository repository)
    {
        var deleted = repository.Delete(id);

        return JsonSerializer.Serialize(new
        {
            success = deleted,
            message = deleted
                ? $"Weather forecast {id} deleted successfully"
                : $"Weather forecast with ID {id} not found"
        }, JsonOptions);
    }
}
