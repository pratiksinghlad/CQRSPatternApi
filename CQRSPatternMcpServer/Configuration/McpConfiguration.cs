namespace CQRSPattern.McpServer.Configuration;

/// <summary>
/// Configuration for MCP servers from mcp.json
/// </summary>
public sealed class McpConfiguration
{
    /// <summary>
    /// Gets or sets the collection of MCP servers
    /// </summary>
    public Dictionary<string, McpServerConfig> McpServers { get; set; } = new();

    /// <summary>
    /// Gets or sets the default server to use
    /// </summary>
    public string DefaultServer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets general settings
    /// </summary>
    public McpSettings Settings { get; set; } = new();
}

/// <summary>
/// Configuration for an individual MCP server
/// </summary>
public sealed class McpServerConfig
{
    /// <summary>
    /// Gets or sets the server type (http or stdio)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL for HTTP-based servers
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the command for stdio-based servers
    /// </summary>
    public string? Command { get; set; }

    /// <summary>
    /// Gets or sets the arguments for stdio-based servers
    /// </summary>
    public List<string> Args { get; set; } = new();

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets HTTP headers (for HTTP type)
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Gets or sets environment variables (for stdio type)
    /// </summary>
    public Dictionary<string, string> Env { get; set; } = new();

    /// <summary>
    /// Gets or sets the timeout in milliseconds
    /// </summary>
    public int Timeout { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the number of retry attempts
    /// </summary>
    public int RetryAttempts { get; set; } = 3;
}

/// <summary>
/// General MCP settings
/// </summary>
public sealed class McpSettings
{
    /// <summary>
    /// Gets or sets the API URL (HTTP)
    /// </summary>
    public string ApiUrl { get; set; } = "http://localhost:5000";

    /// <summary>
    /// Gets or sets the API URL (HTTPS)
    /// </summary>
    public string ApiUrlHttps { get; set; } = "https://localhost:5001";

    /// <summary>
    /// Gets or sets whether logging is enabled
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets the log level
    /// </summary>
    public string LogLevel { get; set; } = "Information";
}
