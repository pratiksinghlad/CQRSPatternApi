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

    /// <summary>
    /// Settings to connect to azure servicebus.
    /// </summary>
    public AsbSettings Asb { get; set; }

    /// <summary>
    /// Key to ApplicationInsights endpoint.
    /// </summary>
    public string ApplicationInsights { get; set; }
}

public class AsbSettings
{
    public string Url { get; set; }
    public string Prefix { get; set; }
    public string SharedAccessKey { get; set; }

}