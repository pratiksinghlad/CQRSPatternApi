using MySqlConnector;

namespace CQRSPattern.Infrastructure.Persistence.Factories;

public interface IMySqlConnectionManager
{
    MySqlConnection Get();
}
