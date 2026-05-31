# CQRS Pattern with Unified MCP Server in .NET 10

![.Net](https://img.shields.io/badge/-.NET%2010.0-blueviolet?logo=dotnet)
![Mysql](https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=white)
![EF](https://img.shields.io/badge/-Entity_Framework-8C3D65?logo=dotnet&logoColor=white)
![Openapi](https://img.shields.io/badge/Docs-OpenAPI%2010.0-success?style=flat-square)
![XUnit](https://img.shields.io/badge/Tests-xUnit-blue)
![MCP](https://img.shields.io/badge/MCP-Unified_Server-success)
![Governance](https://img.shields.io/badge/MCP-Agent_Governance-critical)

This repository demonstrates the **CQRS (Command Query Responsibility Segregation)** pattern using **.NET 10**, featuring a **Unified API** that serves both **RESTful** endpoints and the **Model Context Protocol (MCP)** via a single application host, secured by the **Agent Governance Toolkit**.

## 🏗️ Architecture

One process (`dotnet run`) starts everything:

- **REST API** — standard HTTP controllers for web and mobile clients.
- **MCP HTTP Server** — standardised protocol for AI integration (Postman, Claude, Gemini).
- **MCP Stdio Server** — direct pipe for local IDE/LLM integration (Claude Desktop, etc.).

All protocols share the same **MediatR** core — zero logic duplication across access methods.

![cqrs_pattern](./Screenshots/CQRS_Pattern.jpg)

## 📋 Table of Contents

- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Security & Governance](#-security--governance)
- [MCP Integration](#-mcp-integration)
- [REST Endpoints](#-rest-endpoints)
- [Running Tests](#-running-tests)
- [Architecture Details](#-architecture-details)
- [Libraries](#-libraries)

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [MySQL Server](https://www.mysql.com/downloads/) (configure connection strings in `appsettings.json`)

### Quick Launch

```bash
# Clone & restore
git clone https://github.com/pratiksinghlad/CQRSPatternApi.git
cd CQRSPatternApi
dotnet restore

# Run the unified server
dotnet run --project CQRSPattern.Api
```

| Endpoint | URL |
|---|---|
| REST API | `http://localhost:5001/api/employees` |
| MCP | `http://localhost:5001/mcp` |
| API Docs (Scalar) | `http://localhost:5001/scalar` |
| Health (ready) | `http://localhost:5001/health/ready` |

## 🛠️ Project Structure

| Project | Role |
|---|---|
| `CQRSPattern.Api` | Unified host — REST + MCP HTTP/Stdio + Governance |
| `CQRSPattern.Application` | Commands, queries, MediatR handlers |
| `CQRSPattern.Infrastructure.Persistence` | EF Core with dual read/write database |
| `CQRSPattern.Infrastructure.Mediator` | MediatR pipeline wiring |
| `CQRSPattern.McpServer` | Standalone Stdio wrapper for CLI use cases |

## 🔒 Security & Governance

The MCP server is secured by **[Microsoft.AgentGovernance.Extensions.ModelContextProtocol](https://devblogs.microsoft.com/dotnet/announcing-agent-governance-toolkit-mcp-extensions-for-dotnet/)** (Public Preview), added via a single `.WithGovernance(...)` call on `IMcpServerBuilder`.

### What it adds

| Control | Behaviour |
|---|---|
| **Startup tool scanning** | Scans registered tools before exposure; fails startup if unsafe definitions are found (tool poisoning, hidden instructions, schema abuse, etc.) |
| **Runtime policy enforcement** | Each tool call is evaluated against `policies/mcp.yaml`; denied calls never execute |
| **Response sanitisation** | Strips prompt-injection tags, credential patterns, and exfiltration URLs from tool output before it reaches the model |
| **Audit & Metrics** | Structured audit log and metrics for every governed tool call |

All protections are **on by default** — `ScanToolsOnStartup`, `FailOnUnsafeTools`, `SanitizeResponses`, `EnableAudit`, `EnableMetrics`.

### Policy file

`CQRSPattern.Api/policies/mcp.yaml` uses a **deny-by-default** posture. Every registered tool has an explicit `allow` rule; any tool not listed is automatically blocked.

```yaml
default_action: deny

rules:
  - name: allow-get-all-employees
    condition: "tool_name == 'get_all_employees'"
    action: allow
    priority: 10
  # ... one rule per tool
```

To add a new MCP tool: register it in C# **and** add its allow rule to the policy file. This prevents accidental exposure of new tools.

### Configuration

Override governance settings per environment in `appsettings.{Environment}.json`:

```json
"McpGovernance": {
  "PolicyPath": "policies/mcp.yaml",
  "DefaultAgentId": "did:mcp:anonymous",
  "ServerName": "cqrs-api"
}
```

> **Compliance note**: The Agent Governance Toolkit provides technical controls that support security programmes. It does not by itself guarantee legal or regulatory compliance. Validate your end-to-end implementation against applicable requirements (GDPR, SOC 2, internal policies, etc.).

## 🌍 MCP Integration

See **[README_MCP.md](README_MCP.md)** for detailed instructions on:

- Configuring MCP clients (Postman, Claude, VS Code).
- Building your own .NET MCP Server.
- Using the C# MCP Client.

## 📡 REST Endpoints

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/employees` | Get all employees |
| `POST` | `/api/employees` | Create employee |
| `PUT` | `/api/employees/{id}` | Full update |
| `PATCH` | `/api/employees/{id}` | Partial update |
| `PATCH` | `/api/employees/{id}/jsonpatch` | JSON Patch operations |

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

Tests cover application logic (xUnit), persistence (xUnit + EF InMemory), and architecture rules (NetArchTest).

## 📘 Architecture Details

- **[ARCHITECTURE.md](ARCHITECTURE.md)** — Diagrams and patterns.
- **[DDD_ANALYSIS.md](DDD_ANALYSIS.md)** — Domain-driven design breakdown.
- **[MCP_IMPLEMENTATION_SUMMARY.md](MCP_IMPLEMENTATION_SUMMARY.md)** — Technical details on the MCP bridge.

## 📚 Libraries

| Library | Purpose |
|---|---|
| MediatR | Mediator pattern — decouples commands/queries from handlers |
| Entity Framework Core | ORM with dual read/write database support |
| FluentValidation | Request validation in the CQRS pipeline |
| Scalar | Modern API documentation (replaces Swagger UI) |
| Serilog | Structured logging |
| AutoMapper | Object-to-object mapping |
| Microsoft.AgentGovernance.Extensions.ModelContextProtocol | MCP server governance (startup scanning, policy enforcement, response sanitisation) |

---

_Built with ❤️ using .NET 10, CQRS, and Agent Governance Toolkit._
