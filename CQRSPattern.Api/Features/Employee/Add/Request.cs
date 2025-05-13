using CQRSPattern.Application.Features.Employee.Add;

namespace CQRSPattern.Api.Features.Employee.Add;

#nullable disable

/// <summary>
/// Request to add a employee.
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
    /// <returns></returns>
    public AddEmployeeCommand ToMediator()
    {
        return AddEmployeeCommand.CreateCommand(FirstName, LastName, Gender, BirthDate, HireDate);
    }
}
