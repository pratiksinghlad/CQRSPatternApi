namespace CQRSPattern.Application.Features.Models;

public sealed class WeatherForecast
{
    public required int Id { get; set; }
    public required DateTime Date { get; set; }
    public required int TemperatureC { get; set; }
    public string Summary { get; set; } = string.Empty;

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    private const int _minTemp = -20;
    private const int _maxTemp = 55;

    public static WeatherForecast CreateRandom(int id, int daysOffset) => new()
    {
        Id = id,
        Date = DateTime.Now.AddDays(daysOffset),
        TemperatureC = Random.Shared.Next(_minTemp, _maxTemp),
        Summary = GetRandomSummary()
    };

    private static string GetRandomSummary() =>
        _weatherSummaries[Random.Shared.Next(_weatherSummaries.Length)];

    private static readonly string[] _weatherSummaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
}
