using CQRSPattern.Application.Infrastructure.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CQRSPattern.Infrastructure.Persistence.Database;

public class RealDbContext : BaseDbContext
{
    private readonly string _connectionString;
    private readonly int _timeout;
    private readonly ILogger _logger;

    public RealDbContext(string connectionString) : base()
    {
        _connectionString = connectionString;
        _timeout = 30;
    }

    public RealDbContext(string connectionString, int timeout) : base()
    {
        _connectionString = connectionString;
        _timeout = timeout;
    }

    public RealDbContext(IOptions<ConnectionStrings> connectionStrings, ILogger<RealDbContext> logger) : base()
    {
        _connectionString = connectionStrings.Value.CQRSPatternDb;
        _timeout = connectionStrings.Value.SqlTimeoutInSeconds;
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString), dbContextOptionsBuilder =>
        {
            dbContextOptionsBuilder.MigrationsAssembly("CQRSPattern.Infrastructure.Persistence");

            // Default is 30 seconds.
            dbContextOptionsBuilder.CommandTimeout(_timeout); 
        });

        if (_logger != null)
            optionsBuilder.LogTo(msg => _logger.LogDebug(msg, null));
    }
}