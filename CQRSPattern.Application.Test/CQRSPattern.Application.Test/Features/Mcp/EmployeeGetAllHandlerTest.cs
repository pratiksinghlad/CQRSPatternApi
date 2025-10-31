using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Handlers;
using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Mcp;

/// <summary>
/// Unit tests for EmployeeGetAllHandler
/// </summary>
public sealed class EmployeeGetAllHandlerTest
{
    private readonly Mock<IMediatorFactory> _mediatorFactoryMock;
    private readonly Mock<IMediatorScope> _mediatorScopeMock;
    private readonly Mock<ILogger<EmployeeGetAllHandler>> _loggerMock;
    private readonly EmployeeGetAllHandler _sut;

    public EmployeeGetAllHandlerTest()
    {
        _mediatorFactoryMock = new Mock<IMediatorFactory>();
        _mediatorScopeMock = new Mock<IMediatorScope>();
        _loggerMock = new Mock<ILogger<EmployeeGetAllHandler>>();

        _mediatorFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(_mediatorScopeMock.Object);

        _sut = new EmployeeGetAllHandler(_mediatorFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmployees_WhenSuccessful()
    {
        // Arrange
        var expectedEmployees = new List<EmployeeModel>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Gender = "Male", BirthDate = DateTime.Parse("1990-01-01"), HireDate = DateTime.Parse("2020-01-01") },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Gender = "Female", BirthDate = DateTime.Parse("1992-05-15"), HireDate = DateTime.Parse("2021-03-01") }
        };

        var queryResult = new GetAllQueryResult { Data = expectedEmployees };

        _mediatorScopeMock
            .Setup(x => x.SendAsync(It.IsAny<GetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        // Act
        var result = await _sut.HandleAsync(null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var employees = Assert.IsAssignableFrom<IEnumerable<EmployeeModel>>(result);
        Assert.Equal(2, employees.Count());
        _mediatorFactoryMock.Verify(x => x.CreateScope(), Times.Once);
        _mediatorScopeMock.Verify(x => x.SendAsync(It.IsAny<GetAllQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyList_WhenNoEmployees()
    {
        // Arrange
        var queryResult = new GetAllQueryResult { Data = [] };

        _mediatorScopeMock
            .Setup(x => x.SendAsync(It.IsAny<GetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);

        // Act
        var result = await _sut.HandleAsync(null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var employees = Assert.IsAssignableFrom<IEnumerable<EmployeeModel>>(result);
        Assert.Empty(employees);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMediatorFactoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new EmployeeGetAllHandler(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new EmployeeGetAllHandler(_mediatorFactoryMock.Object, null!));
    }
}
