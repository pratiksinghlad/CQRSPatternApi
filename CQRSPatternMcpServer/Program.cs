using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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

            // Temporary HTTP endpoint to invoke MCP tool logic directly (works around stdio pairing issues)
            app.MapGet("/mcp/tools/get_all_employees", async (IHttpClientFactory httpClientFactory) =>
            {
                var client = httpClientFactory.CreateClient("CQRSApi");
                var response = await client.GetAsync("/api/employees");
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                // Return raw JSON content
                return Results.Content(body, "application/json");
            });

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