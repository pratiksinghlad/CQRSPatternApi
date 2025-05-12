using CQRSPattern.Application.Features.Models;
using CQRSPattern.Application.Repositories.Read;

namespace CQRSPattern.Infrastructure.Persistence.Repositories.Read;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly List<WeatherForecast> _forecasts;
    private int _nextId = 1;

    // Event that will be triggered when data changes
    public event Action<WeatherForecast> OnDataChange = delegate { };

    private static readonly string[] Summaries = new[]
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching",
    };

    public WeatherForecastRepository()
    {
        _forecasts = Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecast
            {
                Id = _nextId++,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            })
            .ToList();
    }

    public IEnumerable<WeatherForecast> GetAll()
    {
        return _forecasts;
    }

    public WeatherForecast? GetById(int id)
    {
        return _forecasts.FirstOrDefault(f => f.Id == id);
    }

    public WeatherForecast Add(WeatherForecast forecast)
    {
        forecast.Id = _nextId++;
        _forecasts.Add(forecast);

        // Trigger the event
        OnDataChange(forecast);

        return forecast;
    }

    public WeatherForecast? Update(WeatherForecast forecast)
    {
        var existingForecast = _forecasts.FirstOrDefault(f => f.Id == forecast.Id);
        if (existingForecast == null)
            return null;

        existingForecast.Date = forecast.Date;
        existingForecast.TemperatureC = forecast.TemperatureC;
        existingForecast.Summary = forecast.Summary;

        // Trigger the event
        OnDataChange(existingForecast);

        return existingForecast;
    }

    public bool Delete(int id)
    {
        var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
        if (forecast == null)
            return false;

        _forecasts.Remove(forecast);

        // Trigger the event
        OnDataChange(forecast);

        return true;
    }
}
