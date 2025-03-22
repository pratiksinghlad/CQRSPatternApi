using FluentValidation;

namespace CQRSPattern.Application.Features.Employee.Add;

public class AddEmployeeCommandValidator : AbstractValidator<AddEmployeeCommand>
{
    public AddEmployeeCommandValidator()
    {
        RuleFor(cmd => cmd.FirstName)
            .NotNull()
            .NotEmpty();

        RuleFor(cmd => cmd.LastName)
            .NotNull()
            .NotEmpty();

        RuleFor(cmd => cmd.Gender)
            .NotNull()
            .NotEmpty()
            .MaximumLength(1)
            .WithMessage("Gender must be a single character.");

        // BirthDate should not be null, not empty, and employee must be at least 18 years old.
        RuleFor(cmd => cmd.BirthDate)
            .Must(birthDate => birthDate <= DateTime.Today.AddYears(-18))
            .WithMessage("Employee must be at least 18 years old.");

        RuleFor(cmd => cmd.HireDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Hire date must be today or in the future.")
            .Must((cmd, hireDate) => hireDate >= cmd.BirthDate.AddYears(18))
            .WithMessage("Employee must be at least 18 years old on the hire date.");
    }
}
