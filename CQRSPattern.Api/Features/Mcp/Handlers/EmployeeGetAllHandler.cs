using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Services;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Mediator;

namespace CQRSPattern.Api.Features.Mcp.Handlers;

/// <summary>
/// Handler for employee.getAll MCP method
/// </summary>
public sealed class EmployeeGetAllHandler : IMcpMethodHandler
{
    private readonly IMediatorFactory _mediatorFactory;
    private readonly ILogger<EmployeeGetAllHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeGetAllHandler"/> class
    /// </summary>
    /// <param name="mediatorFactory">The mediator factory</param>
    /// <param name="logger">Logger instance</param>
    public EmployeeGetAllHandler(IMediatorFactory mediatorFactory, ILogger<EmployeeGetAllHandler> logger)
    {
        _mediatorFactory = mediatorFactory ?? throw new ArgumentNullException(nameof(mediatorFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<object?> HandleAsync(object? methodParams, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing employee.getAll");

        var scope = _mediatorFactory.CreateScope();
        var result = await scope.SendAsync(GetAllQuery.Create(), cancellationToken);

        _logger.LogInformation("Retrieved {Count} employees", result.Data.Count());
        return result.Data;
    }
}
