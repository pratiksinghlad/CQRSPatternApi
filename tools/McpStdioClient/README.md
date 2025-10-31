# MCP Stdio Test Client

Simple console app that calls an MCP server DLL over stdio using JSON-RPC with Content-Length framing.

## Usage

Build the client:
```powershell
dotnet build tools\McpStdioClient
```

Run a test request:
```powershell
dotnet run --project tools\McpStdioClient -- "CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll" "tools\McpStdioClient\request-get-all-employees.json"
```

Or query entities:
```powershell
dotnet run --project tools\McpStdioClient -- "CQRSPattern.McpServer\bin\Debug\net9.0\CQRSPattern.McpServer.dll" "tools\McpStdioClient\request-query-entities.json"
```

## Prerequisites

- The CQRS API must be running (e.g., `dotnet run --project CQRSPattern.Api`)
- The MCP server DLL must be built (`dotnet build CQRSPattern.McpServer`)
- Set CQRS_API_URL environment variable if API is not on http://localhost:5000

## Request files

- `request-get-all-employees.json` - calls the typed get_all_employees tool
- `request-query-entities.json` - calls the generic query_entities tool

Edit these files to test different tool calls and parameters.
