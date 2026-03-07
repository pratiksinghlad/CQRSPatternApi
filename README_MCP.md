# MCP Server Integration — CQRS Pattern API

This project integrates the official **[Model Context Protocol (MCP)](https://modelcontextprotocol.io)** server directly into the existing CQRS REST API using the `ModelContextProtocol.AspNetCore` SDK.

**No separate server or port needed** — MCP endpoints run alongside your existing REST controllers on the same host.

---

## Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed
- MySQL running locally (see `appsettings.json` for connection strings)

### Run the API + MCP Server

```bash
# From the solution root
dotnet run --project CQRSPattern.Api

# The API starts on http://localhost:5001
# MCP endpoint is at http://localhost:5001/mcp
# Scalar API docs at http://localhost:5001/scalar
```

> **Note:** Both REST API and MCP server run on the same port (`5001`). No extra setup needed.

### Verify MCP is Running

Open a new terminal and run:

```bash
# Send an MCP initialize request
curl -X POST http://localhost:5001/mcp ^
  -H "Content-Type: application/json" ^
  -d "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2025-03-26\",\"capabilities\":{},\"clientInfo\":{\"name\":\"curl-test\",\"version\":\"1.0.0\"}}}"
```

You should receive a JSON-RPC response with the server's capabilities and protocol version.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     CQRSPattern.Api                             │
│                     (http://localhost:5001)                      │
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

- **POST `/mcp`** — Send JSON-RPC requests (initialize, tools/list, tools/call)
- **GET `/mcp`** — Open SSE stream for server-initiated messages

### 2. SSE (Server-Sent Events)

Built into Streamable HTTP. Clients that connect via `GET /mcp` receive an SSE stream.

### 3. stdio (for CLI tools like Claude Code)

Use the standalone `CQRSPatternMcpServer` project, which wraps the same tools with `WithStdioServerTransport()`.

---

## MCP Session Lifecycle

MCP uses JSON-RPC 2.0 over HTTP. A typical session follows this flow:

```
Client                              Server (http://localhost:5001/mcp)
  │                                      │
  │  1. POST initialize                  │
  │ ──────────────────────────────────▶  │
  │  ◀────────── capabilities + session  │
  │                                      │
  │  2. POST notifications/initialized   │
  │ ──────────────────────────────────▶  │
  │  ◀────────── (accepted, no content)  │
  │                                      │
  │  3. POST tools/list                  │
  │ ──────────────────────────────────▶  │
  │  ◀────────── list of available tools │
  │                                      │
  │  4. POST tools/call                  │
  │ ──────────────────────────────────▶  │
  │  ◀────────── tool result             │
  │                                      │
```

> **Important:** After the `initialize` response, the server returns a `Mcp-Session-Id` header. You **must** include this header in all subsequent requests in the same session.

---

## Connecting from Clients

### 🔵 Postman (Step-by-Step Guide)

Postman has built-in MCP support. Here's how to use it:

#### Option A: Using Postman's MCP Client (Recommended)

1. Open Postman
2. Create a new **MCP Request** (available in recent Postman versions)
3. Set the **MCP Server URL** to:
   ```
   http://localhost:5001/mcp
   ```
4. Postman will auto-discover available tools via the MCP `initialize` → `tools/list` flow
5. Select a tool from the dropdown and fill in parameters

#### Option B: Manual HTTP Requests (Raw JSON-RPC)

You can also test MCP manually using standard HTTP POST requests:

**Step 1: Initialize the session**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Accept: application/json, text/event-stream

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

**Expected response:**

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "protocolVersion": "2025-03-26",
    "capabilities": {
      "tools": { "listChanged": false }
    },
    "serverInfo": {
      "name": "CQRSPattern.Api",
      "version": "1.0.0"
    }
  }
}
```

> **Copy the `Mcp-Session-Id` header** from the response — add it to all subsequent requests as `Mcp-Session-Id: <value>`.

**Step 2: Send initialized notification**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Mcp-Session-Id: <session-id-from-step-1>

{
  "jsonrpc": "2.0",
  "method": "notifications/initialized"
}
```

> This is a notification (no `id` field). The server responds with `202 Accepted`.

**Step 3: List available tools**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Mcp-Session-Id: <session-id-from-step-1>

{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/list",
  "params": {}
}
```

**Expected response:**

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

**Step 4: Call a tool — Get All Employees**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Mcp-Session-Id: <session-id-from-step-1>

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

**Step 5: Call a tool — Add Employee**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Mcp-Session-Id: <session-id-from-step-1>

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

**Step 6: Call a tool — Update Employee**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Mcp-Session-Id: <session-id-from-step-1>

{
  "jsonrpc": "2.0",
  "id": 5,
  "method": "tools/call",
  "params": {
    "name": "update_employee",
    "arguments": {
      "id": 1,
      "firstName": "Jane",
      "lastName": "Smith",
      "gender": "Female",
      "birthDate": "1985-03-20",
      "hireDate": "2019-01-15"
    }
  }
}
```

**Step 7: Call a tool — Patch Employee (partial update)**

```http
POST http://localhost:5001/mcp
Content-Type: application/json
Mcp-Session-Id: <session-id-from-step-1>

{
  "jsonrpc": "2.0",
  "id": 6,
  "method": "tools/call",
  "params": {
    "name": "patch_employee",
    "arguments": {
      "id": 1,
      "lastName": "Johnson"
    }
  }
}
```

---

### 🟢 VS Code (Copilot / Extensions)

Add to your VS Code `settings.json` or `.vscode/mcp.json`:

```json
{
  "mcp": {
    "servers": {
      "cqrs-api": {
        "type": "http",
        "url": "http://localhost:5001/mcp"
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

### 🟠 Claude Desktop / Claude Code

Add to `~/.claude/claude_desktop_config.json` (or Claude Desktop config):

**HTTP mode** (connect to the running API):

```json
{
  "mcpServers": {
    "cqrs-api": {
      "type": "http",
      "url": "http://localhost:5001/mcp"
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

---

### 🟣 Gemini CLI / Other MCP Clients

Any MCP-compliant client can connect. The server supports:

- **Streamable HTTP** at `http://localhost:5001/mcp`
- **SSE** at `GET http://localhost:5001/mcp`
- **stdio** via the `CQRSPatternMcpServer` project

Generic configuration:

```json
{
  "mcpServers": {
    "cqrs-api": {
      "url": "http://localhost:5001/mcp"
    }
  }
}
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

---

## Troubleshooting

| Issue                              | Solution                                                                                                |
| ---------------------------------- | ------------------------------------------------------------------------------------------------------- |
| `Connection refused` on port 5001  | Make sure the API is running: `dotnet run --project CQRSPattern.Api`                                    |
| MCP requests return `404`          | Ensure `endpoints.MapMcp()` is in `Startup.cs` and the URL is `/mcp`                                    |
| `Mcp-Session-Id` missing error     | After `initialize`, copy the `Mcp-Session-Id` response header and include it in all subsequent requests |
| Tools not showing in `tools/list`  | Check that `EmployeeTools.cs` has `[McpServerToolType]` and `[McpServerTool]` attributes                |
| Database errors when calling tools | Ensure MySQL is running and connection strings are set in `appsettings.json`                            |
