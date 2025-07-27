using CQRSPattern.Application.Features.Employee.Patch;
using CQRSPattern.Application.Repositories.Write;
using CQRSPattern.Application.Test.Builders.Employee.Patch;
using Moq;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.Patch;

/// <summary>
/// Unit tests for the PatchEmployeeCommandHandler class.
/// </summary>
public class PatchEmployeeCommandHandlerTest
{
    private readonly Mock<IEmployeeWriteRepository> _employeeRepository;
    private readonly PatchEmployeeCommandHandler _sut;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchEmployeeCommandHandlerTest"/> class.
    /// </summary>
    public PatchEmployeeCommandHandlerTest()
    {
        _employeeRepository = new Mock<IEmployeeWriteRepository>(MockBehavior.Strict);
        _sut = new PatchEmployeeCommandHandler(_employeeRepository.Object);
    }

    /// <summary>
    /// Tests successful partial update of an employee.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmployeeExists_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder().Build();
        _employeeRepository
            .Setup(x => x.PatchAsync(
                command.Id,
                command.FirstName,
                command.LastName,
                command.Gender,
                command.BirthDate,
                command.HireDate,
                default))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        _employeeRepository.Verify(x => x.PatchAsync(
            command.Id,
            command.FirstName,
            command.LastName,
            command.Gender,
            command.BirthDate,
            command.HireDate,
            default), Times.Once);
    }

    /// <summary>
    /// Tests partial update with only first name.
    /// </summary>
    [Fact]
    public async Task Handle_WhenOnlyFirstNameProvided_ShouldUpdateOnlyFirstName()
    {
        // Arrange
        const int employeeId = 1;
        const string newFirstName = "UpdatedName";
        var command = new PatchEmployeeCommandBuilder().WithOnlyFirstName(employeeId, newFirstName);

        _employeeRepository
            .Setup(x => x.PatchAsync(employeeId, newFirstName, null, null, null, null, default))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        _employeeRepository.Verify(x => x.PatchAsync(
            employeeId, newFirstName, null, null, null, null, default), Times.Once);
    }

    /// <summary>
    /// Tests that KeyNotFoundException is thrown when employee is not found.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmployeeNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder().Build();
        _employeeRepository
            .Setup(x => x.PatchAsync(
                command.Id,
                command.FirstName,
                command.LastName,
                command.Gender,
                command.BirthDate,
                command.HireDate,
                default))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(command, default));

        Assert.Equal($"Employee with ID {command.Id} not found.", exception.Message);
        _employeeRepository.Verify(x => x.PatchAsync(
            command.Id,
            command.FirstName,
            command.LastName,
            command.Gender,
            command.BirthDate,
            command.HireDate,
            default), Times.Once);
    }

    /// <summary>
    /// Tests that ArgumentNullException is thrown when repository is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenRepositoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new PatchEmployeeCommandHandler(null!));
        Assert.Equal("employeeRepository", exception.ParamName);
    }
}
