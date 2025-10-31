using System.ComponentModel;
using System.Text;
using System.Text.Json;
using ModelContextProtocol.Server;


namespace CQRSPattern.McpServer
{
    /// <summary>
    /// Tools for interacting with the CQRS Pattern API
    /// </summary>
    [McpServerToolType]
    public static class CQRSTools
    {
        /// <summary>
        /// Queries entities from the CQRS API
        /// </summary>
        [McpServerTool(Name = "query_entities")]
        [Description("Query entities from the CQRS API with optional filtering and pagination")]
        public static async Task<string> QueryEntities(
            [Description("The type of entity to query (e.g., 'users', 'orders', 'products')")] string entityType,
            [Description("Page number for pagination (default: 1)")] int pageNumber = 1,
            [Description("Number of items per page (default: 10)")] int pageSize = 10,
            [Description("Optional JSON string with filter criteria")] string? filters = null,
            IHttpClientFactory httpClientFactory = null!,
            CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient("CQRSApi");
            
            var queryParams = new Dictionary<string, string>
            {
                ["pageNumber"] = pageNumber.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (!string.IsNullOrEmpty(filters))
            {
                try
                {
                    var filterDict = JsonSerializer.Deserialize<Dictionary<string, string>>(filters);
                    if (filterDict != null)
                    {
                        foreach (var kvp in filterDict)
                        {
                            queryParams[kvp.Key] = kvp.Value;
                        }
                    }
                }
                catch
                {
                    // Ignore invalid filter JSON
                }
            }

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var response = await client.GetAsync($"/api/{entityType}?{queryString}", cancellationToken);
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <summary>
        /// Executes a command in the CQRS API
        /// </summary>
        [McpServerTool(Name = "execute_command")]
        [Description("Execute a CQRS command (create, update, delete operations)")]
        public static async Task<string> ExecuteCommand(
            [Description("The type of command to execute (e.g., 'CreateUser', 'UpdateOrder')")] string commandType,
            [Description("JSON payload for the command")] string payload,
            IHttpClientFactory httpClientFactory = null!,
            CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient("CQRSApi");
            
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/commands/{commandType}", content, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        [McpServerTool(Name = "get_entity_by_id")]
        [Description("Retrieve a specific entity by its unique identifier")]
        public static async Task<string> GetEntityById(
            [Description("The type of entity (e.g., 'users', 'orders')")] string entityType,
            [Description("The unique identifier of the entity")] string id,
            IHttpClientFactory httpClientFactory = null!,
            CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient("CQRSApi");
            
            var response = await client.GetAsync($"/api/{entityType}/{id}", cancellationToken);
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <summary>
        /// Checks the health status of the CQRS API
        /// </summary>
        [McpServerTool(Name = "api_health_check")]
        [Description("Check the health status of the CQRS API")]
        public static async Task<string> HealthCheck(
            IHttpClientFactory httpClientFactory = null!,
            CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient("CQRSApi");
            
            try
            {
                var response = await client.GetAsync("/health", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                
                return $"API Health Status: {(response.IsSuccessStatusCode ? "Healthy" : "Unhealthy")}\n\n{content}";
            }
            catch (Exception ex)
            {
                return $"API Health Status: Unhealthy\n\nError: {ex.Message}";
            }
        }
    }
}