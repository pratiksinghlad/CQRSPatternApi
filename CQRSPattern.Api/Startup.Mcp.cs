using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.AspNetCore;

namespace CQRSPattern.Api;

/// <summary>
/// Startup partial — MCP (Model Context Protocol) server registration.
/// Configures the official ModelContextProtocol SDK with dual-transport support.
/// </summary>
public partial class Startup
{
    /// <summary>
    /// Registers MCP server services with both HTTP and Stdio transports enabled.
    /// 1. HTTP Transport: Accessible via POST/GET /mcp (remote clients, Postman)
    /// 2. Stdio Transport: Accessible via stdin/stdout (local clients, Claude Desktop)
    /// </summary>
    private static void LoadMcp(IServiceCollection services)
    {
        services
            .AddMcpServer()
            // Support HTTP Transport (Stateless mode for production readiness)
            .WithHttpTransport(options => options.Stateless = true)
            // Support Stdio Transport (Direct pipe for desktop LLM integration)
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            .WithResourcesFromAssembly()
            .WithPromptsFromAssembly();
    }
}
