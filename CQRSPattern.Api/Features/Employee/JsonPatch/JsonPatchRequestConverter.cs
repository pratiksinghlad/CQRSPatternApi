using CQRSPattern.Application.Features.Employee;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace CQRSPattern.Api.Features.Employee.JsonPatch;

/// <summary>
/// Converter utility to transform JsonPatchRequest to JsonPatchDocument.
/// </summary>
public static class JsonPatchRequestConverter
{
    /// <summary>
    /// Converts a JsonPatchRequest to a JsonPatchDocument&lt;EmployeeModel&gt;.
    /// </summary>
    /// <param name="request">The JSON patch request to convert</param>
    /// <returns>A JsonPatchDocument instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when operation is invalid</exception>
    public static JsonPatchDocument<EmployeeModel> ToJsonPatchDocument(this JsonPatchRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var patchDocument = new JsonPatchDocument<EmployeeModel>();

        foreach (var operation in request.Operations)
        {
            switch (operation.Op.ToLowerInvariant())
            {
                case "add":
                    AddOperationByPath(patchDocument, operation.Path, operation.Value);
                    break;
                    
                case "remove":
                    RemoveOperationByPath(patchDocument, operation.Path);
                    break;
                    
                case "replace":
                    ReplaceOperationByPath(patchDocument, operation.Path, operation.Value);
                    break;
                    
                case "move":
                    if (string.IsNullOrWhiteSpace(operation.From))
                        throw new InvalidOperationException("Move operation requires 'from' property");
                    MoveOperationByPath(patchDocument, operation.From, operation.Path);
                    break;
                    
                case "copy":
                    if (string.IsNullOrWhiteSpace(operation.From))
                        throw new InvalidOperationException("Copy operation requires 'from' property");
                    CopyOperationByPath(patchDocument, operation.From, operation.Path);
                    break;
                    
                case "test":
                    TestOperationByPath(patchDocument, operation.Path, operation.Value);
                    break;
                    
                default:
                    throw new InvalidOperationException($"Unsupported operation: {operation.Op}");
            }
        }

        return patchDocument;
    }

    /// <summary>
    /// Creates an expression for a property based on the JSON Patch path.
    /// </summary>
    /// <param name="path">The JSON Patch path (e.g., "/firstName")</param>
    /// <returns>An expression representing the property</returns>
    private static Expression<Func<EmployeeModel, object?>> GetPropertyExpression(string path)
    {
        // Map JSON Patch paths to actual property names with correct casing
        var propertyName = path.TrimStart('/').ToLowerInvariant() switch
        {
            "firstname" => nameof(EmployeeModel.FirstName),
            "lastname" => nameof(EmployeeModel.LastName),
            "gender" => nameof(EmployeeModel.Gender),
            "birthdate" => nameof(EmployeeModel.BirthDate),
            "hiredate" => nameof(EmployeeModel.HireDate),
            "id" => nameof(EmployeeModel.Id),
            _ => throw new InvalidOperationException($"Unsupported path '{path}' for EmployeeModel")
        };

        var parameter = Expression.Parameter(typeof(EmployeeModel), "x");
        
        var property = typeof(EmployeeModel).GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance);
        
        if (property == null)
        {
            throw new InvalidOperationException(
                $"Property '{propertyName}' not found on EmployeeModel");
        }

        var propertyAccess = Expression.Property(parameter, property);
        var objectConversion = Expression.Convert(propertyAccess, typeof(object));
        
        return Expression.Lambda<Func<EmployeeModel, object?>>(objectConversion, parameter);
    }

    /// <summary>
    /// Adds an operation by path.
    /// </summary>
    private static void AddOperationByPath(JsonPatchDocument<EmployeeModel> patchDocument, string path, object? value)
    {
        var expression = GetPropertyExpression(path);
        patchDocument.Add(expression, value);
    }

    /// <summary>
    /// Removes an operation by path.
    /// </summary>
    private static void RemoveOperationByPath(JsonPatchDocument<EmployeeModel> patchDocument, string path)
    {
        var expression = GetPropertyExpression(path);
        patchDocument.Remove(expression);
    }

    /// <summary>
    /// Replaces an operation by path.
    /// </summary>
    private static void ReplaceOperationByPath(JsonPatchDocument<EmployeeModel> patchDocument, string path, object? value)
    {
        var expression = GetPropertyExpression(path);
        patchDocument.Replace(expression, value);
    }

    /// <summary>
    /// Moves an operation by path.
    /// </summary>
    private static void MoveOperationByPath(JsonPatchDocument<EmployeeModel> patchDocument, string fromPath, string toPath)
    {
        var fromExpression = GetPropertyExpression(fromPath);
        var toExpression = GetPropertyExpression(toPath);
        patchDocument.Move(fromExpression, toExpression);
    }

    /// <summary>
    /// Copies an operation by path.
    /// </summary>
    private static void CopyOperationByPath(JsonPatchDocument<EmployeeModel> patchDocument, string fromPath, string toPath)
    {
        var fromExpression = GetPropertyExpression(fromPath);
        var toExpression = GetPropertyExpression(toPath);
        patchDocument.Copy(fromExpression, toExpression);
    }

    /// <summary>
    /// Tests an operation by path.
    /// </summary>
    private static void TestOperationByPath(JsonPatchDocument<EmployeeModel> patchDocument, string path, object? value)
    {
        var expression = GetPropertyExpression(path);
        patchDocument.Test(expression, value);
    }

    /// <summary>
    /// Validates that the JSON patch operations are semantically correct for EmployeeModel.
    /// </summary>
    /// <param name="request">The JSON patch request to validate</param>
    /// <returns>A list of validation errors, empty if valid</returns>
    public static List<string> ValidateForEmployee(this JsonPatchRequest request)
    {
        var errors = new List<string>();
        var validPaths = new[] { "/firstName", "/lastName", "/gender", "/birthDate", "/hireDate", "/id" };

        foreach (var operation in request.Operations)
        {
            // Validate path
            if (!validPaths.Any(vp => operation.Path.Equals(vp, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"Invalid path '{operation.Path}'. Valid paths are: {string.Join(", ", validPaths)}");
            }

            // Validate ID operations (should not allow modification of ID)
            if (operation.Path.Equals("/id", StringComparison.OrdinalIgnoreCase) && 
                operation.Op.ToLowerInvariant() is "add" or "replace")
            {
                errors.Add("Cannot modify employee ID");
            }

            // Validate date formats
            if ((operation.Path.Equals("/birthDate", StringComparison.OrdinalIgnoreCase) || 
                 operation.Path.Equals("/hireDate", StringComparison.OrdinalIgnoreCase)) &&
                operation.Value != null)
            {
                if (operation.Value is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.String)
                    {
                        if (!DateTime.TryParse(jsonElement.GetString(), out _))
                        {
                            errors.Add($"Invalid date format in path '{operation.Path}'. Use ISO 8601 format (e.g., '1990-01-01T00:00:00Z')");
                        }
                    }
                }
                else if (operation.Value is string dateString)
                {
                    if (!DateTime.TryParse(dateString, out _))
                    {
                        errors.Add($"Invalid date format in path '{operation.Path}'. Use ISO 8601 format (e.g., '1990-01-01T00:00:00Z')");
                    }
                }
            }
        }

        return errors;
    }
}
