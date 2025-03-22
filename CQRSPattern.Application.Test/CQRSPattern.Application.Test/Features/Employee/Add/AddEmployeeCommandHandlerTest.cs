using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.Add;
using CQRSPattern.Application.Repositories.Write;
using CQRSPattern.Application.Test.Builders.Employee.GetAll;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.Add;

public class AddEmployeeCommandHandlerTest
{
    public AddEmployeeCommandHandlerTest()
    {
        _employeeRepo = new Mock<IEmployeeWriteRepository>(MockBehavior.Default);
        _sut = new AddEmployeeCommandHandler(_employeeRepo.Object);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var employee = new EmployeeModelBuilder()
                       .With(x => x.Id, 0)
                       .Build();
        var cmd = new AddEmployeeCommandBuilder().Build();
        _employeeRepo.Setup(x => x.AddAsync(employee, default));

        // Act
        await _sut.Handle(cmd, default);

        // Assert
        _employeeRepo.Verify(a => a.AddAsync(It.IsAny<EmployeeModel>(), default), Times.Once);
    }

    private readonly AddEmployeeCommandHandler _sut;
    private Mock<IEmployeeWriteRepository> _employeeRepo;
}