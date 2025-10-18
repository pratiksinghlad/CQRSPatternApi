# CQRSPattern MCP Server

A Model Context Protocol (MCP) server built with .NET 9 that exposes your CQRS Pattern API to GitHub Copilot and other MCP clients.

## What is MCP?

The Model Context Protocol (MCP) is an open protocol that standardizes how applications provide context to Large Language Models (LLMs). This MCP server allows GitHub Copilot to interact directly with your CQRS API.

## Features

- **Pure .NET 9** - No Node.js required
- **Official MCP SDK** - Uses `ModelContextProtocol` NuGet package
- **4 Built-in Tools**:
  - Query entities with pagination/filtering
  - Execute CQRS commands
  - Get entities by ID
  - Check API health status

## Configuration

Environment variables (set in `.vscode\mcp.json`):

- `CQRS_API_URL` - Base URL of your CQRS API (default: http://localhost:5000)
- `CQRS_API_KEY` - Optional API key for authentication

## Usage with VS Code

This MCP server is designed to run with GitHub Copilot in VS Code. The configuration is in `.vscode\mcp.json` at the repository root.

### Start the Server

The MCP server starts automatically when GitHub Copilot needs it. You can also run it manually:

```powershell
dotnet run --project CQRSPattern.McpServer
```

## Available Tools

### query_entities

Query entities from the CQRS API with optional filtering and pagination.

**Parameters:**
- `entityType` (required) - Type of entity (e.g., 'users', 'orders')
- `pageNumber` (optional) - Page number (default: 1)
- `pageSize` (optional) - Items per page (default: 10)
- `filters` (optional) - JSON string with filter criteria

**Example Copilot Query:**
```
"Query all orders from my API with page size 20"
```

### execute_command

Execute a CQRS command.

**Parameters:**
- `commandType` (required) - Command type (e.g., 'CreateUser', 'UpdateOrder')
- `payload` (required) - JSON payload for the command

**Example Copilot Query:**
```
"Create a new user with name John Doe and email john@example.com"
```

### get_entity_by_id

Retrieve a specific entity by its ID.

**Parameters:**
- `entityType` (required) - Type of entity
- `id` (required) - Entity ID

**Example Copilot Query:**
```
"Get user with ID 123"
```

### api_health_check

Check the health status of the CQRS API.

**Example Copilot Query:**
```
"Is my CQRS API running?"
```

## Adding Custom Tools

To add a new tool, add a method to the `CQRSApiTools` class in `Program.cs`:

```csharp
[McpServerTool(Name = "my_custom_tool")]
[Description("Description of what your tool does")]
public static async Task<string> MyCustomTool(
    [Description("Parameter description")] string param,
    IHttpClientFactory httpClientFactory = null!,
    CancellationToken cancellationToken = default)
{
    var client = httpClientFactory.CreateClient("CQRSApi");
    var response = await client.GetAsync($"/api/custom/{param}", cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync(cancellationToken);
}
```

## Dependencies

- `ModelContextProtocol` (0.4.0-preview.2) - Official MCP SDK
- `Microsoft.Extensions.Hosting` - Hosting infrastructure
- `Microsoft.Extensions.Http` - HttpClient factory

## Architecture

```
GitHub Copilot → MCP Protocol (stdio) → CQRSPattern.McpServer → HTTP → CQRS API
```

The MCP server:
1. Listens for MCP requests on stdin/stdout
2. Translates them to HTTP requests
3. Calls your CQRS API
4. Returns responses to Copilot

## Resources

- [Model Context Protocol C# SDK](https://github.com/modelcontextprotocol/csharp-sdk)
- [MCP Specification](https://spec.modelcontextprotocol.io/)
- [MCP Documentation](https://modelcontextprotocol.io/)
