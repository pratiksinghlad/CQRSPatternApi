using CQRSPattern.Application.Repositories.Write;
using MediatR;

namespace CQRSPattern.Application.Features.Employee.Patch;

/// <summary>
/// Handler for the PatchEmployeeCommand that performs partial updates on employee entities.
/// </summary>
public class PatchEmployeeCommandHandler : IRequestHandler<PatchEmployeeCommand, Unit>
{
    private readonly IEmployeeWriteRepository _employeeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchEmployeeCommandHandler"/> class.
    /// </summary>
    /// <param name="employeeRepository">The employee write repository</param>
    /// <exception cref="ArgumentNullException">Thrown when employeeRepository is null</exception>
    public PatchEmployeeCommandHandler(IEmployeeWriteRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    /// <summary>
    /// Handles the partial update of an employee entity.
    /// </summary>
    /// <param name="request">The patch employee command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unit value indicating completion</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the employee with the specified ID is not found</exception>
    public async Task<Unit> Handle(PatchEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Perform the partial update using the repository
        var wasUpdated = await _employeeRepository.PatchAsync(
            request.Id,
            request.FirstName,
            request.LastName,
            request.Gender,
            request.BirthDate,
            request.HireDate,
            cancellationToken);

        // Throw exception if employee was not found
        if (!wasUpdated)
        {
            throw new KeyNotFoundException($"Employee with ID {request.Id} not found.");
        }

        return Unit.Value;
    }
}
