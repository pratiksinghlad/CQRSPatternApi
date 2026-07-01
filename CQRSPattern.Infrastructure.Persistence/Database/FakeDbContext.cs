using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CQRSPattern.Infrastructure.Persistence.Database;

public sealed class FakeDbContext : BaseDbContext
{
    private readonly string _databaseName = $"FakeDbInstance-{Guid.NewGuid():N}";
    private readonly InMemoryDatabaseRoot _databaseRoot = new();

    public FakeDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .UseInMemoryDatabase(
                _databaseName,
                _databaseRoot
            )
            .EnableServiceProviderCaching(false);
    }

    public override void Dispose()
    {
        Database.EnsureDeleted();
        base.Dispose();
    }
}