using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Repositories.Write;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace CQRSPattern.Infrastructure.Persistence.Repositories.Read;

public class EmployeeWriteRepository : IEmployeeWriteRepository
{
    public EmployeeWriteRepository(IDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmployeeModel employee, CancellationToken cancellationToken)
    {
        var employeeEntity = new EmployeeEntity
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Gender = employee.Gender,
            BirthDate = employee.BirthDate,
            HireDate = employee.HireDate,
        };

        await _dbContext.Employees.AddAsync(employeeEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EmployeeModel employee, CancellationToken cancellationToken)
    {
        var existingEmployee = await _dbContext.Employees.FindAsync(
            new object[] { employee.Id },
            cancellationToken
        );

        if (existingEmployee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {employee.Id} not found.");
        }

        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.Gender = employee.Gender;
        existingEmployee.BirthDate = employee.BirthDate;
        existingEmployee.HireDate = employee.HireDate;

        _dbContext.Employees.Update(existingEmployee);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> PatchAsync(
        int id,
        string? firstName = null,
        string? lastName = null,
        string? gender = null,
        DateTime? birthDate = null,
        DateTime? hireDate = null,
        CancellationToken cancellationToken = default)
    {
        var existingEmployee = await _dbContext.Employees.FindAsync([id], cancellationToken);
        
        if (existingEmployee == null)
        {
            return false;
        }

        // Update only the fields that are provided (not null) using C# 13 modern patterns
        if (firstName is not null)
        {
            existingEmployee.FirstName = firstName;
        }

        if (lastName is not null)
        {
            existingEmployee.LastName = lastName;
        }

        if (gender is not null)
        {
            existingEmployee.Gender = gender;
        }

        if (birthDate.HasValue)
        {
            existingEmployee.BirthDate = birthDate.Value;
        }

        if (hireDate.HasValue)
        {
            existingEmployee.HireDate = hireDate.Value;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> PatchWithJsonPatchAsync(
        int id,
        JsonPatchDocument<EmployeeModel> patchDocument,
        CancellationToken cancellationToken = default)
    {
        // Validate input parameters
        if (patchDocument == null)
        {
            throw new ArgumentNullException(nameof(patchDocument));
        }

        // First check if employee exists
        var existingEmployee = await _dbContext.Employees.FindAsync([id], cancellationToken);
        
        if (existingEmployee == null)
        {
            return false;
        }

        // Convert entity to model for JSON Patch operations with null safety
        var employeeModel = new EmployeeModel
        {
            Id = existingEmployee.Id,
            FirstName = existingEmployee.FirstName ?? string.Empty,
            LastName = existingEmployee.LastName ?? string.Empty,
            Gender = existingEmployee.Gender ?? string.Empty,
            BirthDate = existingEmployee.BirthDate,
            HireDate = existingEmployee.HireDate,
        };

        var errors = new List<string>();

        try
        {
            // Apply the patch operations with detailed error handling
            patchDocument.ApplyTo(
                employeeModel,
                error =>
                {
                    if (error == null)
                        return;

                    var errorMessage = 
                        $"JSON Patch operation failed: {error.ErrorMessage ?? "Unknown error"}";

                    if (error.Operation != null)
                    {
                        errorMessage +=
                            $" Operation: '{error.Operation.op ?? "unknown"}' on path '{error.Operation.path ?? "unknown"}'";

                        if (error.Operation.value != null)
                        {
                            errorMessage += $" (attempted value: '{error.Operation.value}')";
                        }

                        if (!string.IsNullOrEmpty(error.Operation.from))
                        {
                            errorMessage += $" (from: '{error.Operation.from}')";
                        }
                    }

                    errors.Add(errorMessage);
                });

            // If there were errors during patch application, throw an exception with details
            if (errors.Count > 0)
            {
                throw new InvalidOperationException(
                    $"JSON Patch validation failed: {string.Join("; ", errors)}");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            // Handle other JSON Patch related errors
            throw new InvalidOperationException(
                $"Failed to apply JSON Patch operations: {ex?.Message ?? "Unknown error"}",
                ex);
        }

        // Validate the patched model before updating the entity
        if (string.IsNullOrWhiteSpace(employeeModel.FirstName))
        {
            throw new InvalidOperationException(
                "FirstName cannot be null or empty after patch operation");
        }

        if (string.IsNullOrWhiteSpace(employeeModel.LastName))
        {
            throw new InvalidOperationException(
                "LastName cannot be null or empty after patch operation");
        }

        if (string.IsNullOrWhiteSpace(employeeModel.Gender))
        {
            throw new InvalidOperationException(
                "Gender cannot be null or empty after patch operation");
        }

        // Update the entity with patched values - required fields are already validated above
        existingEmployee.FirstName = employeeModel.FirstName;
        existingEmployee.LastName = employeeModel.LastName;
        existingEmployee.Gender = employeeModel.Gender;
        existingEmployee.BirthDate = employeeModel.BirthDate;
        existingEmployee.HireDate = employeeModel.HireDate;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private readonly IDatabaseContext _dbContext;
}
