using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Repositories.Read;
using OMP.Devops.Infrastructure.Persistence.Test.Builders;
using Xunit;

namespace CQRSPattern.Infrastructure.Persistence.Test.Repositories.Read;

public class EmployeeReadRepositoryTest
{
    public EmployeeReadRepositoryTest()
    {
        _context = new FakeDbContext();
        _sut = new EmployeeReadRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_Success()
    {
        // Arrange
        var employeeEntities = new EmployeeEntityListBuilder().Build();

        _context.Employees.AddRange(employeeEntities);
        await _context.SaveChangesAsync(default);

        // Act
        var result = await _sut.GetAllAsync(default);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(1, result.First().Id);
    }

    private FakeDbContext _context;
    private readonly EmployeeReadRepository _sut;
}
