# CQRS API & MCP Server — Running Guide

This project is configured as a **Unified API Server**. Every time you run the application (F5 or `dotnet run`), it starts **three protocols simultaneously on the same host**:

1.  **RESTful API** — Standard CRUD endpoints (e.g., `/api/employees`).
2.  **MCP (HTTP)** — Remote Model Context Protocol at `https://localhost:5001/mcp`.
3.  **MCP (STDIO)** — Local Model Context Protocol via stdin/stdout (JSON-RPC).

---

## 1. How to Run locally (Visual Studio / F5)

Just click the **Play (CQRSPattern.Api)** button in Visual Studio or press **F5**.

- **REST UI (Scalar)**: Will open automatically at [https://localhost:5001/scalar](https://localhost:5001/scalar).
- **MCP HTTP Handshake**: Verify by sending a POST to `https://localhost:5001/mcp` using your Postman collection.
- **MCP Stdio**: The process will also be listening for JSON-RPC messages on its standard input.

---

## 2. Connecting Claude Desktop (Local AI)

To use this API as a "Brain" for Claude Desktop, add the following to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "cqrs-all-in-one": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "d:/Code/Github/CQRSPatternApi/CQRSPattern.Api/CQRSPattern.Api.csproj"
      ]
    }
  }
}
```

_Note: Use absolute paths for the `--project` argument._

---

## 3. Connecting VS Code / GitHub Copilot

Copy the following into your `.vscode/mcp.json`:

```json
{
  "servers": {
    "cqrs-api-stdio": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "d:/Code/Github/CQRSPatternApi/CQRSPattern.Api/CQRSPattern.Api.csproj"
      ]
    },
    "cqrs-api-http": {
      "type": "http",
      "url": "https://localhost:5001/mcp"
    }
  }
}
```

---

## 4. Testing with Postman (HTTP)

- **Handshake**: `POST https://localhost:5001/mcp`
- **Header**: `Accept: application/json, text/event-stream`
- **Body**:
  ```json
  {
    "jsonrpc": "2.0",
    "id": 1,
    "method": "initialize",
    "params": {
      "protocolVersion": "2025-03-26",
      "clientInfo": { "name": "postman", "version": "1.0" },
      "capabilities": {}
    }
  }
  ```
- **Subsequent Calls**: Always include the `Mcp-Session-Id` header received in the initialize response.

---

## Important Technical Note: Logging

All application logs are diverted to **StdErr** (Standard Error). This is intentional to ensure that the **StdOut** (Standard Output) stream remains dedicated to pure JSON-RPC messages, which is required for MCP clients like Claude Desktop to function correctly.
