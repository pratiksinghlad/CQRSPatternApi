using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Models;

namespace CQRSPattern.Api.Features.Mcp.Services;

/// <summary>
/// Service for routing MCP requests to appropriate handlers
/// </summary>
public interface IMcpRequestRouter
{
    /// <summary>
    /// Routes an MCP request to the appropriate handler
    /// </summary>
    /// <param name="request">The MCP request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The MCP response</returns>
    Task<McpResponse> RouteRequestAsync(McpRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Implementation of MCP request router
/// </summary>
public sealed class McpRequestRouter : IMcpRequestRouter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<McpRequestRouter> _logger;
    private readonly Dictionary<string, Type> _methodHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpRequestRouter"/> class
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving handlers</param>
    /// <param name="logger">Logger instance</param>
    public McpRequestRouter(IServiceProvider serviceProvider, ILogger<McpRequestRouter> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Register method handlers
        _methodHandlers = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["employee.getAll"] = typeof(Handlers.EmployeeGetAllHandler),
            ["employee.add"] = typeof(Handlers.EmployeeAddHandler),
            ["employee.update"] = typeof(Handlers.EmployeeUpdateHandler),
            ["employee.patch"] = typeof(Handlers.EmployeePatchHandler),
        };
    }

    /// <inheritdoc/>
    public async Task<McpResponse> RouteRequestAsync(McpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Routing MCP request: Method={Method}, Id={Id}", request.Method, request.Id);

            if (string.IsNullOrWhiteSpace(request.Method))
            {
                return McpResponse.CreateError("INVALID_METHOD", "Method name is required", request.Id);
            }

            if (!_methodHandlers.TryGetValue(request.Method, out var handlerType))
            {
                _logger.LogWarning("Unknown MCP method: {Method}", request.Method);
                return McpResponse.CreateError(
                    "METHOD_NOT_FOUND",
                    $"Method '{request.Method}' is not supported. Available methods: {string.Join(", ", _methodHandlers.Keys)}",
                    request.Id
                );
            }

            // Resolve handler from DI container
            var handler = _serviceProvider.GetService(handlerType) as IMcpMethodHandler;
            if (handler == null)
            {
                _logger.LogError("Handler not registered for method: {Method}", request.Method);
                return McpResponse.CreateError("INTERNAL_ERROR", "Handler not available for this method", request.Id);
            }

            // Execute handler
            var result = await handler.HandleAsync(request.Params, cancellationToken);

            _logger.LogInformation("MCP request completed successfully: Method={Method}, Id={Id}", request.Method, request.Id);
            return McpResponse.CreateSuccess(result, request.Id);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error in MCP request");
            return McpResponse.CreateError("INVALID_PARAMS", $"Invalid parameter format: {ex.Message}", request.Id);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in MCP request");
            return McpResponse.CreateError("VALIDATION_ERROR", ex.Message, request.Id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found in MCP request");
            return McpResponse.CreateError("NOT_FOUND", ex.Message, request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing MCP request");
            return McpResponse.CreateError("INTERNAL_ERROR", "An unexpected error occurred", request.Id);
        }
    }
}
