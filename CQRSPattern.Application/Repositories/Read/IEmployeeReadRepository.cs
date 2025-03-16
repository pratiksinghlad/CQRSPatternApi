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
}