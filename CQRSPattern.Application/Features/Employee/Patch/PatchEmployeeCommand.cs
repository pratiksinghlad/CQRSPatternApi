using MediatR;

namespace CQRSPattern.Application.Features.Employee.Patch;

/// <summary>
/// Command for partially updating an employee via HTTP PATCH.
/// Supports optional properties to allow partial updates.
/// </summary>
public class PatchEmployeeCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets the employee's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the employee's first name. Null indicates no update.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the employee's last name. Null indicates no update.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the employee's gender. Null indicates no update.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Gets or sets the employee's birth date. Null indicates no update.
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Gets or sets the employee's hire date. Null indicates no update.
    /// </summary>
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchEmployeeCommand"/> class.
    /// </summary>
    public PatchEmployeeCommand() { }

    /// <summary>
    /// Creates a new instance of the patch employee command.
    /// </summary>
    /// <param name="id">The employee's unique identifier</param>
    /// <param name="firstName">The employee's first name (optional)</param>
    /// <param name="lastName">The employee's last name (optional)</param>
    /// <param name="gender">The employee's gender (optional)</param>
    /// <param name="birthDate">The employee's birth date (optional)</param>
    /// <param name="hireDate">The employee's hire date (optional)</param>
    /// <returns>A new PatchEmployeeCommand instance</returns>
    public static PatchEmployeeCommand CreateCommand(
        int id,
        string? firstName = null,
        string? lastName = null,
        string? gender = null,
        DateTime? birthDate = null,
        DateTime? hireDate = null
    )
    {
        return new PatchEmployeeCommand
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            BirthDate = birthDate,
            HireDate = hireDate,
        };
    }

    /// <summary>
    /// Determines if the command contains any fields to update.
    /// </summary>
    /// <returns>True if at least one property has a value; otherwise, false</returns>
    public bool HasAnyUpdates()
    {
        return FirstName is not null ||
               LastName is not null ||
               Gender is not null ||
               BirthDate is not null ||
               HireDate is not null;
    }
}
