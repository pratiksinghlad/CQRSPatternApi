using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Services;
using CQRSPattern.Application.Mediator;

namespace CQRSPattern.Api.Features.Mcp.Handlers;

/// <summary>
/// Handler for employee.add MCP method
/// </summary>
public sealed class EmployeeAddHandler : IMcpMethodHandler
{
    private readonly IMediatorFactory _mediatorFactory;
    private readonly ILogger<EmployeeAddHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeAddHandler"/> class
    /// </summary>
    /// <param name="mediatorFactory">The mediator factory</param>
    /// <param name="logger">Logger instance</param>
    public EmployeeAddHandler(IMediatorFactory mediatorFactory, ILogger<EmployeeAddHandler> logger)
    {
        _mediatorFactory = mediatorFactory ?? throw new ArgumentNullException(nameof(mediatorFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<object?> HandleAsync(object? methodParams, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing employee.add");

        if (methodParams == null)
        {
            throw new ArgumentException("Parameters are required for employee.add");
        }

        // Deserialize params to Add.Request
        var json = JsonSerializer.Serialize(methodParams);
        var request = JsonSerializer.Deserialize<Employee.Add.Request>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (request == null)
        {
            throw new ArgumentException("Invalid parameters for employee.add");
        }

        var scope = _mediatorFactory.CreateScope();
        await scope.SendAsync(request.ToMediator(), cancellationToken);

        _logger.LogInformation("Employee added successfully");
        return new { message = "Employee created successfully" };
    }
}
