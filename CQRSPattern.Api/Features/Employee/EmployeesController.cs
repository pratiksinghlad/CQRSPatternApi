using CQRSPattern.Api.Features.Employee.Add;
using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Api.Features.Employee;

/// <summary>
/// API controller for employee management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IMediatorFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeesController"/> class.
    /// </summary>
    /// <param name="factory">The mediator factory</param>
    public EmployeesController(IMediatorFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Gets all employees.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of employee models</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeModel>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _factory.CreateScope();
            var result = await scope.SendAsync(GetAllQuery.Create(), cancellationToken);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving employees", error = ex.Message });
        }
    }

    /// <summary>
    /// Adds a new employee.
    /// </summary>
    /// <param name="request">The employee creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Action result indicating success or failure</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAsync(
        [FromBody] Request request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var scope = _factory.CreateScope();
            await scope.SendAsync(request.ToMediator(), cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">The employee ID</param>
    /// <param name="request">The employee update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Action result indicating success or failure</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] Update.Request request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid employee ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var scope = _factory.CreateScope();
            await scope.SendAsync(request.ToMediator(id), cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the employee", error = ex.Message });
        }
    }
}
