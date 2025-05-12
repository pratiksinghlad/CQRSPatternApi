using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace CQRSPattern.Infrastructure.Persistence.Factories;

public class MySqlConnectionManager : IMySqlConnectionManager, IDisposable
{
    private readonly IConfiguration _configuration;
    private MySqlConnection _mySqlConnection;
    private bool _disposed;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public MySqlConnectionManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MySqlConnection Get()
    {
        if (_mySqlConnection != null)
            return _mySqlConnection;

        // Ensure the connection string is a valid MySQL connection string.
        var connectionString = _configuration.GetConnectionString(
            Application.Constants.Database.ConnectionStringWriteDbName
        );
        _mySqlConnection = new MySqlConnection(connectionString);

        _lock.Wait();

        _lock.Release();
        return _mySqlConnection;
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
