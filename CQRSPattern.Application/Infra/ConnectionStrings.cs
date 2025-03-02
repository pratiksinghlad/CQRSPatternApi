namespace CQRSPattern.Application.Infrastructure.Infra;

/// <summary>
/// All connection strings are represented using this class
/// </summary>
public class ConnectionStrings
{
    /// <summary>
    /// Connection string to the Devops database
    /// </summary>
    public string CQRSPatternDb { get; set; }

    /// <summary>
    /// Timeout of query
    /// </summary>
    public int SqlTimeoutInSeconds { get; set; }
}