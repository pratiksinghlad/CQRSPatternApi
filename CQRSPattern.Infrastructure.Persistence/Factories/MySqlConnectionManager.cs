using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace CQRSPattern.Infrastructure.Persistence.Factories;

/// <summary>
/// Manages MySQL connections with thread-safe access and proper resource disposal
/// </summary>
public class MySqlConnectionManager : IMySqlConnectionManager, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MySqlConnectionManager> _logger;
    private MySqlConnection _mySqlConnection;
    private bool _disposed;
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlConnectionManager"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration</param>
    /// <param name="logger">The logger instance</param>
    public MySqlConnectionManager(IConfiguration configuration, ILogger<MySqlConnectionManager> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public MySqlConnection Get()
    {
        _lock.Wait();
        try
        {
            if (_mySqlConnection != null)
            {
                return _mySqlConnection;
            }

            var connectionString = _configuration.GetConnectionString(
                Application.Constants.Database.ConnectionStringWriteDbName
            );
            _mySqlConnection = new MySqlConnection(connectionString);
            return _mySqlConnection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MySqlConnectionManager()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing && _mySqlConnection != null)
        {
            _mySqlConnection.Dispose();
        }

        _disposed = true;
    }
}