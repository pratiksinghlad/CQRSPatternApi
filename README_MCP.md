# 🌐 Model Context Protocol (MCP) — .NET Implementation Guide

This document provides a comprehensive guide to the **Model Context Protocol (MCP)** implementation in this project, covering the current server state, how to build your own .NET MCP server, and how to use the MCP client in C#.

---

## 🚀 Current State: Unified MCP Server

This project implements a **Unified MCP Server** within the `CQRSPattern.Api` project. It is "unified" because it serves both REST and MCP from the same process, sharing the same business logic (CQRS + MediatR).

### Supported Transports

1.  **Streamable HTTP (`/mcp`)**: Recommended for remote clients (Postman, custom web apps).
    - Supports `POST` for JSON-RPC requests.
    - Supports `GET` for SSE (Server-Sent Events) streams.
2.  **stdio (Standard Input/Output)**: Recommended for local LLM integration (Claude Desktop, Claude Code).
    - Configured via the `CQRSPattern.Api` entry point or the standalone `CQRSPattern.McpServer` wrapper.

### Key Features

- **Stateless HTTP**: Optimized for scalability.
- **Auto-Discovery**: Tools are automatically discovered from the assembly using attributes.
- **Shared Logic**: MCP tools call the same MediatR commands as REST controllers.

---

## 🛠️ How to Build an MCP Server in .NET

Building an MCP server in .NET is straightforward using the `ModelContextProtocol.AspNetCore` SDK.

### 1. Install NuGet Packages

```xml
<PackageReference Include="ModelContextProtocol" Version="0.4.0-preview.2" />
<PackageReference Include="ModelContextProtocol.AspNetCore" Version="0.4.0-preview.2" />
```

### 2. Configure Services (`Program.cs` or `Startup.cs`)

Use the `AddMcpServer()` extension to register the protocol handlers.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMcpServer()
        .WithHttpTransport(options => options.Stateless = true) // Enable HTTP
        .WithStdioServerTransport()                             // Enable stdio
        .WithToolsFromAssembly()                                // Auto-scan for tools
        .WithResourcesFromAssembly()
        .WithPromptsFromAssembly();
}
```

### 3. Map Endpoints

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapMcp(); // Registers /mcp endpoint
    });
}
```

### 4. Create Tools

Decorate your static methods or classes with attributes. The SDK will handle the JSON-RPC mapping.

```csharp
[McpServerToolType]
public static class MyTools
{
    [McpServerTool(Name = "get_system_status")]
    [Description("Returns the current status of the system")]
    public static async Task<string> GetStatus(IMediator mediator)
    {
        var status = await mediator.Send(new GetStatusQuery());
        return $"System is {status.State}";
    }
}
```

### 🧠 Why Descriptions Matter for AI

The `[McpServerTool]` name and `[Description]` attributes are the **only information** an LLM (like Claude or GPT) receives to understand your API.

- **Tool Name**: Used by the AI to uniquely identify the action (e.g., `update_employee`).
- **Tool Description**: Acts as the documentation for the AI. If the description is vague, the AI won't know when to call it.
  - _Example_: `[Description("Update an existing employee's full record. All fields are required — this is a full replacement.")]` tells the AI exactly what the tool does and its constraints.
- **Parameter Descriptions**: Tell the AI what data to provide for each argument.
  - _Example_: `[Description("Employee's first name")] string firstName` ensures the AI passes a string and understands its semantic meaning.

Without clear descriptions, the AI may hallucinate parameters or fail to use the tool at the correct moment.

---

## 🤖 MCP Client with C#

If you want to build a custom C# application that _consumes_ an MCP server, you can use the `IMcpClient` interface.

### Example: Simple C# MCP Client

```csharp
using ModelContextProtocol.Client;
using ModelContextProtocol.Transports;

// 1. Setup Transport (e.g., stdio to a server process)
var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Command = "dotnet",
    Arguments = ["run", "--project", "CQRSPattern.Api.csproj"]
});

// 2. Initialize Client
var client = new McpClient(transport);
await client.ConnectAsync();

// 3. Initialize Session
var initializationResult = await client.InitializeAsync(new InitializeParams
{
    ProtocolVersion = "2024-11-05",
    ClientInfo = new ClientInfo { Name = "MyCoolClient", Version = "1.0.0" }
});

// 4. List and Call Tools
var tools = await client.ListToolsAsync();
var result = await client.CallToolAsync("get_all_employees", new Dictionary<string, object>());

Console.WriteLine(result.Content[0].Text);
```

---

## 🔌 Connecting Common Clients

### Postman

Postman 2024+ includes an MCP client.

1.  Target URL: `http://localhost:5001/mcp`
2.  Method: `initialize`
3.  Postman will handle the session headers (`Mcp-Session-Id`) automatically.

### Claude Desktop

Add this to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "cqrs-api": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "D:\\Code\\Github\\CQRSPatternApi\\CQRSPattern.Api\\CQRSPattern.Api.csproj"
      ]
    }
  }
}
```

### VS Code / GitHub Copilot

GitHub Copilot and VS Code MCP extensions allow you to connect AI chat directly to your local codebase tools.

**Configuration:** Add this to your `.vscode/mcp.json` (for project-specific) or your global user settings:

```json
{
  "servers": {
    "cqrs-api": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "D:\\Code\\Github\\CQRSPatternApi\\CQRSPattern.Api\\CQRSPattern.Api.csproj"
      ]
    }
  }
}
```

**Transport Support Policy:**

- **stdio (Primary)**: The standard way for VS Code to communicate with local .NET processes. It launches the project and communicates via stdin/stdout.
- **http (Alternative)**: Supported if the API is already running. You can point the client to `http://localhost:5001/mcp`.

Once configured, you can type `@mcp` or use the relevant tool invocation in Copilot Chat to trigger your CQRS commands.

---

### 🛠️ Built-in Tool: McpStdioClient

The repository includes a custom C# tool located in `tools/McpStdioClient` that demonstrates how to implement a low-level JSON-RPC client for the stdio transport.

**Usage:**

```bash
cd tools/McpStdioClient
dotnet run -- "../../CQRSPattern.Api/bin/Debug/net9.0/CQRSPattern.Api.dll" "./request-list-tools.json"
```

---

### ⚠️ Important Technical Note: Logging

All application logs in the `CQRSPattern.Api` project are diverted to **StdErr** (Standard Error). This is critical because the **StdOut** (Standard Output) stream is reserved for clear JSON-RPC protocol messages. Mixing logs into StdOut would break compatibility with MCP clients like Claude Desktop.

---

## ❓ FAQ & Troubleshooting

**Q: Why do I get a 404 on `/mcp`?**
A: Ensure `endpoints.MapMcp()` is called in your `Startup.cs` and that the server is running on the expected port (default 5001).

**Q: How do I handle authentication?**
A: Since MCP HTTP uses standard ASP.NET Core endpoints, you can use `[Authorize]` attributes or middleware just like REST.

**Q: Can I use both REST and MCP simultaneously?**
A: Yes! This project is designed for exactly that. They share the same DI container and database context.

---

_For more details on the specification, visit [modelcontextprotocol.io](https://modelcontextprotocol.io)._
