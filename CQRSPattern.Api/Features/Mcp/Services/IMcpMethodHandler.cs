namespace CQRSPattern.Api.Features.Mcp.Services;

/// <summary>
/// Interface for handling MCP method requests
/// </summary>
public interface IMcpMethodHandler
{
    /// <summary>
    /// Handles an MCP method request
    /// </summary>
    /// <param name="methodParams">The method parameters as a JSON object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the method execution</returns>
    Task<object?> HandleAsync(object? methodParams, CancellationToken cancellationToken);
}
