using Autofac.Extensions.DependencyInjection;

namespace CQRSPattern.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Create host
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
            });
}