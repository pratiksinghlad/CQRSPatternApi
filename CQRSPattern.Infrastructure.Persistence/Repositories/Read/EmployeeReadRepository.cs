using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Repositories.Read;
using CQRSPattern.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace CQRSPattern.Infrastructure.Persistence.Repositories.Read;

public class EmployeeReadRepository : IEmployeeReadRepository
{
    public EmployeeReadRepository(IDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<EmployeeModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var employees = await _dbContext
            .Employees.Select(e => e.ToModel())
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return employees;
    }

    public async Task<EmployeeModel?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _dbContext
            .Employees
            .Where(e => e.Id == id)
            .Select(e => e.ToModel())
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return employee;
    }

    private readonly IDatabaseContext _dbContext;
}
