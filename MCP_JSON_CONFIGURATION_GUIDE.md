# mcp.json Configuration Guide

Complete guide to understanding and configuring `mcp.json` for CQRS Pattern MCP Server.

---

## What is mcp.json?

**mcp.json** is a configuration file that tells Claude (and other MCP clients) how to connect to your MCP servers. It defines:

- **Server endpoints** (URLs, commands)
- **Transport types** (HTTP, stdio, SSE)
- **Connection parameters** (headers, timeouts)
- **Environment variables**

---

## Current Configuration

Your project includes three server configurations:

```jsonc
{
  "mcpServers": {
    "cqrspattern-http-direct": { ... },
    "cqrspattern-http-proxy": { ... },
    "cqrspattern-stdio": { ... }
  },
  "defaultServer": "cqrspattern-http-direct",
  "settings": { ... }
}
```

---

## Server Configuration Details

### 1. HTTP Direct Transport (Recommended for Postman/Web)

```jsonc
"cqrspattern-http-direct": {
  "type": "http",
  "url": "http://localhost:5001/mcp",
  "description": "CQRS Pattern API MCP Endpoint - Direct HTTP Transport",
  "headers": {
    "Content-Type": "application/json"
  },
  "timeout": 30000,      // 30 seconds before timing out
  "retryAttempts": 3     // Automatically retry 3 times on failure
}
```

**When to use:**

- Postman requests
- Web clients
- Claude (Web or Desktop)
- Any HTTP client

**How it works:**

1. Sends HTTP POST requests to `http://localhost:5001/mcp`
2. Receives JSON-RPC responses
3. Supports streaming via Server-Sent Events (SSE)

**Prerequisites:**

- Main API running: `dotnet run` in `CQRSPattern.Api/`
- Server listening on port 5001

---

### 2. HTTP Proxy Transport (For External Proxy Server)

```jsonc
"cqrspattern-http-proxy": {
  "type": "http",
  "url": "http://localhost:5002/mcp",
  "description": "CQRS Pattern MCP Server - HTTP Proxy Transport",
  "headers": {
    "Content-Type": "application/json"
  },
  "timeout": 30000,
  "retryAttempts": 3
}
```

**When to use:**

- Routing through a proxy server
- External MCP server wrapper
- Advanced HTTP routing scenarios

**How it works:**

1. Connects to proxy on localhost:5002
2. Proxy forwards requests to main API
3. Useful for rate limiting, load balancing, etc.

**Prerequisites:**

- Proxy server running on port 5002
- Proxy configured to forward to API on port 5001

---

### 3. Stdio Transport (For Claude Code, CLI)

```jsonc
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
    "CQRS_API_URL": "http://localhost:5001",
    "CQRS_API_KEY": ""
  }
}
```

**When to use:**

- Claude Code integration
- CLI MCP clients
- IDE integrations
- Internal tools

**How it works:**

1. Spawns process: `dotnet run --project CQRSPatternMcpServer/...`
2. Communicates via stdin/stdout
3. Environment variables passed to process
4. Process stays alive during session

**Prerequisites:**

- CQRSPatternMcpServer project exists
- Dotnet SDK installed
- Can run without main API (or with main API on background)

---

## Global Settings

```jsonc
"defaultServer": "cqrspattern-http-direct",
"settings": {
  "apiUrl": "http://localhost:5001",
  "apiUrlHttps": "https://localhost:5001",
  "mcpServerUrl": "http://localhost:5002",
  "enableLogging": true,
  "logLevel": "Information"
}
```

| Setting         | Purpose                               | Example                           |
| --------------- | ------------------------------------- | --------------------------------- |
| `defaultServer` | Which server to use if none specified | `cqrspattern-http-direct`         |
| `apiUrl`        | Main API HTTP endpoint                | `http://localhost:5001`           |
| `apiUrlHttps`   | Main API HTTPS endpoint               | `https://localhost:5001`          |
| `mcpServerUrl`  | MCP Server HTTP endpoint              | `http://localhost:5002`           |
| `enableLogging` | Turn logging on/off                   | `true` or `false`                 |
| `logLevel`      | Logging verbosity                     | `Information`, `Debug`, `Warning` |

---

