using MediatR;

namespace CQRSPattern.Application.Features.Employee.Update;

public class UpdateEmployeeCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Default CTor
    /// </summary>
    public UpdateEmployeeCommand() { }

    /// <summary>
    /// Create new instance of the command.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="gender"></param>
    /// <param name="birthDate"></param>
    /// <param name="hireDate"></param>
    /// <returns></returns>
    public static UpdateEmployeeCommand CreateCommand(
        int id,
        string firstName,
        string lastName,
        string gender,
        DateTime birthDate,
        DateTime hireDate
    )
    {
        return new UpdateEmployeeCommand()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            BirthDate = birthDate,
            HireDate = hireDate,
        };
    }
}
