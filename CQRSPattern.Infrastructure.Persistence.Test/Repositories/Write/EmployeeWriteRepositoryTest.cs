using CQRSPattern.Application.Test.Builders.Employee.GetAll;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Repositories.Read;
using Xunit;

namespace CQRSPattern.Infrastructure.Persistence.Test.Repositories.Write;

public class EmployeeWriteRepositoryTest
{
    public EmployeeWriteRepositoryTest()
    {
        _context = new FakeDbContext();
        _sut = new EmployeeWriteRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_Success()
    {
        // Arrange
        var employee = new EmployeeModelBuilder().Build();

        // Act
        await _sut.AddAsync(employee, default);

        var result = _context.Employees.First();

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Pratik", result.FirstName);
    }

    private FakeDbContext _context;
    private readonly EmployeeWriteRepository _sut;
}
