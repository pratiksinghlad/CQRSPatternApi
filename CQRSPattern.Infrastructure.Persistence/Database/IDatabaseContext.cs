using CQRSPattern.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRSPattern.Infrastructure.Persistence.Database;

public interface IDatabaseContext
{
    /// <summary>
    /// Commit changes to database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation cancellationToken in case process is interrupted.</param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Access the current db context.
    /// </summary>
    DbContext Context { get; }

    /// <summary>
    /// Employee entities.
    /// </summary>
    public DbSet<EmployeeEntity> Employees { get; set; }

    /// <summary>
    /// Department entities.
    /// </summary>
    public DbSet<DepartmentEntity> Departments { get; set; }
}
