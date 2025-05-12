using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Repositories.Read;
using CQRSPattern.Application.Test.Builders.Employee.GetAll;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.GetAll;

public class GetAllQueryHandlerTest
{
    public GetAllQueryHandlerTest()
    {
        _employeeRepo = new Mock<IEmployeeReadRepository>(MockBehavior.Default);
        _sut = new GetAllQueryHandler(_employeeRepo.Object);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        var cmd = new GetAllQuery() { };

        // Arrange
        var employees = new EmployeeModelListBuilder().Build();
        _employeeRepo.Setup(serv => serv.GetAllAsync(default)).ReturnsAsync(employees);

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        Assert.True(result.Data.All(x => x.Gender == "M" && x.HireDate.Date < DateTime.Today));
        _employeeRepo.Verify(a => a.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_NoData()
    {
        // Arrange
        var cmd = new GetAllQuery() { };
        var employees = new List<EmployeeModel>();
        _employeeRepo.Setup(serv => serv.GetAllAsync(default)).ReturnsAsync(employees);

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        _employeeRepo.Verify(a => a.GetAllAsync(default), Times.Once);
        Assert.Empty(result.Data);
    }

    private readonly GetAllQueryHandler _sut;
    private Mock<IEmployeeReadRepository> _employeeRepo;
}
