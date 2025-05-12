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

    private readonly IDatabaseContext _dbContext;
}
