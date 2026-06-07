using MediatR;

namespace CQRSPattern.Application.Features.Employee.GetById;

/// <summary>
/// Query for retrieving a single employee by identifier.
/// </summary>
public sealed class GetEmployeeByIdQuery : IRequest<GetEmployeeByIdQueryResult>
{
    /// <summary>
    /// Gets or sets the employee identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Creates a new query for the specified employee.
    /// </summary>
    /// <param name="id">The employee identifier.</param>
    /// <returns>A new <see cref="GetEmployeeByIdQuery"/> instance.</returns>
    public static GetEmployeeByIdQuery Create(int id)
    {
        return new GetEmployeeByIdQuery { Id = id };
    }
}