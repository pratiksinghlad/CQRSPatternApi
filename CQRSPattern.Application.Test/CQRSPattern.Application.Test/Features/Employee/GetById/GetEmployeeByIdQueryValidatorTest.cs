using CQRSPattern.Application.Features.Employee.GetById;
using FluentValidation.TestHelper;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.GetById;

/// <summary>
/// Unit tests for the GetEmployeeByIdQueryValidator class.
/// </summary>
public class GetEmployeeByIdQueryValidatorTest
{
    private readonly GetEmployeeByIdQueryValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeByIdQueryValidatorTest"/> class.
    /// </summary>
    public GetEmployeeByIdQueryValidatorTest()
    {
        _validator = new GetEmployeeByIdQueryValidator();
    }

    /// <summary>
    /// Tests validation with a valid employee ID.
    /// </summary>
    [Fact]
    public void Validate_WhenEmployeeIdIsValid_ShouldPassValidation()
    {
        // Arrange
        var query = GetEmployeeByIdQuery.Create(1);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests validation failure when employee ID is invalid.
    /// </summary>
    /// <param name="invalidId">The invalid employee ID.</param>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenEmployeeIdIsInvalid_ShouldHaveValidationError(int invalidId)
    {
        // Arrange
        var query = GetEmployeeByIdQuery.Create(invalidId);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Employee ID must be greater than 0");
    }
}