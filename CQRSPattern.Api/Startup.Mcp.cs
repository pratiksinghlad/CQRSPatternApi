namespace CQRSPattern.Api;

/// <summary>
/// Startup partial — MCP (Model Context Protocol) server registration.
/// Configures the official ModelContextProtocol SDK with Streamable HTTP transport.
/// </summary>
public partial class Startup
{
    /// <summary>
    /// Registers MCP server services with Streamable HTTP transport (stateless mode).
    /// Tools, resources, and prompts are auto-discovered from the assembly via attributes.
    /// Stateless mode is required for load-balanced/containerized deployments.
    /// </summary>
    private static void LoadMcp(IServiceCollection services)
    {
        services
            .AddMcpServer()
            .WithHttpTransport(options => options.Stateless = true)
            .WithToolsFromAssembly()
            .WithResourcesFromAssembly()
            .WithPromptsFromAssembly();
    }
}
