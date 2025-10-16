using CQRSPattern.Application.Features.Employee;
using Microsoft.AspNetCore.JsonPatch;

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
    /// Partially updates an employee with only the provided fields using modern .NET 9 approach.
    /// Uses Entity Framework ExecuteUpdateAsync for optimal performance.
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

    /// <summary>
    /// Partially updates an employee using JSON Patch operations.
    /// Supports complex patch operations like add, remove, replace, move, copy, test.
    /// </summary>
    /// <param name="id">The employee's unique identifier</param>
    /// <param name="patchDocument">The JSON patch document containing operations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the employee was found and updated; otherwise, false</returns>
    Task<bool> PatchWithJsonPatchAsync(
        int id,
        JsonPatchDocument<EmployeeModel> patchDocument,
        CancellationToken cancellationToken = default);
}
