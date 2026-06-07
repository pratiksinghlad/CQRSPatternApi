using CQRSPattern.Application.Features.Employee.GetById;
using CQRSPattern.Application.Repositories.Read;
using CQRSPattern.Application.Test.Builders.Employee.GetAll;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.GetById;

/// <summary>
/// Unit tests for the GetEmployeeByIdQueryHandler class.
/// </summary>
public class GetEmployeeByIdQueryHandlerTest
{
    private readonly Mock<IEmployeeReadRepository> _employeeRepository;
    private readonly GetEmployeeByIdQueryHandler _sut;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeByIdQueryHandlerTest"/> class.
    /// </summary>
    public GetEmployeeByIdQueryHandlerTest()
    {
        _employeeRepository = new Mock<IEmployeeReadRepository>(MockBehavior.Strict);
        _sut = new GetEmployeeByIdQueryHandler(_employeeRepository.Object);
    }

    /// <summary>
    /// Tests successful retrieval of an employee.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmployeeExists_ShouldReturnEmployee()
    {
        // Arrange
        const int employeeId = 1;
        var employee = new EmployeeModelBuilder().With(x => x.Id, employeeId).Build();
        var query = GetEmployeeByIdQuery.Create(employeeId);

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employeeId, default))
            .ReturnsAsync(employee);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        Assert.Same(employee, result.Data);
        _employeeRepository.Verify(x => x.GetByIdAsync(employeeId, default), Times.Once);
    }

    /// <summary>
    /// Tests retrieval when an employee is not found.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_ShouldReturnNullData()
    {
        // Arrange
        const int employeeId = 404;
        var query = GetEmployeeByIdQuery.Create(employeeId);

        _employeeRepository
            .Setup(x => x.GetByIdAsync(employeeId, default))
            .ReturnsAsync((CQRSPattern.Application.Features.Employee.EmployeeModel?)null);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        Assert.Null(result.Data);
        _employeeRepository.Verify(x => x.GetByIdAsync(employeeId, default), Times.Once);
    }
}