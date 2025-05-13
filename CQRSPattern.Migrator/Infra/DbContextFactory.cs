using CQRSPattern.Application.Constants;
using CQRSPattern.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CQRSPattern.Migrator.Infra;

public class DbContextFactory : IDesignTimeDbContextFactory<ReadDbContext>
{
    public ReadDbContext CreateDbContext(string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environments.Development;

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json")
            .AddJsonFile("secrets/appsettings.secrets.json")
            .Build();

        return new ReadDbContext(
            configuration.GetConnectionString(Database.ConnectionStringWriteDbName)
        );
    }
}
