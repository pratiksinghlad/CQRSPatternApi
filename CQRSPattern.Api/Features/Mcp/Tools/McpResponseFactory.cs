using System.Text.Json;

namespace CQRSPattern.Api.Features.Mcp.Tools;

/// <summary>
/// Default implementation of the MCP response factory.
/// Enforces consistent, camel-cased, and indented JSON for MCP tool outputs.
/// </summary>
public class McpResponseFactory : IMcpResponseFactory
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpResponseFactory"/> class.
    /// </summary>
    public McpResponseFactory()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <inheritdoc/>
    public string Ok(object? data)
    {
        return JsonSerializer.Serialize(data, _options);
    }

    /// <inheritdoc/>
    public string Success(string message)
    {
        return JsonSerializer.Serialize(new { success = true, message }, _options);
    }

    /// <inheritdoc/>
    public string Error(string message)
    {
        return JsonSerializer.Serialize(new { success = false, message }, _options);
    }
}
