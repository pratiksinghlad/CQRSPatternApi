using CQRSPattern.Application.Features.Employee.Patch;
using System.ComponentModel.DataAnnotations;

namespace CQRSPattern.Api.Features.Employee.Patch;

/// <summary>
/// Request to partially update an employee using HTTP PATCH.
/// All properties are optional to support partial updates.
/// </summary>
public class Request
{
    /// <summary>
    /// Gets or sets the employee's first name.
    /// </summary>
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the employee's last name.
    /// </summary>
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the employee's gender.
    /// </summary>
    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
    public string? Gender { get; set; }

    /// <summary>
    /// Gets or sets the employee's birth date.
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Gets or sets the employee's hire date.
    /// </summary>
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Converts the request into a mediator command for partial employee updates.
    /// </summary>
    /// <param name="id">The ID of the employee to patch</param>
    /// <returns>A PatchEmployeeCommand instance</returns>
    public PatchEmployeeCommand ToMediator(int id)
    {
        return PatchEmployeeCommand.CreateCommand(
            id,
            FirstName,
            LastName,
            Gender,
            BirthDate,
            HireDate
        );
    }

    /// <summary>
    /// Determines if the request contains any fields to update.
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
