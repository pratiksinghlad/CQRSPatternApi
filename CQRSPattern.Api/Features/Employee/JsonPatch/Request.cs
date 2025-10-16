using CQRSPattern.Application.Features.Employee;
using CQRSPattern.Application.Features.Employee.JsonPatch;
using Microsoft.AspNetCore.JsonPatch;

namespace CQRSPattern.Api.Features.Employee.JsonPatch;

/// <summary>
/// Request wrapper for JSON Patch operations on employees.
/// </summary>
public class Request
{
    /// <summary>
    /// Gets or sets the JSON patch document containing operations.
    /// </summary>
    public JsonPatchDocument<EmployeeModel> PatchDocument { get; set; } = null!;

    /// <summary>
    /// Converts the request into a mediator command for JSON patch operations.
    /// </summary>
    /// <param name="id">The ID of the employee to patch</param>
    /// <returns>A JsonPatchEmployeeCommand instance</returns>
    public JsonPatchEmployeeCommand ToMediator(int id) =>
        JsonPatchEmployeeCommand.CreateCommand(id, PatchDocument);

    /// <summary>
    /// Determines if the request contains any operations to apply.
    /// </summary>
    /// <returns>True if at least one operation is present; otherwise, false</returns>
    public bool HasAnyOperations() => 
        PatchDocument?.Operations?.Count > 0;
}
