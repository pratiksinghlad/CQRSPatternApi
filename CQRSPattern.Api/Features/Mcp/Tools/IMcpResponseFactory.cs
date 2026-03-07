namespace CQRSPattern.Api.Features.Mcp.Tools;

/// <summary>
/// Factory for creating standard JSON responses for MCP tools.
/// Enforces DRY and SOLID principles by abstracting serialization options.
/// </summary>
public interface IMcpResponseFactory
{
    /// <summary>
    /// Serializes an object to a formatted JSON string using standard application options.
    /// </summary>
    string Ok(object? data);

    /// <summary>
    /// Creates a standardized successful response JSON.
    /// </summary>
    string Success(string message);

    /// <summary>
    /// Creates a standardized error response JSON.
    /// </summary>
    string Error(string message);
}
