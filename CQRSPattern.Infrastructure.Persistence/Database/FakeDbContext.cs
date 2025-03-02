using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CQRSPattern.Infrastructure.Persistence.Database;

public sealed class FakeDbContext : BaseDbContext
{
    public FakeDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var randomUniqueInMemoryDatabaseInstanceName = "FakeDbInstance";

        optionsBuilder.UseInMemoryDatabase(randomUniqueInMemoryDatabaseInstanceName, new InMemoryDatabaseRoot())
            .EnableServiceProviderCaching(false);
    }

    public override void Dispose()
    {
        Database.EnsureDeleted();
    }
}