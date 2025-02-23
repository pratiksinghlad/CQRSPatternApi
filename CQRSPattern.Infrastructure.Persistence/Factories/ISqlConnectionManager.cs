using System.Data.SqlClient;

namespace CQRSPattern.Application.Infrastructure.Persistence.Factories;

public interface ISqlConnectionManager
{
    SqlConnection Get();
}