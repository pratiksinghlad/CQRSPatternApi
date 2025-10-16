using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Services;
using CQRSPattern.Application.Mediator;

namespace CQRSPattern.Api.Features.Mcp.Handlers;

/// <summary>
/// Handler for employee.update MCP method
/// </summary>
public sealed class EmployeeUpdateHandler : IMcpMethodHandler
{
    private readonly IMediatorFactory _mediatorFactory;
    private readonly ILogger<EmployeeUpdateHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeUpdateHandler"/> class
    /// </summary>
    /// <param name="mediatorFactory">The mediator factory</param>
    /// <param name="logger">Logger instance</param>
    public EmployeeUpdateHandler(IMediatorFactory mediatorFactory, ILogger<EmployeeUpdateHandler> logger)
    {
        _mediatorFactory = mediatorFactory ?? throw new ArgumentNullException(nameof(mediatorFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<object?> HandleAsync(object? methodParams, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing employee.update");

        if (methodParams == null)
        {
            throw new ArgumentException("Parameters are required for employee.update");
        }

        // Deserialize params to UpdateRequest
        var json = JsonSerializer.Serialize(methodParams);
        var updateRequest = JsonSerializer.Deserialize<UpdateRequest>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (updateRequest == null || updateRequest.Id <= 0)
        {
            throw new ArgumentException("Invalid parameters for employee.update. Id is required.");
        }

        var scope = _mediatorFactory.CreateScope();
        await scope.SendAsync(updateRequest.Request.ToMediator(updateRequest.Id), cancellationToken);

        _logger.LogInformation("Employee {Id} updated successfully", updateRequest.Id);
        return new { message = "Employee updated successfully" };
    }

    /// <summary>
    /// Request model for employee update
    /// </summary>
    private sealed record UpdateRequest
    {
        public int Id { get; init; }
        public Employee.Update.Request Request { get; init; } = null!;
    }
}
