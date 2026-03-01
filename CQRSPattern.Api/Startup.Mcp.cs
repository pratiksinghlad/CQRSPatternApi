namespace CQRSPattern.Api;

/// <summary>
/// Startup partial — MCP (Model Context Protocol) server registration.
/// Configures the official ModelContextProtocol SDK with Streamable HTTP transport.
/// </summary>
public partial class Startup
{
    /// <summary>
    /// Registers MCP server services with Streamable HTTP transport.
    /// Tools are auto-discovered from the assembly via [McpServerToolType] attributes.
    /// </summary>
    private static void LoadMcp(IServiceCollection services)
    {
        services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();
    }
}
