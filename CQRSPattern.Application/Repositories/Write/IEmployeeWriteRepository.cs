using CQRSPattern.Application.Features.Employee;

namespace CQRSPattern.Application.Repositories.Write;

public interface IEmployeeWriteRepository
{
    /// <summary>
    /// Add employee.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(EmployeeModel employee, CancellationToken cancellationToken);

    /// <summary>
    /// update employee.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(EmployeeModel employee, CancellationToken cancellationToken);
}
