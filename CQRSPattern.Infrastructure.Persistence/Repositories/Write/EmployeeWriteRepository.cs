using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Repositories.Write;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Entities;

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
        var existingEmployee = await _dbContext.Employees.FindAsync(
            new object[] { id },
            cancellationToken
        );

        if (existingEmployee == null)
        {
            return false;
        }

        // Update only the fields that are provided (not null)
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

        _dbContext.Employees.Update(existingEmployee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private readonly IDatabaseContext _dbContext;
}
