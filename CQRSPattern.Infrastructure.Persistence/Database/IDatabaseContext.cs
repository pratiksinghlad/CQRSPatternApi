using Microsoft.EntityFrameworkCore;

namespace CQRSPattern.Application.Infrastructure.Persistence.Database;

public interface IDatabaseContext
{ 
    /// <summary>
    /// Commit changes to database.
    /// </summary>
    /// <param name="token">Cancellation token in case process is interrupted.</param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken token = default);

    /// <summary>
    /// Access the current db context.
    /// </summary>
    DbContext Context { get; }
}