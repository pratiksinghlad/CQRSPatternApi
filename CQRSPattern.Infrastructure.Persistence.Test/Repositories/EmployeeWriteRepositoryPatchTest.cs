using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Entities;
using CQRSPattern.Infrastructure.Persistence.Repositories.Read;
using Xunit;

namespace CQRSPattern.Infrastructure.Persistence.Test.Repositories;

/// <summary>
/// Unit tests for the EmployeeWriteRepository PatchAsync method.
/// </summary>
public class EmployeeWriteRepositoryPatchTest : IDisposable
{
    private FakeDbContext _context;
    private readonly EmployeeWriteRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeWriteRepositoryPatchTest"/> class.
    /// </summary>
    public EmployeeWriteRepositoryPatchTest()
    {
        _context = new FakeDbContext();
        _repository = new EmployeeWriteRepository(_context);
    }

    /// <summary>
    /// Tests successful partial update of an employee.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenEmployeeExists_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var originalEmployee = new EmployeeEntity
        {
            FirstName = "Original",
            LastName = "Employee",
            Gender = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1)
        };

        _context.Employees.Add(originalEmployee);
        await _context.SaveChangesAsync();

        const string updatedFirstName = "UpdatedFirst";
        const string updatedLastName = "UpdatedLast";

        // Act
        var result = await _repository.PatchAsync(
            originalEmployee.Id,
            firstName: updatedFirstName,
            lastName: updatedLastName);

        // Assert
        Assert.True(result);

        var updatedEmployee = await _context.Employees.FindAsync(originalEmployee.Id);
        Assert.NotNull(updatedEmployee);
        Assert.Equal(updatedFirstName, updatedEmployee.FirstName);
        Assert.Equal(updatedLastName, updatedEmployee.LastName);
        // Verify unchanged fields
        Assert.Equal(originalEmployee.Gender, updatedEmployee.Gender);
        Assert.Equal(originalEmployee.BirthDate, updatedEmployee.BirthDate);
        Assert.Equal(originalEmployee.HireDate, updatedEmployee.HireDate);
    }

    /// <summary>
    /// Tests partial update with only one field.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenOnlyOneFieldProvided_ShouldUpdateOnlyThatField()
    {
        // Arrange
        var originalEmployee = new EmployeeEntity
        {
            FirstName = "Original",
            LastName = "Employee",
            Gender = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1)
        };

        _context.Employees.Add(originalEmployee);
        await _context.SaveChangesAsync();

        const string updatedFirstName = "UpdatedName";

        // Act
        var result = await _repository.PatchAsync(
            originalEmployee.Id,
            firstName: updatedFirstName);

        // Assert
        Assert.True(result);

        var updatedEmployee = await _context.Employees.FindAsync(originalEmployee.Id);
        Assert.NotNull(updatedEmployee);
        Assert.Equal(updatedFirstName, updatedEmployee.FirstName);
        // Verify all other fields unchanged
        Assert.Equal(originalEmployee.LastName, updatedEmployee.LastName);
        Assert.Equal(originalEmployee.Gender, updatedEmployee.Gender);
        Assert.Equal(originalEmployee.BirthDate, updatedEmployee.BirthDate);
        Assert.Equal(originalEmployee.HireDate, updatedEmployee.HireDate);
    }

    /// <summary>
    /// Tests partial update with date fields.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenDateFieldsProvided_ShouldUpdateDateFields()
    {
        // Arrange
        var originalEmployee = new EmployeeEntity
        {
            FirstName = "Original",
            LastName = "Employee",
            Gender = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1)
        };

        _context.Employees.Add(originalEmployee);
        await _context.SaveChangesAsync();

        var updatedBirthDate = new DateTime(1985, 6, 15);
        var updatedHireDate = new DateTime(2021, 3, 10);

        // Act
        var result = await _repository.PatchAsync(
            originalEmployee.Id,
            birthDate: updatedBirthDate,
            hireDate: updatedHireDate);

        // Assert
        Assert.True(result);

        var updatedEmployee = await _context.Employees.FindAsync(originalEmployee.Id);
        Assert.NotNull(updatedEmployee);
        Assert.Equal(updatedBirthDate, updatedEmployee.BirthDate);
        Assert.Equal(updatedHireDate, updatedEmployee.HireDate);
        // Verify unchanged fields
        Assert.Equal(originalEmployee.FirstName, updatedEmployee.FirstName);
        Assert.Equal(originalEmployee.LastName, updatedEmployee.LastName);
        Assert.Equal(originalEmployee.Gender, updatedEmployee.Gender);
    }

    /// <summary>
    /// Tests partial update when employee doesn't exist.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenEmployeeDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        const int nonExistentId = 999;

        // Act
        var result = await _repository.PatchAsync(
            nonExistentId,
            firstName: "UpdatedName");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests partial update with all fields provided.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenAllFieldsProvided_ShouldUpdateAllFields()
    {
        // Arrange
        var originalEmployee = new EmployeeEntity
        {
            FirstName = "Original",
            LastName = "Employee",
            Gender = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1)
        };

        _context.Employees.Add(originalEmployee);
        await _context.SaveChangesAsync();

        const string updatedFirstName = "Updated";
        const string updatedLastName = "Employee";
        const string updatedGender = "Female";
        var updatedBirthDate = new DateTime(1985, 6, 15);
        var updatedHireDate = new DateTime(2021, 3, 10);

        // Act
        var result = await _repository.PatchAsync(
            originalEmployee.Id,
            updatedFirstName,
            updatedLastName,
            updatedGender,
            updatedBirthDate,
            updatedHireDate);

        // Assert
        Assert.True(result);

        var updatedEmployee = await _context.Employees.FindAsync(originalEmployee.Id);
        Assert.NotNull(updatedEmployee);
        Assert.Equal(updatedFirstName, updatedEmployee.FirstName);
        Assert.Equal(updatedLastName, updatedEmployee.LastName);
        Assert.Equal(updatedGender, updatedEmployee.Gender);
        Assert.Equal(updatedBirthDate, updatedEmployee.BirthDate);
        Assert.Equal(updatedHireDate, updatedEmployee.HireDate);
    }

    /// <summary>
    /// Tests partial update with null values (no update for those fields).
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenNullValuesProvided_ShouldNotUpdateThoseFields()
    {
        // Arrange
        var originalEmployee = new EmployeeEntity
        {
            FirstName = "Original",
            LastName = "Employee",
            Gender = "Male",
            BirthDate = new DateTime(1990, 1, 1),
            HireDate = new DateTime(2020, 1, 1)
        };

        _context.Employees.Add(originalEmployee);
        await _context.SaveChangesAsync();

        // Act - Only provide firstName, all others are null
        var result = await _repository.PatchAsync(
            originalEmployee.Id,
            firstName: "UpdatedName",
            lastName: null,
            gender: null,
            birthDate: null,
            hireDate: null);

        // Assert
        Assert.True(result);

        var updatedEmployee = await _context.Employees.FindAsync(originalEmployee.Id);
        Assert.NotNull(updatedEmployee);
        Assert.Equal("UpdatedName", updatedEmployee.FirstName);
        // Verify all other fields remain unchanged
        Assert.Equal(originalEmployee.LastName, updatedEmployee.LastName);
        Assert.Equal(originalEmployee.Gender, updatedEmployee.Gender);
        Assert.Equal(originalEmployee.BirthDate, updatedEmployee.BirthDate);
        Assert.Equal(originalEmployee.HireDate, updatedEmployee.HireDate);
    }

    /// <summary>
    /// Disposes the test context.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }
}