using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure logging to go to stderr (required for MCP stdio protocol)
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add HTTP client for API calls
builder.Services.AddHttpClient("CQRSApi", client =>
{
    var apiUrl = Environment.GetEnvironmentVariable("CQRS_API_URL") ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiUrl);
    
    var apiKey = Environment.GetEnvironmentVariable("CQRS_API_KEY");
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }
});

// Configure MCP server with stdio transport
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// No additional transports registered here; VS Code will connect over stdio transport.

// Diagnostic: enumerate methods with the McpServerTool attribute and log them to stderr so clients/admins can verify discovery
try
{
    var execAsm = System.Reflection.Assembly.GetExecutingAssembly();
    var toolMethods = execAsm.GetTypes()
        .SelectMany(t => t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance))
        .Where(m => m.GetCustomAttributesData().Any(ad => ad.AttributeType.Name == "McpServerToolAttribute"))
        .ToList();

    if (toolMethods.Count > 0)
    {
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger("McpStartup");
        logger.LogInformation("Discovered {Count} MCP tool methods:", toolMethods.Count);
        foreach (var m in toolMethods)
        {
            // try to read the attribute named argument 'Name' if present
            var attr = m.GetCustomAttributesData().FirstOrDefault(ad => ad.AttributeType.Name == "McpServerToolAttribute");
            string toolName = m.Name;
            if (attr != null)
            {
                var named = attr.NamedArguments.FirstOrDefault(na => na.MemberName == "Name");
                if (named.TypedValue.Value is string s && !string.IsNullOrEmpty(s)) toolName = s;
            }
            logger.LogInformation(" - {Tool} => {Method}", toolName, m.DeclaringType?.FullName + "." + m.Name);
        }
    }
    else
    {
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>().CreateLogger("McpStartup");
        logger.LogWarning("No MCP tool methods discovered via reflection");
    }
}
catch (Exception ex)
{
    // write to stderr to avoid polluting stdio
    Console.Error.WriteLine($"MCP discovery diagnostics failed: {ex}");
}

var app = builder.Build();

// Temporary HTTP endpoint to invoke MCP tool logic directly (works around stdio pairing issues).
// GET /mcp/tools/get_all_employees -> returns the JSON array returned by the CQRS API's /api/employees
app.MapGet("/mcp/tools/get_all_employees", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("CQRSApi");
    var response = await client.GetAsync("/api/employees");
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStringAsync();
    // Return raw JSON content
    return Results.Content(body, "application/json");
});

await app.RunAsync();

/// <summary>
/// Tools for interacting with the CQRS Pattern API
/// </summary>
[McpServerToolType]
public static class CQRSApiTools
{
    /// <summary>
    /// Queries entities from the CQRS API
    /// </summary>
    [McpServerTool(Name = "query_entities")]
    [Description("Query entities from the CQRS API with optional filtering and pagination")]
    public static async Task<string> QueryEntities(
        [Description("The type of entity to query (e.g., 'users', 'orders', 'products')")] string entityType,
        [Description("Page number for pagination (default: 1)")] int pageNumber = 1,
        [Description("Number of items per page (default: 10)")] int pageSize = 10,
        [Description("Optional JSON string with filter criteria")] string? filters = null,
        IHttpClientFactory httpClientFactory = null!,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("CQRSApi");
        
        var queryParams = new Dictionary<string, string>
        {
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        if (!string.IsNullOrEmpty(filters))
        {
            try
            {
                var filterDict = JsonSerializer.Deserialize<Dictionary<string, string>>(filters);
                if (filterDict != null)
                {
                    foreach (var kvp in filterDict)
                    {
                        queryParams[kvp.Key] = kvp.Value;
                    }
                }
            }
            catch
            {
                // Ignore invalid filter JSON
            }
        }

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var response = await client.GetAsync($"/api/{entityType}?{queryString}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a command in the CQRS API
    /// </summary>
    [McpServerTool(Name = "execute_command")]
    [Description("Execute a CQRS command (create, update, delete operations)")]
    public static async Task<string> ExecuteCommand(
        [Description("The type of command to execute (e.g., 'CreateUser', 'UpdateOrder')")] string commandType,
        [Description("JSON payload for the command")] string payload,
        IHttpClientFactory httpClientFactory = null!,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("CQRSApi");
        
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"/api/commands/{commandType}", content, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    [McpServerTool(Name = "get_entity_by_id")]
    [Description("Retrieve a specific entity by its unique identifier")]
    public static async Task<string> GetEntityById(
        [Description("The type of entity (e.g., 'users', 'orders')")] string entityType,
        [Description("The unique identifier of the entity")] string id,
        IHttpClientFactory httpClientFactory = null!,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("CQRSApi");
        
        var response = await client.GetAsync($"/api/{entityType}/{id}", cancellationToken);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Checks the health status of the CQRS API
    /// </summary>
    [McpServerTool(Name = "api_health_check")]
    [Description("Check the health status of the CQRS API")]
    public static async Task<string> HealthCheck(
        IHttpClientFactory httpClientFactory = null!,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("CQRSApi");
        
        try
        {
            var response = await client.GetAsync("/health", cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            return $"API Health Status: {(response.IsSuccessStatusCode ? "Healthy" : "Unhealthy")}\n\n{content}";
        }
        catch (Exception ex)
        {
            return $"API Health Status: Unhealthy\n\nError: {ex.Message}";
        }
    }
}
