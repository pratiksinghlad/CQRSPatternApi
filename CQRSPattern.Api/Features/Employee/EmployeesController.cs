using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Mediator;
using Microsoft.AspNetCore.JsonPatch;
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
            var scope = _factory.CreateScope();
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
        [FromBody] Add.Request request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var scope = _factory.CreateScope();
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
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Update.Request request, CancellationToken cancellationToken)
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

            var scope = _factory.CreateScope();
            await scope.SendAsync(request.ToMediator(id), cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Partially updates an existing employee using HTTP PATCH.
    /// </summary>
    /// <param name="id">The employee ID</param>
    /// <param name="request">The employee partial update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Action result indicating success or failure</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchAsync(int id, [FromBody] Patch.Request request, CancellationToken cancellationToken
    )
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid employee ID");
            }

            if (!request.HasAnyUpdates())
            {
                return BadRequest("At least one field must be provided for partial update");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var scope = _factory.CreateScope();
            await scope.SendAsync(request.ToMediator(id), cancellationToken);

            return Accepted();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while partially updating the employee", error = ex.Message });
        }
    }

    /// <summary>
    /// Partially updates an existing employee using JSON Patch operations.
    /// Supports complex patch operations like add, remove, replace, move, copy, test.
    /// </summary>
    /// <param name="id">The employee ID</param>
    /// <param name="request">The JSON patch request containing operations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Action result indicating success or failure</returns>
    /// <remarks>
    /// Example request body:
    /// 
    ///     [
    ///       { "op": "replace", "path": "/FirstName", "value": "Jane" },
    ///       { "op": "replace", "path": "/BirthDate", "value": "1985-06-15T00:00:00Z" },
    ///       { "op": "test", "path": "/Gender", "value": "Female" },
    ///       { "op": "remove", "path": "/LastName" }
    ///     ]
    /// 
    /// Supported operations:
    /// - **add**: Adds a value to the specified path
    /// - **remove**: Removes the value at the specified path
    /// - **replace**: Replaces the value at the specified path
    /// - **move**: Moves a value from one path to another
    /// - **copy**: Copies a value from one path to another
    /// - **test**: Tests that the value at the specified path equals the given value
    /// 
    /// Valid paths: /FirstName, /LastName, /Gender, /BirthDate, /HireDate
    /// </remarks>
    [HttpPatch("{id}/jsonpatch")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> JsonPatchAsync(
        int id,
        [FromBody] JsonPatchDocument<EmployeeModel> patchDocument,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Invalid employee ID");
            }

            if (patchDocument == null)
            {
                return BadRequest("Patch document cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var scope = _factory?.CreateScope();
            if (scope == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Unable to create mediator scope" });
            }

            var command = CQRSPattern.Application.Features.Employee.JsonPatch.JsonPatchEmployeeCommand.CreateCommand(id, patchDocument);
            var wasUpdated = await scope.SendAsync(command, cancellationToken);
            
            if (!wasUpdated)
            {
                return NotFound(new { message = $"Employee with ID {id} not found" });
            }

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = $"Null argument: {ex.ParamName}", error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while applying JSON patch to the employee", error = ex?.Message ?? "Unknown error" });
        }
    }
}
