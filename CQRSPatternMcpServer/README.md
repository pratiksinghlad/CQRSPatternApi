
# CQRSPattern MCP Server

This project makes parts of the CQRSPattern API available as Model Context Protocol (MCP) tools. The MCP server is a small .NET host that discovers tools in the assembly (via `WithToolsFromAssembly()`) and exposes them over the stdio transport so an MCP client (such as VS Code Copilot Chat) can call them.

## Key points

- Target: .NET 9 (net9.0)
- Uses: ModelContextProtocol C# SDK (preview)
- Transport: stdio (used by VS Code MCP client). Do NOT start the server with `dotnet run` when attaching over stdio; use the compiled DLL (`dotnet <dll>`) or let VS Code launch it using `.vscode/mcp.json`.
- Tools: tools are attributed in the assembly and discovered by `WithToolsFromAssembly()` in `Program.cs`.

## Environment variables

- `CQRS_API_URL` (optional) — base address of the running CQRS API the tools will call. Default: `http://localhost:5000` in code. The `.vscode/mcp.json` in the repo sets this to `http://localhost:5001/` for local testing.
- `CQRS_API_KEY` (optional) — if your API needs a bearer token, set this and the named HttpClient will add it to requests.

## Build

From the repository root (Windows, cmd.exe or PowerShell):

```powershell
dotnet build CQRSPattern.McpServer\CQRSPattern.McpServer.csproj -c Debug
```

After a successful build the server DLL is at:

```text
CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll
```

## Run (local manual run — for debugging)

- Start the CQRS API first so MCP tools can call its endpoints (example):

```powershell
dotnet run --project CQRSPattern.Api\CQRSPattern.Api.csproj -c Debug
```

- Start the MCP server (run the compiled DLL — important: do NOT use `dotnet run` when pairing over stdio):

```powershell
dotnet .\CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll
```

You should see the host start; logs are written to stderr (the project config writes Console logs to stderr to keep stdio clear for MCP JSON messages).

## Run from VS Code (recommended for Copilot Chat / MCP client)

1. Open the workspace in VS Code.
2. Make sure the `.vscode/mcp.json` entry for `cqrspattern` is present and points to the DLL (it has been configured to use the built DLL path).
3. Start Copilot Chat (or your MCP-enabled extension) and select the `cqrspattern` server if required. The extension will spawn `dotnet <dll>` and connect over stdio.
4. Ask the chat to `list tools` or call a specific tool like `query_entities`.

Example chat prompt:

```text
Call the MCP tool "query_entities" with entityType="employees", pageNumber=1, pageSize=20 and return JSON.
```

## Troubleshooting

- If tools do not appear in the client:
  - Make sure the server is started via the DLL (no stray build output on stdout). If VS Code is launching the process from `.vscode/mcp.json`, it should already be correct.
  - Check the extension's output panel and the server's logs (stderr) for errors. Logging is configured to write to stderr to avoid interfering with stdio.
- If a tool call fails:
  - Verify `CQRS_API_URL` points at a running API instance and endpoints exist.
  - If authentication is required, set `CQRS_API_KEY` in `.vscode/mcp.json` or the environment and restart the server.

## Next improvements (optional)

- Add typed tools that return strongly-typed DTOs (instead of raw JSON strings) for better client parsing.
- Add unit tests that call tools in-process (ModelContextProtocol allows in-process invocation patterns) or add a temporary TCP test transport for automated integration tests.

## Contact / Notes

This README was generated to document how to run and use the MCP server for the CQRSPattern project. If you want, I can add a typed `GetAllEmployeesTool` and an xUnit test to demonstrate discovery and invocation.

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
