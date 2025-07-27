using FluentValidation;

namespace CQRSPattern.Application.Features.Employee.Patch;

/// <summary>
/// Validator for the PatchEmployeeCommand to ensure data integrity for partial updates.
/// </summary>
public class PatchEmployeeCommandValidator : AbstractValidator<PatchEmployeeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatchEmployeeCommandValidator"/> class.
    /// </summary>
    public PatchEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Employee ID must be greater than 0");

        // Validate FirstName only if it's provided (not null)
        When(x => x.FirstName is not null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name cannot be empty when provided")
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters");
        });

        // Validate LastName only if it's provided (not null)
        When(x => x.LastName is not null, () =>
        {
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name cannot be empty when provided")
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters");
        });

        // Validate Gender only if it's provided (not null)
        When(x => x.Gender is not null, () =>
        {
            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender cannot be empty when provided")
                .MaximumLength(10)
                .WithMessage("Gender cannot exceed 10 characters")
                .Must(gender => IsValidGender(gender!))
                .WithMessage("Gender must be 'Male', 'Female', or 'Other'");
        });

        // Validate BirthDate only if it's provided (not null)
        When(x => x.BirthDate.HasValue, () =>
        {
            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Today)
                .WithMessage("Birth date must be in the past")
                .GreaterThan(DateTime.Today.AddYears(-120))
                .WithMessage("Birth date cannot be more than 120 years ago");
        });

        // Validate HireDate only if it's provided (not null)
        When(x => x.HireDate.HasValue, () =>
        {
            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Hire date cannot be in the future");
        });

        // Validate that BirthDate is before HireDate when both are provided
        When(x => x.BirthDate.HasValue && x.HireDate.HasValue, () =>
        {
            RuleFor(x => x.HireDate)
                .GreaterThan(x => x.BirthDate)
                .WithMessage("Hire date must be after birth date");
        });

        // Ensure at least one field is provided for update
        RuleFor(x => x)
            .Must(x => x.HasAnyUpdates())
            .WithMessage("At least one field must be provided for partial update");
    }

    /// <summary>
    /// Validates if the provided gender value is acceptable.
    /// </summary>
    /// <param name="gender">The gender value to validate</param>
    /// <returns>True if the gender is valid; otherwise, false</returns>
    private static bool IsValidGender(string gender)
    {
        var validGenders = new[] { "Male", "Female", "Other" };
        return validGenders.Any(vg => string.Equals(vg, gender, StringComparison.OrdinalIgnoreCase));
    }
}