## Usage Scenarios

### Scenario 1: Local Development with Postman

**Goal:** Test MCP server with Postman while developing REST API

**Configuration:**

```jsonc
{
  "mcpServers": {
    "local-http": {
      "type": "http",
      "url": "http://localhost:5001/mcp",
      "headers": { "Content-Type": "application/json" },
      "timeout": 30000,
    },
  },
  "defaultServer": "local-http",
}
```

**Steps:**

1. Run: `dotnet run` in `CQRSPattern.Api/`
2. Import Postman collection: `CQRS_Pattern_MCP_Postman.postman_collection.json`
3. Make requests to `http://localhost:5001/mcp`

---

### Scenario 2: Claude Integration (Web + Desktop)

**Goal:** Use MCP tools directly in Claude conversations

**Configuration:**

```jsonc
{
  "mcpServers": {
    "cqrs-claude": {
      "type": "http",
      "url": "http://localhost:5001/mcp",
      "headers": { "Content-Type": "application/json" },
      "timeout": 30000,
    },
  },
  "defaultServer": "cqrs-claude",
}
```

**Steps:**

1. Run: `dotnet run` in `CQRSPattern.Api/`
2. Open Claude (web or desktop)
3. Settings → Developer → MCP Config
4. Add server with HTTP type pointing to `http://localhost:5001/mcp`
5. Test connection
6. Use naturally: "Get all employees and..."

---

### Scenario 3: Production Deployment

**Goal:** Deploy MCP server with environment-specific URLs

**Configuration:**

```jsonc
{
  "mcpServers": {
    "prod-api": {
      "type": "http",
      "url": "https://api.mycompany.com/mcp",
      "headers": {
        "Content-Type": "application/json",
        "Authorization": "Bearer ${API_TOKEN}",
      },
      "timeout": 60000,
      "retryAttempts": 5,
    },
  },
  "defaultServer": "prod-api",
  "settings": {
    "apiUrl": "https://api.mycompany.com",
    "enableLogging": false,
    "logLevel": "Warning",
  },
}
```

**Notes:**

- Use HTTPS in production
- Add authentication headers if needed
- Increase timeout for slower networks
- Disable verbose logging in production
- Use `${VAR_NAME}` for environment variable substitution

---

### Scenario 4: Multiple Environments

**Goal:** Use different servers for dev, staging, production

**Configuration:**

```jsonc
{
  "mcpServers": {
    "dev": {
      "type": "http",
      "url": "http://localhost:5001/mcp",
      "description": "Development server",
    },
    "staging": {
      "type": "http",
      "url": "https://staging.mycompany.com/mcp",
      "description": "Staging server",
    },
    "prod": {
      "type": "http",
      "url": "https://api.mycompany.com/mcp",
      "headers": {
        "Authorization": "Bearer ${PROD_API_TOKEN}",
      },
      "description": "Production server",
    },
  },
  "defaultServer": "dev",
}
```

**Usage:**

- Local: Uses `dev` by default
- Staging: Specify in client: select `staging` server
- Production: Specify in client: select `prod` server + set `PROD_API_TOKEN`

---

### Scenario 5: stdio for IDE Integration

**Goal:** Use MCP with VS Code, JetBrains IDE, etc.

**Configuration:**

```jsonc
{
  "mcpServers": {
    "cqrs-local": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "CQRSPatternMcpServer/CQRSPattern.McpServer.csproj",
      ],
      "env": {
        "CQRS_API_URL": "http://localhost:5001",
        "DOTNET_ENVIRONMENT": "Development",
      },
    },
  },
  "defaultServer": "cqrs-local",
}
```

**Usage:**

- IDE integrations (Claude Code, etc.)
- No need to manually start API (stdio server handles it)
- Process managed by IDE
- Logs shown in IDE output

---

## Configuration Properties Reference

### Server Object Properties

```jsonc
{
  "type": "http|stdio", // Transport type
  "url": "https://...", // (HTTP only) Endpoint URL
  "command": "dotnet", // (stdio only) Executable
  "args": ["run", "..."], // (stdio only) Command arguments
  "description": "...", // Human description
  "headers": {
    // (HTTP only) Request headers
    "Authorization": "Bearer token",
  },
  "timeout": 30000, // (HTTP) Timeout in ms
  "retryAttempts": 3, // (HTTP) Retry count
  "env": {
    // (stdio) Environment variables
    "VAR_NAME": "value",
  },
}
```

