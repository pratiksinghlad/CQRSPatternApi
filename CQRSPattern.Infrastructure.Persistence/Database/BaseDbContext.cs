using CQRSPattern.Infrastructure.Persistence.Configuration;
using CQRSPattern.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRSPattern.Infrastructure.Persistence.Database;

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
        modelBuilder.ApplyConfiguration(new EmployeeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentEntityConfiguration());
    }

    public DbContext Context { get; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<DepartmentEntity> Departments { get; set; }
}