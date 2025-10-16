using CQRSPattern.Api.Features.Mcp.Models;
using CQRSPattern.Api.Features.Mcp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Api.Features.Mcp;

/// <summary>
/// MCP (Model Context Protocol) server controller for handling incoming requests
/// </summary>
[ApiController]
[Route("mcp")]
[Produces("application/json")]
public sealed class McpController : ControllerBase
{
    private readonly IMcpRequestRouter _router;
    private readonly ILogger<McpController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpController"/> class
    /// </summary>
    /// <param name="router">The MCP request router</param>
    /// <param name="logger">Logger instance</param>
    public McpController(IMcpRequestRouter router, ILogger<McpController> logger)
    {
        _router = router ?? throw new ArgumentNullException(nameof(router));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes an MCP request
    /// </summary>
    /// <param name="request">The MCP request containing method and parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>MCP response with success status and result or error</returns>
    /// <remarks>
    /// This endpoint accepts MCP-formatted requests and routes them to the appropriate handler.
    /// 
    /// Example request for getting all employees:
    /// 
    ///     POST /mcp/request
    ///     {
    ///       "method": "employee.getAll",
    ///       "id": "req-123"
    ///     }
    /// 
    /// Example request for adding an employee:
    /// 
    ///     POST /mcp/request
    ///     {
    ///       "method": "employee.add",
    ///       "params": {
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "gender": "Male",
    ///         "birthDate": "1990-01-01T00:00:00Z",
    ///         "hireDate": "2020-01-01T00:00:00Z"
    ///       },
    ///       "id": "req-124"
    ///     }
    /// 
    /// Example request for updating an employee:
    /// 
    ///     POST /mcp/request
    ///     {
    ///       "method": "employee.update",
    ///       "params": {
    ///         "id": 1,
    ///         "request": {
    ///           "firstName": "Jane",
    ///           "lastName": "Doe",
    ///           "gender": "Female",
    ///           "birthDate": "1990-01-01T00:00:00Z",
    ///           "hireDate": "2020-01-01T00:00:00Z"
    ///         }
    ///       },
    ///       "id": "req-125"
    ///     }
    /// 
    /// Example request for patching an employee:
    /// 
    ///     POST /mcp/request
    ///     {
    ///       "method": "employee.patch",
    ///       "params": {
    ///         "id": 1,
    ///         "request": {
    ///           "firstName": "Jane"
    ///         }
    ///       },
    ///       "id": "req-126"
    ///     }
    /// 
    /// Available methods:
    /// - employee.getAll: Retrieves all employees
    /// - employee.add: Adds a new employee
    /// - employee.update: Updates an existing employee
    /// - employee.patch: Partially updates an existing employee
    /// </remarks>
    [HttpPost("request")]
    [ProducesResponseType(typeof(McpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<McpResponse>> ProcessRequestAsync(
        [FromBody] McpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                _logger.LogWarning("Received null MCP request");
                return BadRequest(McpResponse.CreateError("INVALID_REQUEST", "Request body is required"));
            }

            _logger.LogInformation("Processing MCP request: Method={Method}, Id={Id}", request.Method, request.Id);

            var response = await _router.RouteRequestAsync(request, cancellationToken);

            // Return 200 OK for all responses, with success/error indicated in the response body
            // This is standard MCP practice - the HTTP status indicates protocol-level success
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in MCP controller");
            return Ok(McpResponse.CreateError("INTERNAL_ERROR", "An unexpected error occurred"));
        }
    }
}
