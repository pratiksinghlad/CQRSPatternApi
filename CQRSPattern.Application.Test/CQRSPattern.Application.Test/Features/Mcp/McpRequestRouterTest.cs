using System.Text.Json;
using CQRSPattern.Api.Features.Mcp.Models;
using CQRSPattern.Api.Features.Mcp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Mcp;

/// <summary>
/// Unit tests for McpRequestRouter
/// </summary>
public sealed class McpRequestRouterTest
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<McpRequestRouter>> _loggerMock;
    private readonly McpRequestRouter _sut;

    public McpRequestRouterTest()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<McpRequestRouter>>();
        _sut = new McpRequestRouter(_serviceProviderMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsError_WhenMethodIsEmpty()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "",
            Id = "test-001"
        };

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal("INVALID_METHOD", response.Error.Code);
        Assert.Equal("test-001", response.Id);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsError_WhenMethodNotFound()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "employee.delete",
            Id = "test-002"
        };

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal("METHOD_NOT_FOUND", response.Error.Code);
        Assert.Contains("employee.delete", response.Error.Message);
        Assert.Equal("test-002", response.Id);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsError_WhenHandlerNotRegistered()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "employee.getAll",
            Id = "test-003"
        };

        _serviceProviderMock
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(null);

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal("INTERNAL_ERROR", response.Error.Code);
        Assert.Equal("test-003", response.Id);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsSuccess_WhenHandlerExecutesSuccessfully()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "employee.getAll",
            Id = "test-004"
        };

        var mockHandler = new Mock<IMcpMethodHandler>();
        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<object?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<object> { new { id = 1, name = "Test" } });

        _serviceProviderMock
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(mockHandler.Object);

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Result);
        Assert.Null(response.Error);
        Assert.Equal("test-004", response.Id);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsValidationError_WhenArgumentException()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "employee.add",
            Id = "test-005"
        };

        var mockHandler = new Mock<IMcpMethodHandler>();
        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<object?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid parameters"));

        _serviceProviderMock
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(mockHandler.Object);

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal("VALIDATION_ERROR", response.Error.Code);
        Assert.Equal("Invalid parameters", response.Error.Message);
        Assert.Equal("test-005", response.Id);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsNotFoundError_WhenKeyNotFoundException()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "employee.update",
            Id = "test-006"
        };

        var mockHandler = new Mock<IMcpMethodHandler>();
        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<object?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Employee not found"));

        _serviceProviderMock
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(mockHandler.Object);

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal("NOT_FOUND", response.Error.Code);
        Assert.Equal("Employee not found", response.Error.Message);
        Assert.Equal("test-006", response.Id);
    }

    [Fact]
    public async Task RouteRequestAsync_ReturnsInternalError_WhenUnexpectedException()
    {
        // Arrange
        var request = new McpRequest
        {
            Method = "employee.getAll",
            Id = "test-007"
        };

        var mockHandler = new Mock<IMcpMethodHandler>();
        mockHandler
            .Setup(x => x.HandleAsync(It.IsAny<object?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        _serviceProviderMock
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(mockHandler.Object);

        // Act
        var response = await _sut.RouteRequestAsync(request, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal("INTERNAL_ERROR", response.Error.Code);
        Assert.Equal("test-007", response.Id);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceProviderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new McpRequestRouter(null!, _loggerMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new McpRequestRouter(_serviceProviderMock.Object, null!));
    }
}
