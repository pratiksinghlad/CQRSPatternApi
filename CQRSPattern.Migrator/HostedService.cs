using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CQRSPattern.Application.Constants;
using CQRSPattern.Infrastructure.Persistence.Database;

namespace CQRSPattern.Migrator;

public class HostedService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;

    public HostedService(
        ILogger<HostedService> logger,
        IHostApplicationLifetime appLifetime,
        IConfiguration configuration,
        IDatabaseContext dbContext
        )
    {
        _logger = logger;
        _configuration = configuration;
        _dbContext = dbContext;

        _appLifetime = appLifetime;

        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);

    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"1. {nameof(StartAsync)} has been called.");

        _logger.LogInformation($"Migrating database ...");
        var dbContext = new RealDbContext(_configuration.GetConnectionString(Database.ConnectionStringName), 120);
        await dbContext.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation($"Migration done ...");

        await dbContext.SaveChangesAsync(cancellationToken);
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"4. {nameof(StopAsync)} has been called.");

        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("2. OnStarted has been called.");
        _appLifetime.StopApplication();
    }

    private void OnStopping()
    {
        _logger.LogInformation("3. OnStopping has been called.");
    }

    private void OnStopped()
    {
        _logger.LogInformation("5. OnStopped has been called.");
    }

    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IDatabaseContext _dbContext;
}