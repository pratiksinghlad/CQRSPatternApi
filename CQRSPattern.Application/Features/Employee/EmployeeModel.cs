namespace CQRSPattern.Application.Features.Employee;

public sealed class EmployeeModel
{
    public required int Id { get; set; }
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Gender { get; set; } = string.Empty;
    public required DateTime BirthDate { get; set; }
    public required DateTime HireDate { get; set; }
}
