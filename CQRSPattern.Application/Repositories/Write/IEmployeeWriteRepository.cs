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

    /// <summary>
    /// Partially updates an employee with only the provided fields.
    /// </summary>
    /// <param name="id">The employee's unique identifier</param>
    /// <param name="firstName">The employee's first name (optional)</param>
    /// <param name="lastName">The employee's last name (optional)</param>
    /// <param name="gender">The employee's gender (optional)</param>
    /// <param name="birthDate">The employee's birth date (optional)</param>
    /// <param name="hireDate">The employee's hire date (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the employee was found and updated; otherwise, false</returns>
    Task<bool> PatchAsync(
        int id,
        string? firstName = null,
        string? lastName = null,
        string? gender = null,
        DateTime? birthDate = null,
        DateTime? hireDate = null,
        CancellationToken cancellationToken = default);
}
