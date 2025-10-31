using System.Text.Json.Serialization;

namespace CQRSPattern.Api.Features.Mcp.Models;

/// <summary>
/// Represents an MCP (Model Context Protocol) response
/// </summary>
public sealed record McpResponse
{
    /// <summary>
    /// Indicates whether the request was successful
    /// </summary>
    [JsonPropertyName("success")]
    public required bool Success { get; init; }

    /// <summary>
    /// The result data if the request was successful
    /// </summary>
    [JsonPropertyName("result")]
    public object? Result { get; init; }

    /// <summary>
    /// Error information if the request failed
    /// </summary>
    [JsonPropertyName("error")]
    public McpError? Error { get; init; }

    /// <summary>
    /// Optional request ID for tracking (echoed from request)
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Creates a successful MCP response
    /// </summary>
    /// <param name="result">The result data</param>
    /// <param name="id">Optional request ID</param>
    /// <returns>A successful MCP response</returns>
    public static McpResponse CreateSuccess(object? result = null, string? id = null) =>
        new()
        {
            Success = true,
            Result = result,
            Id = id
        };

    /// <summary>
    /// Creates an error MCP response
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    /// <param name="id">Optional request ID</param>
    /// <returns>An error MCP response</returns>
    public static McpResponse CreateError(string code, string message, string? id = null) =>
        new()
        {
            Success = false,
            Error = new McpError
            {
                Code = code,
                Message = message
            },
            Id = id
        };
}

/// <summary>
/// Represents an error in an MCP response
/// </summary>
public sealed record McpError
{
    /// <summary>
    /// Error code (e.g., "INVALID_METHOD", "VALIDATION_ERROR", "INTERNAL_ERROR")
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    /// <summary>
    /// Additional error details
    /// </summary>
    [JsonPropertyName("details")]
    public object? Details { get; init; }
}
