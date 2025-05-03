using System.Text.Json;
using CQRSPattern.Api.Services;
using CQRSPattern.Application.Features.Models;
using CQRSPattern.Application.Repositories.Read;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Api.Features.Weather;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastRepository _repository;
    private readonly ServerSentEventsService _sseService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        IWeatherForecastRepository repository,
        ServerSentEventsService sseService,
        ILogger<WeatherForecastController> logger)
    {
        _repository = repository;
        _sseService = sseService;
        _logger = logger;

        // Subscribe to data change events
        _repository.OnDataChange += async (forecast) =>
        {
            await _sseService.SendEventToAllAsync(
                "weather-update",
                JsonSerializer.Serialize(forecast)
            );
        };
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var forecast = _repository.GetById(id);
        if (forecast == null)
            return NotFound();

        return Ok(forecast);
    }

    [HttpPost]
    public IActionResult Create([FromBody] WeatherForecast forecast)
    {
        var result = _repository.Add(forecast);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] WeatherForecast forecast)
    {
        forecast.Id = id;
        var result = _repository.Update(forecast);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = _repository.Delete(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}