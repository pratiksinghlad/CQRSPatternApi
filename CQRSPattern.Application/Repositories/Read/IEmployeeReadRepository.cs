using CQRSPattern.Application.Features.Employee;

namespace CQRSPattern.Application.Repositories.Read;

public interface IEmployeeReadRepository
{
    /// <summary>
    /// Get all employees.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<EmployeeModel>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets an employee by their unique identifier.
    /// </summary>
    /// <param name="id">The employee's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The employee model if found; otherwise, null</returns>
    Task<EmployeeModel?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