---

## Environment Variables in mcp.json

You can use environment variable substitution with `${VAR_NAME}` syntax:

```jsonc
{
  "headers": {
    "Authorization": "Bearer ${API_TOKEN}",
  },
  "env": {
    "SECRET_KEY": "${SECRET_KEY}",
    "DATABASE_URL": "${DB_CONNECTION_STRING}",
  },
}
```

**How to set:**

**Windows (cmd):**

```batch
set API_TOKEN=secret123
dotnet run
```

**Windows (PowerShell):**

```powershell
$env:API_TOKEN = "secret123"
dotnet run
```

**macOS/Linux:**

```bash
export API_TOKEN=secret123
dotnet run
```

---

## Validation

### Validate HTTP Connection

```bash
curl -X POST http://localhost:5001/mcp \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}'
```

Should return a JSON response with available tools.

### Validate stdio Connection

```bash
dotnet run --project CQRSPatternMcpServer/CQRSPattern.McpServer.csproj
```

Should start without errors and await stdin input.

---

## Common Issues

### ❌ "Cannot connect to localhost:5001"

**Problem:** HTTP server not running

**Solution:**

```bash
# Terminal 1
cd CQRSPattern.Api
dotnet run

# Terminal 2 (run your client)
# Now use http://localhost:5001/mcp
```

---

### ❌ "stdio command not found"

**Problem:** `dotnet` not in PATH or project path wrong

**Solution:**

- Verify .NET is installed: `dotnet --version`
- Check project path is relative to workspace
- Use absolute paths if relative doesn't work

---

### ❌ "Authorization: Bearer token rejected"

**Problem:** Invalid or missing token

**Solution:**

- Verify token in environment: `echo $API_TOKEN` (bash)
- Check header format: `Bearer token_value` (with space)
- Test without auth first to isolate issue

---

### ❌ "Timeout exceeded"

**Problem:** Server too slow or unresponsive

**Solution:**

- Increase timeout:

```jsonc
"timeout": 60000  // 60 seconds
```

- Check server logs for hanging requests
- Reduce request frequency if rate limiting

---

## Tips & Best Practices

✅ **DO:**

- Use HTTP for web clients (Postman, Claude Web)
- Use stdio for CLI/IDE tools
- Add descriptive names and descriptions
- Set appropriate timeouts for your network
- Use environment variables for secrets
- Test connection before deploying

❌ **DON'T:**

- Hardcode secrets in mcp.json
- Use HTTP (no TLS) in production
- Set very high timeouts (causes poor UX)
- Mix development and production configs in one file
- Commit API keys to git

---

## Advanced: Custom Transport Wrapper

If you need custom logic (caching, rate limiting, auth), create a wrapper:

```
Your Client
    ↓
mcp.json → http://localhost:5002/mcp
    ↓
Your Wrapper Service (rate limit, auth, cache)
    ↓
Main API @ http://localhost:5001/mcp
```

Configure in mcp.json:

```jsonc
{
  "mcpServers": {
    "wrapped": {
      "type": "http",
      "url": "http://localhost:5002/mcp",
    },
  },
}
```

---

## Summary

| Need                  | Configuration       | Start With                     |
| --------------------- | ------------------- | ------------------------------ |
| Local Postman testing | HTTP direct         | `setup-mcp.bat` / `dotnet run` |
| Claude integration    | HTTP direct         | Same as Postman                |
| IDE/CLI tools         | stdio               | `setup-mcp.ps1 start-stdio`    |
| Production            | HTTP + HTTPS + Auth | Production mcp.json            |
| Multiple environments | Multiple servers    | Scenario 4 above               |

---

## Next Steps

1. Copy one of the scenario configurations to your `mcp.json`
2. Update URLs/tokens for your environment
3. Test connection using provided curl/PowerShell examples
4. Integrate with your client (Postman, Claude, etc.)

For more details, see **MCP_INTEGRATION_GUIDE.md**

---

**Last Updated:** March 2026  
**Status:** Production Ready
