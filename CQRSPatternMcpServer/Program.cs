using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using CQRSPattern.McpServer.Configuration;
using Serilog;

namespace CQRSPattern.McpServer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Configure Serilog for the application
            builder.Host.UseSerilog((context, config) => 
            {
                config
                    .ReadFrom.Configuration(context.Configuration)
                    .MinimumLevel.Information()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        "logs/mcp-server-.log",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .Enrich.FromLogContext();
            });

            Log.Information("Starting up MCP Server");

            // Load MCP configuration from mcp.json if it exists
            McpConfiguration? mcpConfig = null;
            
            // Try multiple locations for mcp.json
            var possiblePaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "mcp.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "../../mcp.json"),
                Path.Combine(AppContext.BaseDirectory, "mcp.json"),
                Path.Combine(AppContext.BaseDirectory, "../../mcp.json")
            };

            string? foundConfigPath = null;
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    foundConfigPath = path;
                    break;
                }
            }

            if (foundConfigPath != null)
            {
                try
                {
                    var mcpConfigJson = await File.ReadAllTextAsync(foundConfigPath);
                    mcpConfig = JsonSerializer.Deserialize<McpConfiguration>(mcpConfigJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Log.Information("Loaded MCP configuration from {Path}", foundConfigPath);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to load MCP configuration from {Path}, using defaults", foundConfigPath);
                }
            }
            else
            {
                Log.Information("No mcp.json found in any standard location, using environment variables and defaults");
            }

            // Add HTTP client for API calls
            builder.Services.AddHttpClient("CQRSApi", client =>
            {
                // Try to get API URL from mcp.json first, then environment variable, then default
                var apiUrl = mcpConfig?.Settings?.ApiUrl 
                    ?? Environment.GetEnvironmentVariable("CQRS_API_URL") 
                    ?? "http://localhost:5000";
                client.BaseAddress = new Uri(apiUrl);
                
                Log.Information("MCP Server will connect to API at: {ApiUrl}", apiUrl);
                
                var apiKey = Environment.GetEnvironmentVariable("CQRS_API_KEY");
                if (!string.IsNullOrEmpty(apiKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                    Log.Information("API key configured for requests");
                }
            });

            // Configure MCP server with stdio transport
            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            // Diagnostic: enumerate methods with the McpServerTool attribute and log them to stderr
            try
            {
                // Get logger from the ILoggingBuilder directly
                var execAsm = Assembly.GetExecutingAssembly();
                
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | 
                                          DynamicallyAccessedMemberTypes.NonPublicMethods)]
                IEnumerable<MethodInfo> GetToolMethods(
                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | 
                                              DynamicallyAccessedMemberTypes.NonPublicMethods)] Type type)
                {
                    return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | 
                                         BindingFlags.Static | BindingFlags.Instance)
                        .Where(m => m.GetCustomAttributesData()
                            .Any(ad => ad.AttributeType.Name == "McpServerToolAttribute"));
                }

                var toolMethods = execAsm.GetTypes()
                    .Where(t => !t.IsInterface && !t.IsAbstract)
                    .SelectMany(GetToolMethods)
                    .ToList();

                // Log reflection results using Serilog directly since we're in startup
                if (toolMethods.Count > 0)
                {
                    Log.Information("Discovered {Count} MCP tool methods:", toolMethods.Count);
                    
                    foreach (var m in toolMethods)
                    {
                        var attr = m.GetCustomAttributesData()
                            .FirstOrDefault(ad => ad.AttributeType.Name == "McpServerToolAttribute");
                        
                        string toolName = m.Name;
                        if (attr != null)
                        {
                            var named = attr.NamedArguments
                                .FirstOrDefault(na => na.MemberName == "Name");
                            
                            if (named.TypedValue.Value is string s && !string.IsNullOrEmpty(s))
                            {
                                toolName = s;
                            }
                        }
                        
                        Log.Information(" - {Tool} => {Method}", 
                            toolName, 
                            $"{m.DeclaringType?.FullName}.{m.Name}");
                    }
                }
                else
                {
                    Log.Warning("No MCP tool methods discovered via reflection");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MCP discovery diagnostics failed");
            }

            var app = builder.Build();

            // Add HTTP endpoints for MCP server (HTTP transport mode)
            // These endpoints allow the MCP server to be used over HTTP as well as stdio
            app.MapPost("/mcp/request", async (
                HttpContext context,
                IHttpClientFactory httpClientFactory) =>
            {
                try
                {
                    // Limit request body size to prevent memory issues (10MB max)
                    const long maxRequestSize = 10 * 1024 * 1024;
                    if (context.Request.ContentLength > maxRequestSize)
                    {
                        return Results.BadRequest(new { error = "Request body too large. Maximum size is 10MB." });
                    }

                    // Forward the request stream directly to avoid copying to memory
                    var client = httpClientFactory.CreateClient("CQRSApi");
                    
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/mcp/request")
                    {
                        Content = new StreamContent(context.Request.Body)
                    };
                    
                    requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    
                    Log.Information("Forwarding HTTP MCP request to API");
                    
                    var response = await client.SendAsync(requestMessage);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    
                    return Results.Content(responseBody, "application/json", statusCode: (int)response.StatusCode);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing HTTP MCP request");
                    return Results.Problem("Error processing MCP request", statusCode: 500);
                }
            })
            .WithName("ProcessMcpRequest")
            .WithDescription("Processes MCP requests over HTTP and forwards to the API");

            // Temporary HTTP endpoint to invoke MCP tool logic directly (works around stdio pairing issues)
            app.MapGet("/mcp/tools/get_all_employees", async (IHttpClientFactory httpClientFactory) =>
            {
                var client = httpClientFactory.CreateClient("CQRSApi");
                var response = await client.GetAsync("/api/employees");
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                // Return raw JSON content
                return Results.Content(body, "application/json");
            })
            .WithName("GetAllEmployees")
            .WithDescription("Gets all employees via direct tool call");

            // Health check endpoint for the MCP server
            app.MapGet("/health", async (IHttpClientFactory httpClientFactory) =>
            {
                try
                {
                    var client = httpClientFactory.CreateClient("CQRSApi");
                    var response = await client.GetAsync("/health/ready");
                    var isHealthy = response.IsSuccessStatusCode;
                    
                    return Results.Ok(new
                    {
                        status = isHealthy ? "healthy" : "unhealthy",
                        mcpServer = "running",
                        apiConnection = isHealthy ? "connected" : "disconnected",
                        timestamp = DateTime.UtcNow
                    });
                }
                catch
                {
                    return Results.Ok(new
                    {
                        status = "degraded",
                        mcpServer = "running",
                        apiConnection = "disconnected",
                        timestamp = DateTime.UtcNow
                    });
                }
            })
            .WithName("HealthCheck")
            .WithDescription("Health check for the MCP server");

            try
            {
                Log.Information("Starting MCP Server");
                await app.RunAsync();
                Log.Information("MCP Server stopped cleanly");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "MCP Server terminated unexpectedly");
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.Information("Shutting down MCP Server");
            Log.CloseAndFlush();
        }
    }
}