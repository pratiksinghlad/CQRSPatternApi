using System.Text.Json.Serialization;

namespace CQRSPattern.Api.Features.Mcp.Models;

/// <summary>
/// Represents an incoming MCP (Model Context Protocol) request
/// </summary>
public sealed record McpRequest
{
    /// <summary>
    /// The method/action to execute (e.g., "employee.getAll", "employee.add", "employee.update")
    /// </summary>
    [JsonPropertyName("method")]
    public required string Method { get; init; }

    /// <summary>
    /// Parameters for the request as a JSON object
    /// </summary>
    [JsonPropertyName("params")]
    public object? Params { get; init; }

    /// <summary>
    /// Optional request ID for tracking
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}
