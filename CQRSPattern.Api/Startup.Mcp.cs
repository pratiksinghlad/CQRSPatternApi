using AgentGovernance.Extensions.ModelContextProtocol;

namespace CQRSPattern.Api;

/// <summary>
/// Startup partial — MCP (Model Context Protocol) server registration.
/// Configures the official ModelContextProtocol SDK with dual-transport support
/// and Agent Governance Toolkit for startup scanning, policy enforcement,
/// and response sanitization.
/// </summary>
public partial class Startup
{
    /// <summary>
    /// Registers MCP server services with both HTTP and Stdio transports enabled,
    /// governed by the Agent Governance Toolkit.
    /// </summary>
    private void LoadMcp(IServiceCollection services)
    {
        var governanceSection = Configuration.GetSection("McpGovernance");

        services
            .AddMcpServer()
            .WithHttpTransport(options => options.Stateless = true)
            .WithStdioServerTransport()
            .WithToolsFromAssembly()
            .WithResourcesFromAssembly()
            .WithPromptsFromAssembly()
            .WithGovernance(options =>
            {
                // Policy file is deployed alongside the binary (see .csproj Content item).
                options.PolicyPaths.Add(governanceSection["PolicyPath"] ?? "policies/mcp.yaml");
                options.DefaultAgentId = governanceSection["DefaultAgentId"] ?? "did:mcp:anonymous";
                options.ServerName = governanceSection["ServerName"] ?? "cqrs-api";
            });
    }
}