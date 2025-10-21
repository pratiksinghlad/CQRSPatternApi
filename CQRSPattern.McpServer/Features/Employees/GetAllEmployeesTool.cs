using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Server;

namespace CQRSPattern.McpServer.Features.Employees;

/// <summary>
/// MCP tool that retrieves all employees from the CQRS API.
/// </summary>
[McpServerToolType]
public static class GetAllEmployeesTool
{
    /// <summary>
    /// Gets all employees.
    /// </summary>
    [McpServerTool(Name = "get_all_employees")]
    [Description("Retrieves all employees from the CQRS API")]
    public static async Task<IEnumerable<EmployeeDto>> GetAllEmployees(
        IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("CQRSApi");
        var employees = await client.GetFromJsonAsync<List<EmployeeDto>>("api/employees", cancellationToken).ConfigureAwait(false);
        return employees ?? Enumerable.Empty<EmployeeDto>();
    }

    /// <summary>
    /// Employee data transfer object.
    /// </summary>
    public sealed class EmployeeDto
    {
        /// <summary>Employee identifier.</summary>
        public int Id { get; set; }

        /// <summary>Full name.</summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>Email address.</summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>Department name.</summary>
        public string Department { get; set; } = string.Empty;
    }
}
