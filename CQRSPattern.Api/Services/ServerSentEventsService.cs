using System.Collections.Concurrent;
using System.Threading.Channels;

namespace CQRSPattern.Api.Services;

/// <summary>
/// Service for managing Server-Sent Events (SSE) connections and broadcasting events to clients
/// </summary>
public class ServerSentEventsService
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _clients = new();

    /// <summary>
    /// Adds a new client to the SSE service
    /// </summary>
    /// <returns>A unique identifier for the client</returns>
    public Guid AddClient()
    {
        var clientId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
        _clients.TryAdd(clientId, channel);
        return clientId;
    }

    /// <summary>
    /// Gets the channel reader for a specific client
    /// </summary>
    /// <param name="clientId">The client identifier</param>
    /// <returns>A channel reader for consuming SSE messages</returns>
    /// <exception cref="ArgumentException">Thrown when the client is not found</exception>
    public ChannelReader<string> GetClientChannel(Guid clientId)
    {
        if (_clients.TryGetValue(clientId, out var channel))
        {
            return channel.Reader;
        }

        throw new ArgumentException($"Client with ID {clientId} not found", nameof(clientId));
    }

    /// <summary>
    /// Removes a client from the SSE service
    /// </summary>
    /// <param name="clientId">The client identifier to remove</param>
    public void RemoveClient(Guid clientId)
    {
        if (_clients.TryRemove(clientId, out var channel))
        {
            channel.Writer.Complete();
        }
    }

    /// <summary>
    /// Sends an event to all connected clients
    /// </summary>
    /// <param name="eventType">The type of event</param>
    /// <param name="data">The event data</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendEventToAllAsync(string eventType, string data)
    {
        if (string.IsNullOrEmpty(eventType))
        {
            throw new ArgumentNullException(nameof(eventType));
        }

        var message = $"event: {eventType}\ndata: {data}\n\n";

        var tasks = new List<Task>();
        foreach (var channel in _clients.Values)
        {
            tasks.Add(channel.Writer.WriteAsync(message).AsTask());
        }

        await Task.WhenAll(tasks);
    }
}
