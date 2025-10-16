using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CQRSPattern.Application.Features.Employee.Patch;

namespace CQRSPattern.Api.Features.Employee.Patch;

/// <summary>
/// Request to partially update an employee using HTTP PATCH with C# 13 Optional pattern.
/// Distinguishes between fields not provided vs. fields explicitly set to null.
/// </summary>
public class Request
{
    /// <summary>
    /// Gets or sets the employee's first name.
    /// </summary>
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    [JsonPropertyName("firstName")]
    public Optional<string> FirstName { get; set; } = Optional<string>.None;

    /// <summary>
    /// Gets or sets the employee's last name.
    /// </summary>
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    [JsonPropertyName("lastName")]
    public Optional<string> LastName { get; set; } = Optional<string>.None;

    /// <summary>
    /// Gets or sets the employee's gender.
    /// </summary>
    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
    [JsonPropertyName("gender")]
    public Optional<string> Gender { get; set; } = Optional<string>.None;

    /// <summary>
    /// Gets or sets the employee's birth date.
    /// </summary>
    [JsonPropertyName("birthDate")]
    public Optional<DateTime> BirthDate { get; set; } = Optional<DateTime>.None;

    /// <summary>
    /// Gets or sets the employee's hire date.
    /// </summary>
    [JsonPropertyName("hireDate")]
    public Optional<DateTime> HireDate { get; set; } = Optional<DateTime>.None;

    /// <summary>
    /// Converts the request into a mediator command for partial employee updates.
    /// </summary>
    /// <param name="id">The ID of the employee to patch</param>
    /// <returns>A PatchEmployeeCommand instance</returns>
    public PatchEmployeeCommand ToMediator(int id) =>
        PatchEmployeeCommand.CreateCommand(
            id,
            FirstName.HasValue ? FirstName.Value : null,
            LastName.HasValue ? LastName.Value : null,
            Gender.HasValue ? Gender.Value : null,
            BirthDate.HasValue ? BirthDate.Value : null,
            HireDate.HasValue ? HireDate.Value : null
        );

    /// <summary>
    /// Determines if the request contains any fields to update using C# 13 pattern matching.
    /// </summary>
    /// <returns>True if at least one property has a value; otherwise, false</returns>
    public bool HasAnyUpdates() =>
        FirstName.HasValue
        || LastName.HasValue
        || Gender.HasValue
        || BirthDate.HasValue
        || HireDate.HasValue;
}
