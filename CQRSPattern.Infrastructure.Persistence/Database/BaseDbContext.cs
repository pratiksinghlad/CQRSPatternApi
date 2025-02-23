using Microsoft.EntityFrameworkCore;

namespace CQRSPattern.Application.Infrastructure.Persistence.Database;

public class BaseDbContext : DbContext, IDatabaseContext
{
    public BaseDbContext()
    {
        Context = this;
    }

    public BaseDbContext(DbContextOptions<BaseDbContext> options)
        : base(options)
    {
        Context = this;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyConfiguration(modelBuilder);
    }

    private void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
    }

    public virtual string GetNextSequenceId(string sequenceName)
    {
        return string.Empty;
    }

    public DbContext Context { get; }
}