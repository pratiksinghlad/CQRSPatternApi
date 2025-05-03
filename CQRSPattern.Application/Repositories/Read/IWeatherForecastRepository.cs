using CQRSPattern.Application.Features.Models;

namespace CQRSPattern.Application.Repositories.Read;

public interface IWeatherForecastRepository
{
    IEnumerable<WeatherForecast> GetAll();
    WeatherForecast? GetById(int id);
    WeatherForecast Add(WeatherForecast forecast);
    WeatherForecast? Update(WeatherForecast forecast);
    bool Delete(int id);
        
    // Event for data changes
    event Action<WeatherForecast> OnDataChange;
}