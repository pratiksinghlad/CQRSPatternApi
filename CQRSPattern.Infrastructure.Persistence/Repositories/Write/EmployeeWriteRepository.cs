using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Repositories.Write;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Entities;

namespace CQRSPattern.Infrastructure.Persistence.Repositories.Read;

public class EmployeeWriteRepository : IEmployeeWriteRepository
{
    public EmployeeWriteRepository(WriteDbContext dbContext)
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
            HireDate = employee.HireDate
        };

        await _dbContext.Employees.AddAsync(employeeEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EmployeeModel employee, CancellationToken cancellationToken)
    {
        var existingEmployee = await _dbContext.Employees.FindAsync(new object[] { employee.Id }, cancellationToken);

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

    private readonly WriteDbContext _dbContext;
}