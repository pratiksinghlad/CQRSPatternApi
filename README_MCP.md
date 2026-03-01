# MCP Server Integration — CQRS Pattern API

This project integrates the official **[Model Context Protocol (MCP)](https://modelcontextprotocol.io)** server directly into the existing CQRS REST API using the `ModelContextProtocol.AspNetCore` SDK.

**No separate server or port needed** — MCP endpoints run alongside your existing REST controllers.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     CQRSPattern.Api                             │
│                                                                 │
│  ┌──────────────┐    ┌──────────────────┐    ┌───────────────┐  │
│  │ REST API     │    │  MCP Server      │    │  MediatR /    │  │
│  │ Controllers  │───▶│  (SDK Endpoints) │───▶│  CQRS Layer   │  │
│  │              │    │  /mcp            │    │               │  │
│  │  /api/...    │    │                  │    │  Commands &   │  │
│  └──────────────┘    └──────────────────┘    │  Queries      │  │
│                                              └───────────────┘  │
│                                                                 │
│  Transports:                                                    │
│  • Streamable HTTP  (POST /mcp)                                 │
│  • SSE              (GET  /mcp)                                 │
│  • stdio            (via CQRSPatternMcpServer project)          │
└─────────────────────────────────────────────────────────────────┘
```

### How It Works

1. **MCP tools** are defined in `Features/Mcp/Tools/EmployeeTools.cs` using `[McpServerTool]` attributes
2. Each tool **directly calls the same CQRS commands/queries** used by the REST controllers via `IMediatorFactory`
3. The SDK auto-discovers tools at startup via `WithToolsFromAssembly()`
4. `MapMcp()` registers the Streamable HTTP + SSE endpoints at `/mcp`

**Zero logic duplication** — both REST and MCP share the same Application layer handlers.

---

## Available MCP Tools

| Tool Name           | Description                   | Parameters                                                       |
| ------------------- | ----------------------------- | ---------------------------------------------------------------- |
| `get_all_employees` | Retrieve all employees        | _(none)_                                                         |
| `add_employee`      | Add a new employee            | `firstName`, `lastName`, `gender`, `birthDate`, `hireDate`       |
| `update_employee`   | Full update of an employee    | `id`, `firstName`, `lastName`, `gender`, `birthDate`, `hireDate` |
| `patch_employee`    | Partial update of an employee | `id`, + any optional fields                                      |

---

## Transport Support

### 1. Streamable HTTP (recommended for web clients)

The primary transport. Uses standard HTTP POST/GET at `/mcp`.

- **POST `/mcp`** — Send JSON-RPC requests
- **GET `/mcp`** — Open SSE stream for server-initiated messages

### 2. SSE (Server-Sent Events)

Built into Streamable HTTP. Clients that connect via `GET /mcp` receive an SSE stream.

### 3. stdio (for CLI tools like Claude Code)

Use the standalone `CQRSPatternMcpServer` project, which wraps the same tools with `WithStdioServerTransport()`.

---

## Connecting from Clients

### 🔵 Postman

1. Open Postman and create a new **MCP request** (or use the HTTP tab for raw requests)
2. Set the **MCP Server URL** to:
   ```
   http://localhost:5000/mcp
   ```
3. Postman will auto-discover available tools via the MCP `initialize` → `tools/list` flow

**Manual HTTP test** (raw JSON-RPC):

```http
POST http://localhost:5000/mcp
Content-Type: application/json

{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "initialize",
  "params": {
    "protocolVersion": "2025-03-26",
    "capabilities": {},
    "clientInfo": {
      "name": "postman",
      "version": "1.0.0"
    }
  }
}
```

Then list tools:

```http
POST http://localhost:5000/mcp
Content-Type: application/json

{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/list",
  "params": {}
}
```

Then call a tool:

```http
POST http://localhost:5000/mcp
Content-Type: application/json

{
  "jsonrpc": "2.0",
  "id": 3,
  "method": "tools/call",
  "params": {
    "name": "get_all_employees",
    "arguments": {}
  }
}
```

**Add Employee example:**

```http
POST http://localhost:5000/mcp
Content-Type: application/json

{
  "jsonrpc": "2.0",
  "id": 4,
  "method": "tools/call",
  "params": {
    "name": "add_employee",
    "arguments": {
      "firstName": "John",
      "lastName": "Doe",
      "gender": "Male",
      "birthDate": "1990-01-15",
      "hireDate": "2020-06-01"
    }
  }
}
```

---

### 🟢 VS Code (Copilot / Extensions)

Add to your VS Code `settings.json`:

```json
{
  "mcp": {
    "servers": {
      "cqrs-api": {
        "type": "http",
        "url": "http://localhost:5000/mcp"
      }
    }
  }
}
```

Or for **stdio** mode (uses the standalone MCP server project):

```json
{
  "mcp": {
    "servers": {
      "cqrs-api": {
        "type": "stdio",
        "command": "dotnet",
        "args": ["run", "--project", "path/to/CQRSPatternMcpServer"]
      }
    }
  }
}
```

VS Code Copilot will auto-discover the tools and make them available in chat with `@cqrs-api`.

---

### 🟠 Claude Code (CLI)

Add the MCP server to Claude Code's configuration. Create or edit `~/.claude/claude_desktop_config.json`:

**HTTP mode** (connect to the running API):

```json
{
  "mcpServers": {
    "cqrs-api": {
      "type": "http",
      "url": "http://localhost:5000/mcp"
    }
  }
}
```

**stdio mode** (Claude manages the process):

```json
{
  "mcpServers": {
    "cqrs-api": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "D:\\Code\\Github\\CQRSPatternApi\\CQRSPatternMcpServer"
      ]
    }
  }
}
```

After configuration, Claude Code will list the available tools when you use `/tools` or ask it to interact with employees.

---

## Example Responses

### `tools/list` response

```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "tools": [
      {
        "name": "get_all_employees",
        "description": "Retrieve all employees from the database...",
        "inputSchema": {
          "type": "object",
          "properties": {},
          "required": []
        }
      },
      {
        "name": "add_employee",
        "description": "Add a new employee to the database...",
        "inputSchema": {
          "type": "object",
          "properties": {
            "firstName": {
              "type": "string",
              "description": "Employee's first name"
            },
            "lastName": {
              "type": "string",
              "description": "Employee's last name"
            },
            "gender": { "type": "string", "description": "Employee's gender" },
            "birthDate": { "type": "string", "format": "date-time" },
            "hireDate": { "type": "string", "format": "date-time" }
          },
          "required": [
            "firstName",
            "lastName",
            "gender",
            "birthDate",
            "hireDate"
          ]
        }
      }
    ]
  }
}
```

### `tools/call` response (get_all_employees)

```json
{
  "jsonrpc": "2.0",
  "id": 3,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "[{\"id\":1,\"firstName\":\"John\",\"lastName\":\"Doe\",\"gender\":\"Male\",\"birthDate\":\"1990-01-15\",\"hireDate\":\"2020-06-01\"}]"
      }
    ]
  }
}
```

---

## Running Locally

```bash
# Start the API (MCP server runs on the same port)
dotnet run --project CQRSPattern.Api

