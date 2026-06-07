namespace CQRSPattern.Application.Features.Employee.GetById;

/// <summary>
/// Result for retrieving a single employee by identifier.
/// </summary>
public sealed class GetEmployeeByIdQueryResult
{
    /// <summary>
    /// Gets or sets the employee when found; otherwise, null.
    /// </summary>
    public EmployeeModel? Data { get; set; }
}