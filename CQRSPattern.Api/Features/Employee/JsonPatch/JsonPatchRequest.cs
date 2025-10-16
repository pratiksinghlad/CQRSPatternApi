using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CQRSPattern.Api.Features.Employee.JsonPatch;

/// <summary>
/// Represents a single JSON Patch operation according to RFC 6902.
/// </summary>
public class JsonPatchOperation
{
    /// <summary>
    /// The operation to be performed (add, remove, replace, move, copy, test).
    /// </summary>
    [Required]
    [JsonPropertyName("op")]
    public string Op { get; set; } = string.Empty;

    /// <summary>
    /// A JSON Pointer string specifying the location in the target document to operate on.
    /// </summary>
    [Required]
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The value to be used within the operation (not used for "remove" operations).
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    /// A string containing a JSON Pointer value that references a location within the target document
    /// (used for "move" and "copy" operations).
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }
}

/// <summary>
/// Request model for JSON Patch operations on employees.
/// Contains an array of JSON Patch operations following RFC 6902 standard.
/// </summary>
public class JsonPatchRequest
{
    /// <summary>
    /// Gets or sets the array of JSON Patch operations to be applied.
    /// </summary>
    /// <example>
    /// [
    ///   { "op": "replace", "path": "/firstName", "value": "Jane" },
    ///   { "op": "replace", "path": "/birthDate", "value": "1985-06-15T00:00:00Z" },
    ///   { "op": "test", "path": "/gender", "value": "Female" },
    ///   { "op": "remove", "path": "/lastName" }
    /// ]
    /// </example>
    [Required]
    [MinLength(1, ErrorMessage = "At least one operation must be provided")]
    public JsonPatchOperation[] Operations { get; set; } = [];

    /// <summary>
    /// Validates that all operations have valid op values.
    /// </summary>
    /// <returns>True if all operations are valid; otherwise, false</returns>
    public bool IsValid()
    {
        if (Operations == null || Operations.Length == 0)
            return false;

        var validOps = new[] { "add", "remove", "replace", "move", "copy", "test" };
        
        return Operations.All(op => 
            !string.IsNullOrWhiteSpace(op.Op) && 
            validOps.Contains(op.Op.ToLowerInvariant()) &&
            !string.IsNullOrWhiteSpace(op.Path) &&
            op.Path.StartsWith('/'));
    }
}
