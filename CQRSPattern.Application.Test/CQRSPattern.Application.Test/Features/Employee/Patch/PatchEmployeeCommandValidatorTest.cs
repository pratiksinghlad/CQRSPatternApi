using CQRSPattern.Application.Features.Employee.Patch;
using CQRSPattern.Application.Test.Builders.Employee.Patch;
using FluentValidation.TestHelper;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.Patch;

/// <summary>
/// Unit tests for the PatchEmployeeCommandValidator class.
/// </summary>
public class PatchEmployeeCommandValidatorTest
{
    private readonly PatchEmployeeCommandValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchEmployeeCommandValidatorTest"/> class.
    /// </summary>
    public PatchEmployeeCommandValidatorTest()
    {
        _validator = new PatchEmployeeCommandValidator();
    }

    /// <summary>
    /// Tests successful validation with valid partial update data.
    /// </summary>
    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPassValidation()
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder().Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests validation failure when employee ID is invalid.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenEmployeeIdIsInvalid_ShouldHaveValidationError(int invalidId)
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.Id, invalidId)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Employee ID must be greater than 0");
    }

    /// <summary>
    /// Tests validation failure when first name is empty but provided.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenFirstNameIsEmptyButProvided_ShouldHaveValidationError(string emptyFirstName)
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.FirstName, emptyFirstName)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
              .WithErrorMessage("First name cannot be empty when provided");
    }

    /// <summary>
    /// Tests validation failure when first name exceeds maximum length.
    /// </summary>
    [Fact]
    public void Validate_WhenFirstNameExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longFirstName = new string('A', 101); // 101 characters
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.FirstName, longFirstName)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
              .WithErrorMessage("First name cannot exceed 100 characters");
    }

    /// <summary>
    /// Tests validation failure when gender is invalid.
    /// </summary>
    [Theory]
    [InlineData("InvalidGender")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenGenderIsInvalid_ShouldHaveValidationError(string invalidGender)
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.Gender, invalidGender)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        if (string.IsNullOrWhiteSpace(invalidGender))
        {
            result.ShouldHaveValidationErrorFor(x => x.Gender)
                  .WithErrorMessage("Gender cannot be empty when provided");
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Gender)
                  .WithErrorMessage("Gender must be 'Male', 'Female', or 'Other'");
        }
    }

    /// <summary>
    /// Tests successful validation with valid gender values.
    /// </summary>
    //[Theory]
    //[InlineData("Male")]
    //[InlineData("Female")]
    //[InlineData("Other")]
    //[InlineData("male")] // Case insensitive
    //[InlineData("FEMALE")] // Case insensitive
    //public void Validate_WhenGenderIsValid_ShouldPassValidation(string validGender)
    //{
    //    // Arrange
    //    var command = new PatchEmployeeCommandBuilder()
    //        .With(x => x.Gender, validGender)
    //        .With(x => x.FirstName, null) // Ensure other fields don't interfere
    //        .With(x => x.LastName, null)
    //        .With(x => x.BirthDate, null)
    //        .With(x => x.HireDate, null)
    //        .Build();

    //    // Act
    //    var result = _validator.TestValidate(command);

    //    // Assert
    //    result.ShouldNotHaveValidationErrorsFor(x => x.Gender);
    //}

    /// <summary>
    /// Tests validation failure when birth date is in the future.
    /// </summary>
    [Fact]
    public void Validate_WhenBirthDateIsInFuture_ShouldHaveValidationError()
    {
        // Arrange
        var futureBirthDate = DateTime.Today.AddDays(1);
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.BirthDate, futureBirthDate)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
              .WithErrorMessage("Birth date must be in the past");
    }

    /// <summary>
    /// Tests validation failure when hire date is in the future.
    /// </summary>
    [Fact]
    public void Validate_WhenHireDateIsInFuture_ShouldHaveValidationError()
    {
        // Arrange
        var futureHireDate = DateTime.Today.AddDays(1);
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.HireDate, futureHireDate)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HireDate)
              .WithErrorMessage("Hire date cannot be in the future");
    }

    /// <summary>
    /// Tests validation failure when hire date is before birth date.
    /// </summary>
    [Fact]
    public void Validate_WhenHireDateIsBeforeBirthDate_ShouldHaveValidationError()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var hireDate = new DateTime(1989, 12, 31); // Before birth date
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.BirthDate, birthDate)
            .With(x => x.HireDate, hireDate)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HireDate)
              .WithErrorMessage("Hire date must be after birth date");
    }

    /// <summary>
    /// Tests validation failure when no fields are provided for update.
    /// </summary>
    [Fact]
    public void Validate_WhenNoFieldsProvided_ShouldHaveValidationError()
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder().WithNoUpdates(1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
              .WithErrorMessage("At least one field must be provided for partial update");
    }

    /// <summary>
    /// Tests that null values for optional fields don't cause validation errors.
    /// </summary>
    [Fact]
    public void Validate_WhenOptionalFieldsAreNull_ShouldPassValidation()
    {
        // Arrange
        var command = new PatchEmployeeCommandBuilder()
            .With(x => x.FirstName, "ValidName") // Only provide first name
            .With(x => x.LastName, null)
            .With(x => x.Gender, null)
            .With(x => x.BirthDate, null)
            .With(x => x.HireDate, null)
            .Build();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
