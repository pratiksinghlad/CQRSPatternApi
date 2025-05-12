namespace CQRSPattern.Application.Infrastructure.Infra;

/// <summary>
/// All connection strings are represented using this class
/// </summary>
public class ConnectionStrings
{
    /// <summary>
    /// Connection string to the read database
    /// </summary>
    public string ReadDb { get; set; }

    /// <summary>
    /// Connection string to the write database
    /// </summary>
    public string WriteDb { get; set; }

    /// <summary>
    /// Timeout of query
    /// </summary>
    public int SqlTimeoutInSeconds { get; set; }
}
