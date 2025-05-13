using System.Collections.Concurrent;
using System.Threading.Channels;

namespace CQRSPattern.Api.Services;

public class ServerSentEventsService
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _clients = new();

    public Guid AddClient()
    {
        var clientId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>();
        _clients.TryAdd(clientId, channel);
        return clientId;
    }

    public ChannelReader<string> GetClientChannel(Guid clientId)
    {
        if (_clients.TryGetValue(clientId, out var channel))
        {
            return channel.Reader;
        }

        throw new ArgumentException("Client not found");
    }

    public void RemoveClient(Guid clientId)
    {
        _clients.TryRemove(clientId, out _);
    }

    public async Task SendEventToAllAsync(string eventType, string data)
    {
        var message = $"event: {eventType}\ndata: {data}\n\n";

        foreach (var channel in _clients.Values)
        {
            await channel.Writer.WriteAsync(message);
        }
    }
}
