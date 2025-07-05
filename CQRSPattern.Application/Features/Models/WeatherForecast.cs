using System.Xml.Serialization;

namespace CQRSPattern.Application.Features.Models;

[XmlRoot("WeatherForecast")]
public class WeatherForecast
{
    [XmlElement("Id")]
    public required int Id { get; set; }

    [XmlElement("Date")]
    public required DateTime Date { get; set; }

    [XmlElement("TemperatureC")]
    public required int TemperatureC { get; set; }

    [XmlElement("Summary")]
    public string Summary { get; set; } = string.Empty;

    [XmlElement("TemperatureF")]
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
