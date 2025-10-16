using CQRSPattern.Application.Repositories.Write;
using MediatR;

namespace CQRSPattern.Application.Features.Employee.JsonPatch;

/// <summary>
/// Handler for the JsonPatchEmployeeCommand that applies JSON patch operations to employee entities.
/// </summary>
public class JsonPatchEmployeeCommandHandler : IRequestHandler<JsonPatchEmployeeCommand, bool>
{
    private readonly IEmployeeWriteRepository _employeeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPatchEmployeeCommandHandler"/> class.
    /// </summary>
    /// <param name="employeeRepository">The employee write repository</param>
    /// <exception cref="ArgumentNullException">Thrown when employeeRepository is null</exception>
    public JsonPatchEmployeeCommandHandler(IEmployeeWriteRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    /// <summary>
    /// Handles the JSON patch operations on an employee entity.
    /// </summary>
    /// <param name="request">The JSON patch employee command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the employee was found and updated; otherwise, false</returns>
    public async Task<bool> Handle(JsonPatchEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Apply the JSON patch operations using the repository
        var wasUpdated = await _employeeRepository.PatchWithJsonPatchAsync(
            request.Id,
            request.PatchDocument,
            cancellationToken);

        return wasUpdated;
    }
}
