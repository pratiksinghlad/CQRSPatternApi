using CQRSPattern.Application.Features.Employee;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace CQRSPattern.Application.Features.Employee.JsonPatch;

/// <summary>
/// Command for applying JSON Patch operations to an employee.
/// Supports complex patch operations like add, remove, replace, move, copy, test.
/// </summary>
public class JsonPatchEmployeeCommand : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the employee's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the JSON patch document containing operations.
    /// </summary>
    public JsonPatchDocument<EmployeeModel> PatchDocument { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPatchEmployeeCommand"/> class.
    /// </summary>
    public JsonPatchEmployeeCommand() { }

    /// <summary>
    /// Creates a new instance of the JSON patch employee command.
    /// </summary>
    /// <param name="id">The employee's unique identifier</param>
    /// <param name="patchDocument">The JSON patch document</param>
    /// <returns>A new JsonPatchEmployeeCommand instance</returns>
    public static JsonPatchEmployeeCommand CreateCommand(int id, JsonPatchDocument<EmployeeModel> patchDocument)
    {
        return new JsonPatchEmployeeCommand
        {
            Id = id,
            PatchDocument = patchDocument
        };
    }
}
