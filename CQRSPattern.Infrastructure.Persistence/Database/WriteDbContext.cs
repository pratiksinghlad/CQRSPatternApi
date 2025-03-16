using CQRSPattern.Application.Infrastructure.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CQRSPattern.Infrastructure.Persistence.Database;

public class WriteDbContext : BaseDbContext, IWriteDbContext
{
    private readonly string _connectionString;
    private readonly int _timeout;
    private readonly ILogger _logger;

    public WriteDbContext(string connectionString) : base()
    {
        _connectionString = connectionString;
        _timeout = 30;
    }

    public WriteDbContext(string connectionString, int timeout) : base()
    {
        _connectionString = connectionString;
        _timeout = timeout;
    }

    public WriteDbContext(IOptions<ConnectionStrings> connectionStrings, ILogger<WriteDbContext> logger) : base()
    {
        _connectionString = connectionStrings.Value.ReadDb;
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