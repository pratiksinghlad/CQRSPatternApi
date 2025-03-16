using CQRSPattern.Api.Features.Employee.Add;
using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CQRSPattern.Api.Features.Employee;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="factory"></param>
    public EmployeesController(IMediatorFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Get all employees.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeModel>))]
    public async Task<IEnumerable<EmployeeModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var scope = _factory.CreateScope();
        var result = await scope.SendAsync(GetAllQuery.Create(), cancellationToken);
        
        return result.Data;
    }

    /// <summary>
    /// add employee.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] Request request, CancellationToken cancellationToken)
    {
        var scope = _factory.CreateScope();
        await scope.SendAsync(request.ToMediator(), cancellationToken);

        return Ok();
    }

    /// <summary>
    /// update employee.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Update.Request request, CancellationToken cancellationToken)
    {
        var scope = _factory.CreateScope();
        await scope.SendAsync(request.ToMediator(id), cancellationToken);

        return Ok();
    }

    private readonly IMediatorFactory _factory;
}