using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace CQRSPattern.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    /// <summary>
    /// Creates a unified host that runs all three protocols simultaneously:
    /// 1. REST Controllers (via Kestrel)
    /// 2. MCP HTTP Transport (via Kestrel /mcp)
    /// 3. MCP Stdio Transport (via Stdin/Stdout listener)
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            // ✅ IMPORTANT: Configure Serilog to write to StdErr
            // This prevents logs from corrupting the MCP Stdio transport (stdin/stdout)
            .UseSerilog((context, config) =>
            {
                config
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Verbose);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
                builder.AddJsonFile("secrets/appsettings.secrets.json", true, true);
                builder.AddUserSecrets<Program>();
                builder.AddEnvironmentVariables();
            })
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                Startup.ConfigureContainer(builder);
            });
}
