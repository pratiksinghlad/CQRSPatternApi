# MCP Server Integration Guide

Complete step-by-step guide for calling the CQRS Pattern MCP Server from **Postman** and **Claude**.

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Starting the Server](#starting-the-server)
3. [Using with Postman](#using-with-postman)
4. [Using with Claude](#using-with-claude)
5. [Available Tools](#available-tools)
6. [Troubleshooting](#troubleshooting)

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                     Your System                                 │
│                                                                 │
│  ┌──────────────┐    ┌──────────────────┐    ┌───────────────┐ │
│  │  Postman     │    │  Claude          │    │  VS Code      │ │
│  │  (HTTP)      │    │  (via mcp.json)  │    │  (optional)   │ │
│  └──────┬───────┘    └────────┬─────────┘    └───────┬───────┘ │
│         │                     │                      │         │
│         └─────────────────────┼──────────────────────┘         │
│                               │                               │
│         ┌─────────────────────▼──────────────────────┐        │
│         │  CQRSPattern.Api (localhost:5000)         │        │
│         │                                           │        │
│         │  ┌──────────────────────────────────────┐ │        │
│         │  │  MCP Server @ /mcp                   │ │        │
│         │  │  • POST /mcp (JSON-RPC)              │ │        │
│         │  │  • GET  /mcp (SSE stream)            │ │        │
│         │  └──────────────────┬───────────────────┘ │        │
│         │                     │                    │        │
│         │  ┌──────────────────▼───────────────────┐ │        │
│         │  │  MCP Tools (EmployeeTools.cs)       │ │        │
│         │  │  • get_all_employees                │ │        │
│         │  │  • add_employee                     │ │        │
│         │  │  • update_employee                  │ │        │
│         │  │  • patch_employee                   │ │        │
│         │  └──────────────────┬───────────────────┘ │        │
│         │                     │                    │        │
│         │  ┌──────────────────▼───────────────────┐ │        │
│         │  │  CQRS Commands & Queries            │ │        │
│         │  │  (Mediator + Business Logic)        │ │        │
│         │  └────────────────────────────────────┘ │        │
│         │                                          │        │
│         └──────────────────────────────────────────┘        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Three Transport Options Available:**
1. **HTTP Direct** (`localhost:5000/mcp`) — Postman, web clients
2. **HTTP Proxy** (`localhost:5002/mcp`) — External MCP proxy server
3. **stdio** — Claude, CLI tools (via `CQRSPatternMcpServer`)

---

## Starting the Server

### Option 1: Run Main API (HTTP Direct Transport)

```bash
cd CQRSPattern.Api
dotnet run
```

**Output log should show:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: CQRSPattern.Api.Startup[0]
      MCP Server initialized with HTTP transport
```

**Endpoint:** `http://localhost:5000/mcp`

### Option 2: Run MCP Server with stdio Transport (for Claude)

```bash
cd CQRSPatternMcpServer
dotnet run
```

**Output log should show:**
```
info: CQRSPattern.McpServer.Program[0]
      MCP Server started with stdio transport
```

The server will respond to stdin/stdout — suitable for Claude integration.

---

## Using with Postman

### Step 1: Download & Install Postman

1. Visit [postman.com](https://www.postman.com/downloads/)
2. Install and launch Postman

### Step 2: Discover MCP Tools (Initialize)

Create a **new HTTP request** in Postman:

**Method:** `POST`  
**URL:** `http://localhost:5000/mcp`  
**Content-Type:** `application/json`

**Body:**
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "initialize",
  "params": {
    "protocolVersion": "2024-11-05",
    "capabilities": {
      "experimental": {}
    },
    "clientInfo": {
      "name": "Postman",
      "version": "12.0.0"
    }
  }
}
```

**Click Send** — You should receive a response listing all available tools:

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "protocolVersion": "2024-11-05",
    "capabilities": {
      "logging": {},
      "tools": {}
    },
    "serverInfo": {
      "name": "CQRS Pattern MCP Server",
      "version": "1.0.0"
    }
  }
}
```

### Step 3: List Available Tools

Create another request:

**Method:** `POST`  
**URL:** `http://localhost:5000/mcp`

**Body:**
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/list",
  "params": {}
}
```

**Response will show:**
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
          "properties": {}
        }
      },
      {
        "name": "add_employee",
        "description": "Add a new employee to the database...",
        "inputSchema": {
          "type": "object",
          "properties": {
            "firstName": {"type": "string"},
            "lastName": {"type": "string"},
            "gender": {"type": "string"},
            "birthDate": {"type": "string", "format": "date-time"},
            "hireDate": {"type": "string", "format": "date-time"}
          },
          "required": ["firstName", "lastName", "gender", "birthDate", "hireDate"]
        }
      },
      {
        "name": "update_employee",
        "description": "Update an existing employee's full record...",
        "inputSchema": { ... }
      },
      {
        "name": "patch_employee",
        "description": "Partially update an employee...",
        "inputSchema": { ... }
      }
    ]
  }
}
```

### Step 4: Call an MCP Tool

#### Example 4a: Get All Employees

**Method:** `POST`  
**URL:** `http://localhost:5000/mcp`

**Body:**
```json
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

**Expected Response:**
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

#### Example 4b: Add a New Employee

**Method:** `POST`  
**URL:** `http://localhost:5000/mcp`

**Body:**
```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "method": "tools/call",
  "params": {
    "name": "add_employee",
    "arguments": {
      "firstName": "Jane",
      "lastName": "Smith",
      "gender": "Female",
      "birthDate": "1992-03-20",
      "hireDate": "2021-07-15"
    }
  }
}
```

**Expected Response:**
```json
{
  "jsonrpc": "2.0",
  "id": 4,
  "result": {
    "content": [
      {
        "type": "text",
        "text": "{\"success\":true,\"message\":\"Employee created successfully\",\"employee\":{\"firstName\":\"Jane\",\"lastName\":\"Smith\",\"gender\":\"Female\",\"birthDate\":\"1992-03-20\",\"hireDate\":\"2021-07-15\"}}"
      }
    ]
  }
}
```

#### Example 4c: Update an Employee

**Method:** `POST`  
**URL:** `http://localhost:5000/mcp`

**Body:**
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "method": "tools/call",
  "params": {
    "name": "update_employee",
    "arguments": {
      "id": 1,
      "firstName": "Jonathan",
      "lastName": "Doe",
      "gender": "Male",
      "birthDate": "1990-01-15",
      "hireDate": "2020-06-01"
    }
  }
}
```

#### Example 4d: Partially Update (Patch) an Employee

**Method:** `POST`  
**URL:** `http://localhost:5000/mcp`

**Body:**
```json
{
  "jsonrpc": "2.0",
  "id": 6,
  "method": "tools/call",
  "params": {
    "name": "patch_employee",
    "arguments": {
      "id": 1,
      "firstName": "Jonathan"
    }
  }
}
```

⚡ **Note:** Only `id` and `firstName` are sent. Other fields remain unchanged.

---

## Using with Claude

### Step 1: Configure mcp.json

The `mcp.json` file in your project root already contains the configuration:

```jsonc
{
  "mcpServers": {
    "cqrspattern-http-direct": {
      "type": "http",
      "url": "http://localhost:5000/mcp/request",
      "description": "CQRS Pattern API MCP Endpoint - Direct HTTP Transport",
      "headers": {
        "Content-Type": "application/json"
      },
      "timeout": 30000,
      "retryAttempts": 3
    },
    "cqrspattern-stdio": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "CQRSPatternMcpServer/CQRSPattern.McpServer.csproj"
      ],
      "description": "CQRS Pattern MCP Server - stdio Transport",
      "env": {
        "CQRS_API_URL": "http://localhost:5000",
        "CQRS_API_KEY": ""
      }
    }
  },
  "defaultServer": "cqrspattern-http-direct",
  "settings": {
    "apiUrl": "http://localhost:5000",
    "enableLogging": true,
    "logLevel": "Information"
  }
}
```

### Step 2: Option A - Use HTTP Transport (Easiest)

**Prerequisites:**
- Run the main API server: `dotnet run` in `CQRSPattern.Api/`
- Ensure `mcp.json` exists in your workspace root

**In Claude (Web or Desktop):**

1. **Open Settings** → **Developer Settings** or **MCP Config**
2. **Add MCP Server**
3. **Select Transport:** HTTP
4. **Server URL:** `http://localhost:5000/mcp/request`
5. **Headers:**
   ```
   Content-Type: application/json
   ```
6. **Test Connection** ✓

Claude will auto-discover the tools and you can use them directly in conversation.

**Example conversation:**

> **You:** "Using the MCP server, get all employees and tell me the count."
>
> **Claude:** *[Calls get_all_employees tool]* "I found 5 employees in the database..."

### Step 3: Option B - Use stdio Transport (For CLI/IDE Integration)

**Prerequisites:**
- No need to run the main API separately
- Run: `dotnet run --project CQRSPatternMcpServer/` 

**In mcp.json:**

```jsonc
{
  "mcpServers": {
    "cqrspattern-stdio": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "CQRSPatternMcpServer/CQRSPattern.McpServer.csproj"
      ]
    }
  }
}
```

**In Claude Settings:**

1. **MCP Configuration**
2. **Add MCP Server**
3. **Transport:** stdio
4. **Command:** `dotnet`
5. **Arguments:**
   ```
   run
   --project
   CQRSPatternMcpServer/CQRSPattern.McpServer.csproj
   ```
6. **Test Connection** ✓

---

## Available Tools

### 1. `get_all_employees`

**Description:** Retrieve all employees from the database.

**Parameters:** _(none required)_

**Example:**
```json
{
  "method": "tools/call",
  "params": {
    "name": "get_all_employees",
    "arguments": {}
  }
}
```

---

### 2. `add_employee`

**Description:** Add a new employee to the database.

**Parameters:**
| Parameter | Type | Required | Format |
|-----------|------|----------|--------|
| `firstName` | string | ✓ | Any |
| `lastName` | string | ✓ | Any |
| `gender` | string | ✓ | "Male", "Female", etc. |
| `birthDate` | string | ✓ | ISO 8601 (e.g., `1990-01-15`) |
| `hireDate` | string | ✓ | ISO 8601 (e.g., `2020-06-01`) |

**Example:**
```json
{
  "method": "tools/call",
  "params": {
    "name": "add_employee",
    "arguments": {
      "firstName": "Alice",
      "lastName": "Johnson",
      "gender": "Female",
      "birthDate": "1995-05-10",
      "hireDate": "2023-01-15"
    }
  }
}
```

---

### 3. `update_employee`

**Description:** Fully update an employee's record (all fields required).

**Parameters:**
| Parameter | Type | Required | Format |
|-----------|------|----------|--------|
| `id` | integer | ✓ | Employee ID |
| `firstName` | string | ✓ | Any |
| `lastName` | string | ✓ | Any |
| `gender` | string | ✓ | "Male", "Female", etc. |
| `birthDate` | string | ✓ | ISO 8601 |
| `hireDate` | string | ✓ | ISO 8601 |

**Example:**
```json
{
  "method": "tools/call",
  "params": {
    "name": "update_employee",
    "arguments": {
      "id": 1,
      "firstName": "Alice",
      "lastName": "Williams",
      "gender": "Female",
      "birthDate": "1995-05-10",
      "hireDate": "2023-01-15"
    }
  }
}
```

---

### 4. `patch_employee`

**Description:** Partially update an employee (only fields provided are updated).

**Parameters:**
| Parameter | Type | Required | Format |
|-----------|------|----------|--------|
| `id` | integer | ✓ | Employee ID |
| `firstName` | string | ✗ | Any |
| `lastName` | string | ✗ | Any |
| `gender` | string | ✗ | "Male", "Female", etc. |
| `birthDate` | string | ✗ | ISO 8601 |
| `hireDate` | string | ✗ | ISO 8601 |

**Example (Update only first name):**
```json
{
  "method": "tools/call",
  "params": {
    "name": "patch_employee",
    "arguments": {
      "id": 1,
      "firstName": "Alicia"
    }
  }
}
```

---

## Troubleshooting

### Issue: "Connection refused" when calling `localhost:5000`

**Solution:**
- Ensure the main API is running: `dotnet run` in `CQRSPattern.Api/`
- Check port 5000 is not in use: `netstat -ano | findstr :5000`
- Verify no firewall is blocking localhost connections

### Issue: MCP tools not discoverable in Postman/Claude

**Solution:**
1. Call `initialize` first to establish protocol conversation
2. Verify `mcp.json` is in your workspace root
3. Check application logs for errors: Look for "MCP" in console output
4. Ensure `[McpServerToolType]` and `[McpServerTool]` attributes are on the tool class/methods

### Issue: Tool call returns 500 error

**Solution:**
1. Check the application logs for the full error
2. Verify all required parameters are provided
3. For date parameters, ensure ISO 8601 format: `YYYY-MM-DD`
4. Check database connection string in `appsettings.json`

### Issue: "At least one field must be provided for partial update"

**Solution:**
When using `patch_employee`, provide at least one optional field in addition to the `id`:
```json
{
  "id": 1,
  "firstName": "NewName"  // ← Required for patch to work
}
```

### Issue: Date parsing errors in responses

**Solution:**
Ensure dates are in ISO 8601 format:
- ✓ Correct: `1990-01-15`, `2020-06-01`
- ✗ Wrong: `01/15/1990`, `6/1/2020`

---

## Quick Reference

### Postman Workflow

```
1. POST initialize → Get protocol version
2. POST tools/list → Discover available tools
3. POST tools/call (get_all_employees) → Retrieve data
4. POST tools/call (add_employee) → Create record
5. POST tools/call (patch_employee) → Update record
```

### Claude Integration Workflow

```
1. Configure mcp.json with HTTP or stdio transport
2. Add server to Claude settings
3. Test connection
4. Chat naturally: "Get all employees and add a new one named..."
5. Claude automatically calls the right tools
```

---

## Advanced: Custom Requests with curl

```bash
# Initialize
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 1,
    "method": "initialize",
    "params": {
      "protocolVersion": "2024-11-05",
      "capabilities": {},
      "clientInfo": {"name": "curl", "version": "1.0"}
    }
  }'

# Get all employees
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 2,
    "method": "tools/call",
    "params": {
      "name": "get_all_employees",
      "arguments": {}
    }
  }'

# Add employee
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 3,
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
  }'
```

---

## Next Steps

- 📖 [MCP Specification](https://modelcontextprotocol.io)
- 🔧 [CQRSPattern.Api Documentation](./README.md)
- 🛠️ [MCP Server Details](./README_MCP.md)
- 💾 [Tool Implementation](./CQRSPattern.Api/Features/Mcp/Tools/EmployeeTools.cs)

---

**Last Updated:** March 2026  
**Supports:** .NET 9, C# 13, MCP 2024-11-05
