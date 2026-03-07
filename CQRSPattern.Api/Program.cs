using Autofac.Extensions.DependencyInjection;
using ModelContextProtocol.AspNetCore;

namespace CQRSPattern.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var useHttp = args.Contains("--http", StringComparer.OrdinalIgnoreCase)
                   || string.Equals(Environment.GetEnvironmentVariable("USE_HTTP"), "true", StringComparison.OrdinalIgnoreCase);

        if (useHttp)
        {
            // ✅ HTTP mode — full web server with REST + MCP at /mcp
            await RunHttpMode(args);
        }
        else
        {
            // ✅ STDIO mode — JSON-RPC over stdin/stdout (Claude Desktop, VS Code Copilot)
            await RunStdioMode(args);
        }
    }

    /// <summary>
    /// HTTP mode: WebApplication with REST controllers + MCP endpoint at /mcp.
    /// Uses the existing Startup class and Autofac DI container.
    /// </summary>
    private static async Task RunHttpMode(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    /// <summary>
    /// STDIO mode: Console host with MCP stdio transport.
    /// No HTTP server; JSON-RPC messages flow over stdin/stdout.
    /// Uses Autofac for DI to share the same service registrations.
    /// </summary>
    private static async Task RunStdioMode(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile(
                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                    true,
                    true
                );
                config.AddJsonFile("secrets/appsettings.secrets.json", true, true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Register MCP server with stdio transport
                services
                    .AddMcpServer()
                    .WithStdioServerTransport()
                    .WithToolsFromAssembly()
                    .WithResourcesFromAssembly()
                    .WithPromptsFromAssembly();

                // Register shared services needed by MCP tools
                // (ConnectionStrings config, repositories, mediator, etc.)
                var startup = new Startup(context.Configuration);
                startup.ConfigureSharedServices(services);
            })
            .ConfigureContainer<Autofac.ContainerBuilder>(builder =>
            {
                Startup.ConfigureContainer(builder);
            });

        var host = builder.Build();
        await host.RunAsync();
    }

    /// <summary>
    /// Create host for HTTP mode — preserves existing behavior.
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration(
                (context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    builder.AddJsonFile(
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                        true,
                        true
                    );
                    builder.AddJsonFile("secrets/appsettings.secrets.json", true, true);
                    builder.AddUserSecrets<Program>();
                    builder.AddEnvironmentVariables();
                }
            );
}
