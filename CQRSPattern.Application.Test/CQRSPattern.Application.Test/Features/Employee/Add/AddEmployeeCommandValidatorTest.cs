using CQRSPattern.Application.Features.Employee.Add;
using CQRSPattern.Application.Test.Builders.Employee.GetAll;
using FluentValidation.TestHelper;
using Xunit;

namespace CQRSPattern.Application.Test.Features.Employee.Add;

public class AddEmployeeCommandValidatorTest
{
    public AddEmployeeCommandValidatorTest()
    {
        _validator = new AddEmployeeCommandValidator();
    }

    [Theory]
    [InlineData(null, "Doe", "M", "2025-01-01", "2005-01-01", true)] // Invalid FirstName
    [InlineData("John", "", "M", "2025-01-01", "2005-01-01", true)] // Invalid LastName
    [InlineData("John", "Doe", "", "2025-01-01", "2005-01-01", true)] // Invalid Gender
    [InlineData("John", "Doe", "M", "2010-01-01", "2005-01-01", true)] // Invalid HireDate (in the past)
    [InlineData("John", "Doe", "M", "2025-01-01", "2010-01-01", true)] // Invalid BirthDate (under 18)

    [InlineData("John", "Doe", "M", "2025-01-01", "1990-01-01", false)] // Valid BirthDate (over 18)
    [InlineData("John", "Doe", "M", "2025-01-01", "2005-01-01", false)] // Valid Input
    public void Should_Validate_All_Rules_Correctly(string firstName, string lastName, string gender, string hireDate, string birthDate, bool shouldHaveError)
    {
        // Arrange: Create the AddEmployeeCommand with the parameters provided by InlineData
        var command = new AddEmployeeCommand
        {
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            HireDate = DateTime.Parse(hireDate),
            BirthDate = DateTime.Parse(birthDate)
        };

        // Act: Validate the command
        var result = _validator.TestValidate(command);

        // Assert: Check the validation errors based on the `shouldHaveError` parameter
        if (shouldHaveError)
        {
            result.ShouldHaveAnyValidationError();
        }
        else
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    [Fact]
    public void Valid()
    {
        var command = new AddEmployeeCommandBuilder().Build();

        // Act: Validate the command
        var result = _validator.TestValidate(command);

        // Assert: Check the validation errors based on the `shouldHaveError` parameter
        Assert.True(result.IsValid);
    }

    private readonly AddEmployeeCommandValidator _validator;
}