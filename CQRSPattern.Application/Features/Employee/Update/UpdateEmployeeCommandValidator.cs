﻿using FluentValidation;

namespace CQRSPattern.Application.Features.Employee.Update;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
           .GreaterThan(0);

        RuleFor(cmd => cmd.FirstName)
            .NotNull()
            .NotEmpty();

        RuleFor(cmd => cmd.LastName)
            .NotNull()
            .NotEmpty();

        RuleFor(cmd => cmd.Gender)
            .NotNull()
            .NotEmpty();

        RuleFor(cmd => cmd.HireDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Hire date must be today or in the future.");

        // BirthDate should not be null, not empty, and employee must be at least 18 years old.
        RuleFor(cmd => cmd.BirthDate)
            .Must(birthDate => birthDate <= DateTime.Today.AddYears(-18))
            .WithMessage("Employee must be at least 18 years old.");
    }
}