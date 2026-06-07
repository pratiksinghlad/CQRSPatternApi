using FluentValidation;

namespace CQRSPattern.Application.Features.Employee.GetById;

/// <summary>
/// Validates get-by-id employee queries.
/// </summary>
public sealed class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeByIdQueryValidator"/> class.
    /// </summary>
    public GetEmployeeByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage("Employee ID must be greater than 0");
    }
}