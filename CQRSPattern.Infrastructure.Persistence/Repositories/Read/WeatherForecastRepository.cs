using CQRSPattern.Application.Features.Models;
using CQRSPattern.Application.Repositories.Read;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CQRSPattern.Infrastructure.Persistence.Repositories.Read;

/// <summary>
/// Repository for handling weather forecast data operations
/// </summary>
public sealed class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly ConcurrentDictionary<int, WeatherForecast> _forecasts = new();
    private int _nextId = 1;
    private readonly ILogger<WeatherForecastRepository> _logger;

    /// <summary>
    /// Event that will be triggered when data changes
    /// </summary>
    public event Action<WeatherForecast> OnDataChange = delegate { };

    public WeatherForecastRepository(ILogger<WeatherForecastRepository> logger)
    {
        _logger = logger;
        InitializeData();
    }

    private void InitializeData()
    {
        foreach (var index in Enumerable.Range(1, 5))
        {
            var forecast = WeatherForecast.CreateRandom(_nextId++, index);
            _forecasts.TryAdd(forecast.Id, forecast);
        }
    }

    public IEnumerable<WeatherForecast> GetAll()
    {
        return _forecasts.Values;
    }

    public WeatherForecast? GetById(int id)
    {
        return _forecasts.TryGetValue(id, out var forecast) ? forecast : null;
    }

    public WeatherForecast Add(WeatherForecast forecast)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        forecast.Id = Interlocked.Increment(ref _nextId);
        if (_forecasts.TryAdd(forecast.Id, forecast))
        {
            _logger.LogInformation("Added new weather forecast with ID: {Id}", forecast.Id);
            OnDataChange(forecast);
            return forecast;
        }

        throw new InvalidOperationException($"Could not add forecast with ID: {forecast.Id}");
    }

    public WeatherForecast? Update(WeatherForecast forecast)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        if (_forecasts.TryGetValue(forecast.Id, out var existing))
        {
            if (_forecasts.TryUpdate(forecast.Id, forecast, existing))
            {
                _logger.LogInformation("Updated weather forecast with ID: {Id}", forecast.Id);
                OnDataChange(forecast);
                return forecast;
            }
        }

        _logger.LogWarning("Failed to update weather forecast with ID: {Id}", forecast.Id);
        return null;
    }

    public bool Delete(int id)
    {
        if (_forecasts.TryRemove(id, out var forecast))
        {
            _logger.LogInformation("Deleted weather forecast with ID: {Id}", id);
            OnDataChange(forecast);
            return true;
        }

        _logger.LogWarning("Failed to delete weather forecast with ID: {Id}", id);
        return false;
    }
}
