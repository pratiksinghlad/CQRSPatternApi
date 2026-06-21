# Quick Start Guide - MCP Client and Server

This guide shows you how to run the MCP server and test it with both the stdio client and VS Code Copilot Chat.

## What was built

✅ **MCP Server** (`CQRSPattern.McpServer`)
- Exposes tools via stdio transport
- Includes typed `get_all_employees` tool
- Includes generic `query_entities`, `execute_command`, `get_entity_by_id`, `api_health_check` tools

✅ **Stdio Test Client** (`tools/McpStdioClient`)
- Console app that calls MCP server over stdio
- Useful for testing without VS Code

## Prerequisites

Ensure you have:
- .NET 9 SDK installed
- CQRS API running (or ready to start)

## Step-by-step testing

### 1. Start the CQRS API

Open a terminal and run:

```powershell
dotnet run --project CQRSPattern.Api\CQRSPattern.Api.csproj -c Debug
```

Verify it's running:
```powershell
curl http://localhost:5001/health
# or
curl http://localhost:5001/api/employees
```

### 2. Build everything (if not done)

```powershell
# Build MCP server
dotnet build CQRSPattern.McpServer\CQRSPattern.McpServer.csproj -c Debug

# Build test client
dotnet build tools\McpStdioClient\McpStdioClient.csproj -c Debug
```

### 3. Test with stdio client

Call the typed `get_all_employees` tool:

```powershell
dotnet run --project tools\McpStdioClient -- "CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll" "tools\McpStdioClient\request-get-all-employees.json"
```

Call the generic `query_entities` tool:

```powershell
dotnet run --project tools\McpStdioClient -- "CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll" "tools\McpStdioClient\request-query-entities.json"
```

Expected output: JSON array of employee objects.

### 4. Test with VS Code Copilot Chat

The `.vscode/mcp.json` is already configured to launch the MCP server.

**Steps:**
1. Open VS Code with this workspace
2. Open Copilot Chat
3. Ensure the `cqrspattern` MCP server is selected/started
4. Paste one of these prompts:

**Get all employees (typed tool):**
```text
Call the MCP tool "get_all_employees" with {} and return the result as JSON.
```

**Query employees (generic tool):**
```text
Call the MCP tool "query_entities" with entityType="employees", pageNumber=1, pageSize=100 and return JSON.
```

## Troubleshooting

### Tools don't appear in VS Code
- Ensure `.vscode/mcp.json` points to the built DLL (not `dotnet run`)
- Check VS Code extension output panel for errors
- Verify the DLL path exists: `CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll`

### Tool calls fail with HTTP errors
- Ensure CQRS API is running on the URL in `.vscode/mcp.json` (default: `http://localhost:5001/`)
- Check API has `/api/employees` endpoint
- Set `CQRS_API_KEY` in `.vscode/mcp.json` if authentication is required

### Stdio client shows connection errors
- Do NOT use `dotnet run` for the MCP server when testing stdio (it writes to stdout)
- Use the compiled DLL path directly
- Check server stderr output (printed by client with `[SERVER]` prefix)

## Files created

- `CQRSPattern.McpServer/Features/Employees/GetAllEmployeesTool.cs` - Typed employees tool
- `tools/McpStdioClient/` - Test client console app
- `tools/McpStdioClient/request-*.json` - Sample JSON-RPC requests

## What's next

You can now:
- Add more typed tools in `CQRSPattern.McpServer/Features/`
- Modify request JSON files to test different parameters
- Use Copilot Chat to interact with your API via MCP tools
- Add unit tests for the tools (using xUnit)

## Environment variables

Set these in `.vscode/mcp.json` or your shell:

- `CQRS_API_URL` - Base URL of your API (default: `http://localhost:5001/`)
- `CQRS_API_KEY` - Optional bearer token for API authentication

For manual runs with the test client:

```powershell
$env:CQRS_API_URL = "http://localhost:5001/"
dotnet run --project tools\McpStdioClient -- "CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll" "tools\McpStdioClient\request-get-all-employees.json"
```
