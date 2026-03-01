using System.ComponentModel;
using System.Text.Json;
using CQRSPattern.Application.Features.Employee.Add;
using CQRSPattern.Application.Features.Employee.GetAll;
using CQRSPattern.Application.Features.Employee.Patch;
using CQRSPattern.Application.Features.Employee.Update;
using CQRSPattern.Application.Mediator;
using ModelContextProtocol.Server;

namespace CQRSPattern.Api.Features.Mcp.Tools;

/// <summary>
/// MCP tool definitions for Employee operations.
/// Each tool delegates to existing CQRS commands/queries via MediatR — no logic duplication.
/// </summary>
[McpServerToolType]
public static class EmployeeTools
{
    /// <summary>
    /// Retrieves all employees from the database
    /// </summary>
    [McpServerTool(Name = "get_all_employees")]
    [Description("Retrieve all employees from the database. Returns a list of employee records with their ID, name, gender, birth date, and hire date.")]
    public static async Task<string> GetAllEmployees(
        IMediatorFactory mediatorFactory,
        CancellationToken cancellationToken)
    {
        var scope = mediatorFactory.CreateScope();
        var result = await scope.SendAsync(GetAllQuery.Create(), cancellationToken);

        return JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    /// <summary>
    /// Adds a new employee to the database
    /// </summary>
    [McpServerTool(Name = "add_employee")]
    [Description("Add a new employee to the database. Requires first name, last name, gender, birth date, and hire date.")]
    public static async Task<string> AddEmployee(
        [Description("Employee's first name")] string firstName,
        [Description("Employee's last name")] string lastName,
        [Description("Employee's gender (e.g., 'Male', 'Female')")] string gender,
        [Description("Employee's birth date in ISO 8601 format (e.g., '1990-01-15')")] DateTime birthDate,
        [Description("Employee's hire date in ISO 8601 format (e.g., '2020-06-01')")] DateTime hireDate,
        IMediatorFactory mediatorFactory,
        CancellationToken cancellationToken)
    {
        var command = AddEmployeeCommand.CreateCommand(firstName, lastName, gender, birthDate, hireDate);

        var scope = mediatorFactory.CreateScope();
        await scope.SendAsync(command, cancellationToken);

        return JsonSerializer.Serialize(new
        {
            success = true,
            message = "Employee created successfully",
            employee = new { firstName, lastName, gender, birthDate, hireDate }
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
    }

    /// <summary>
    /// Updates an existing employee (full replacement)
    /// </summary>
    [McpServerTool(Name = "update_employee")]
    [Description("Update an existing employee's full record. All fields are required — this is a full replacement.")]
    public static async Task<string> UpdateEmployee(
        [Description("The employee's unique ID")] int id,
        [Description("Employee's first name")] string firstName,
        [Description("Employee's last name")] string lastName,
        [Description("Employee's gender (e.g., 'Male', 'Female')")] string gender,
        [Description("Employee's birth date in ISO 8601 format")] DateTime birthDate,
        [Description("Employee's hire date in ISO 8601 format")] DateTime hireDate,
        IMediatorFactory mediatorFactory,
        CancellationToken cancellationToken)
    {
        var command = UpdateEmployeeCommand.CreateCommand(id, firstName, lastName, gender, birthDate, hireDate);

        var scope = mediatorFactory.CreateScope();
        await scope.SendAsync(command, cancellationToken);

        return JsonSerializer.Serialize(new
        {
            success = true,
            message = $"Employee {id} updated successfully"
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    /// <summary>
    /// Partially updates an existing employee (only specified fields)
    /// </summary>
    [McpServerTool(Name = "patch_employee")]
    [Description("Partially update an employee. Only provide the fields you want to change — omitted fields remain unchanged.")]
    public static async Task<string> PatchEmployee(
        [Description("The employee's unique ID")] int id,
        [Description("Employee's first name (optional)")] string? firstName = null,
        [Description("Employee's last name (optional)")] string? lastName = null,
        [Description("Employee's gender (optional)")] string? gender = null,
        [Description("Employee's birth date in ISO 8601 format (optional)")] DateTime? birthDate = null,
        [Description("Employee's hire date in ISO 8601 format (optional)")] DateTime? hireDate = null,
        IMediatorFactory mediatorFactory = null!,
        CancellationToken cancellationToken = default)
    {
        var command = PatchEmployeeCommand.CreateCommand(id, firstName, lastName, gender, birthDate, hireDate);

        if (!command.HasAnyUpdates())
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "At least one field must be provided for partial update"
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        var scope = mediatorFactory.CreateScope();
        await scope.SendAsync(command, cancellationToken);

        return JsonSerializer.Serialize(new
        {
            success = true,
            message = $"Employee {id} patched successfully"
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
