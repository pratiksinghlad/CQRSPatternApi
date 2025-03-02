using CQRSPattern.Infrastructure.Persistence.Factories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CQRSPattern.Api.Infra.HealthChecks;

/// <summary>
/// Check mysql database connection
/// </summary>
public class MySqlDatabaseHealthCheck : IHealthCheck
{
    /// <summary>
    /// CTor
    /// </summary>
    /// <param name="sqlConnection"></param>
    public MySqlDatabaseHealthCheck(IMySqlConnectionManager sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            using(var connection = _sqlConnection.Get())
            {
                await connection.OpenAsync(cancellationToken);
                await connection.CloseAsync();
            }

            return HealthCheckResult.Healthy("MySQL Database connection is working.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(description: $"{ex.Message}", exception: ex);
        }
    }

    private readonly IMySqlConnectionManager _sqlConnection;
}