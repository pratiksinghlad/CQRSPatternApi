using CQRSPattern.Application.Features.Employee.Update;

namespace CQRSPattern.Api.Features.Employee.Update;

/// <summary>
/// Request to update a employee.
/// </summary>
public class Request
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Convert request into a mediator command.
    /// </summary>
    /// <param name="id">Id of the employee.</param>
    /// <returns></returns>
    public UpdateEmployeeCommand ToMediator(int id)
    {
        return UpdateEmployeeCommand.CreateCommand(
            id,
            FirstName,
            LastName,
            Gender,
            BirthDate,
            HireDate
        );
    }
}
