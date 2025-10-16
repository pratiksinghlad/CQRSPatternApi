using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Services;
using CQRSPattern.Application.Mediator;

namespace CQRSPattern.Api.Features.Mcp.Handlers;

/// <summary>
/// Handler for employee.patch MCP method
/// </summary>
public sealed class EmployeePatchHandler : IMcpMethodHandler
{
    private readonly IMediatorFactory _mediatorFactory;
    private readonly ILogger<EmployeePatchHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeePatchHandler"/> class
    /// </summary>
    /// <param name="mediatorFactory">The mediator factory</param>
    /// <param name="logger">Logger instance</param>
    public EmployeePatchHandler(IMediatorFactory mediatorFactory, ILogger<EmployeePatchHandler> logger)
    {
        _mediatorFactory = mediatorFactory ?? throw new ArgumentNullException(nameof(mediatorFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<object?> HandleAsync(object? methodParams, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing employee.patch");

        if (methodParams == null)
        {
            throw new ArgumentException("Parameters are required for employee.patch");
        }

        // Deserialize params to PatchRequest
        var json = JsonSerializer.Serialize(methodParams);
        var patchRequest = JsonSerializer.Deserialize<PatchRequest>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (patchRequest == null || patchRequest.Id <= 0)
        {
            throw new ArgumentException("Invalid parameters for employee.patch. Id is required.");
        }

        if (!patchRequest.Request.HasAnyUpdates())
        {
            throw new ArgumentException("At least one field must be provided for partial update");
        }

        var scope = _mediatorFactory.CreateScope();
        await scope.SendAsync(patchRequest.Request.ToMediator(patchRequest.Id), cancellationToken);

        _logger.LogInformation("Employee {Id} patched successfully", patchRequest.Id);
        return new { message = "Employee patched successfully" };
    }

    /// <summary>
    /// Request model for employee patch
    /// </summary>
    private sealed record PatchRequest
    {
        public int Id { get; init; }
        public Employee.Patch.Request Request { get; init; } = null!;
    }
}
