using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Handlers;
using CQRSPattern.Api.Features.Employee.Add;
using CQRSPattern.Application.Features.Employee.Add;
using CQRSPattern.Application.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Mcp;

/// <summary>
/// Unit tests for EmployeeAddHandler
/// </summary>
public sealed class EmployeeAddHandlerTest
{
    private readonly Mock<IMediatorFactory> _mediatorFactoryMock;
    private readonly Mock<IMediatorScope> _mediatorScopeMock;
    private readonly Mock<ILogger<EmployeeAddHandler>> _loggerMock;
    private readonly EmployeeAddHandler _sut;

    public EmployeeAddHandlerTest()
    {
        _mediatorFactoryMock = new Mock<IMediatorFactory>();
        _mediatorScopeMock = new Mock<IMediatorScope>();
        _loggerMock = new Mock<ILogger<EmployeeAddHandler>>();

        _mediatorFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(_mediatorScopeMock.Object);

        _sut = new EmployeeAddHandler(_mediatorFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_AddsEmployee_WhenValidParams()
    {
        // Arrange
        var requestParams = new
        {
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            BirthDate = "1990-01-01T00:00:00Z",
            HireDate = "2020-01-01T00:00:00Z"
        };

        _mediatorScopeMock
            .Setup(x => x.SendAsync(It.IsAny<AddEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _sut.HandleAsync(requestParams, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var response = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(result));
        Assert.NotNull(response);
        Assert.Equal("Employee created successfully", response["message"]);
        _mediatorFactoryMock.Verify(x => x.CreateScope(), Times.Once);
        _mediatorScopeMock.Verify(x => x.SendAsync(It.IsAny<AddEmployeeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ThrowsArgumentException_WhenParamsIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _sut.HandleAsync(null, CancellationToken.None));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMediatorFactoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new EmployeeAddHandler(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new EmployeeAddHandler(_mediatorFactoryMock.Object, null!));
    }
}
