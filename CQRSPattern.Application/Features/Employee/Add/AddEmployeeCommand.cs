using MediatR;

namespace CQRSPattern.Application.Features.Employee.Add;

public class AddEmployeeCommand : IRequest<Unit>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Default CTor (for builders in testing)
    /// </summary>
    public AddEmployeeCommand() { }

    /// <summary>
    /// Create new instance of the command.
    /// </summary>
    /// <param name="title">Title of the bug.</param>
    /// <returns></returns>
    public static AddEmployeeCommand CreateCommand(
        string firstName,
        string lastName,
        string gender,
        DateTime birthDate,
        DateTime hireDate
    )
    {
        return new AddEmployeeCommand()
        {
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            BirthDate = birthDate,
            HireDate = hireDate,
        };
    }
}