# MCP endpoint is now available at:
# http://localhost:5000/mcp
```

---

## How to Add New MCP Tools

To expose a new CQRS command/query as an MCP tool:

1. **Create a tool method** in `Features/Mcp/Tools/` (or add to an existing tool class):

   ```csharp
   [McpServerToolType]
   public static class MyNewTools
   {
       [McpServerTool(Name = "my_new_tool")]
       [Description("What this tool does")]
       public static async Task<string> MyNewTool(
           [Description("Parameter description")] string param1,
           IMediatorFactory mediatorFactory,
           CancellationToken cancellationToken)
       {
           var scope = mediatorFactory.CreateScope();
           var result = await scope.SendAsync(MyCommand.Create(param1), cancellationToken);
           return JsonSerializer.Serialize(result);
       }
   }
   ```

2. **That's it!** — The SDK auto-discovers new `[McpServerToolType]` classes at startup. No registration code needed.

---

## Reusing This Pattern in Other Projects

To add MCP to any existing ASP.NET Core + CQRS project:

1. **Add NuGet packages**:

   ```xml
   <PackageReference Include="ModelContextProtocol" />
   <PackageReference Include="ModelContextProtocol.AspNetCore" />
   ```

2. **Register in Startup/Program.cs**:

   ```csharp
   services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
   ```

3. **Map endpoints**:

   ```csharp
   endpoints.MapMcp();
   ```

4. **Create tool classes** with `[McpServerToolType]` that inject your mediator/service layer

**Total integration: ~3 lines of setup + 1 tool class per feature.**

---

## Project Structure

```
CQRSPattern.Api/
├── Features/
│   ├── Employee/          # REST controllers (unchanged)
│   └── Mcp/
│       └── Tools/
│           └── EmployeeTools.cs   # MCP tool definitions
├── Startup.cs             # Calls LoadMcp() + MapMcp()
├── Startup.Mcp.cs         # MCP server registration (partial)
└── Registrations.cs       # Autofac DI (MCP handled by SDK)
```

---

## Key Files

| File                                  | Purpose                                           |
| ------------------------------------- | ------------------------------------------------- |
| `Features/Mcp/Tools/EmployeeTools.cs` | MCP tool definitions — the single source of truth |
| `Startup.Mcp.cs`                      | MCP SDK registration (3 lines)                    |
| `Startup.cs`                          | Calls `LoadMcp()` + `MapMcp()`                    |
| `Directory.Packages.props`            | Centralized NuGet versions                        |
