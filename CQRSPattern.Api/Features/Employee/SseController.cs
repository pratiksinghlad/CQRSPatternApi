using CQRSPattern.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Api.Features.Employee;

[ApiController]
[Route("api/[controller]")]
public class SseController : ControllerBase
{
    private readonly ServerSentEventsService _sseService;
    private readonly ILogger<SseController> _logger;

    public SseController(ServerSentEventsService sseService, ILogger<SseController> logger)
    {
        _sseService = sseService;
        _logger = logger;
    }

    [HttpGet]
    public async Task Get()
    {
        var clientId = _sseService.AddClient();
        _logger.LogInformation($"Client connected: {clientId}");

        try
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            var channel = _sseService.GetClientChannel(clientId);

            // Send initial connection established event
            await Response.WriteAsync($"event: connected\ndata: {clientId}\n\n");
            await Response.Body.FlushAsync();

            // Keep the connection open and stream events as they come
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // Wait for the next message to be available
                try
                {
                    if (await channel.WaitToReadAsync(HttpContext.RequestAborted))
                    {
                        while (channel.TryRead(out var message))
                        {
                            await Response.WriteAsync(message);
                            await Response.Body.FlushAsync();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        finally
        {
            _sseService.RemoveClient(clientId);
            _logger.LogInformation($"Client disconnected: {clientId}");
        }
    }
}