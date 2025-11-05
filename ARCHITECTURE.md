# MCP Server and API Architecture

## Overview

This document describes the architecture of the CQRS Pattern API with integrated MCP (Model Context Protocol) server support. The system provides two main interfaces for interacting with the CQRS infrastructure:

1. **RESTful HTTP API** - Traditional REST endpoints for CRUD operations
2. **MCP Server** - Standardized protocol interface supporting both HTTP and stdio transports

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                            Client Applications                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐ │
│  │  Web Browser │  │  Mobile App  │  │  VS Code +   │  │ HTTP Client │ │
│  │              │  │              │  │  Copilot     │  │  (curl/etc) │ │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  └──────┬──────┘ │
└─────────┼──────────────────┼──────────────────┼──────────────────┼────────┘
          │                  │                  │                  │
          │ REST/HTTP        │ REST/HTTP        │ MCP Protocol     │ MCP/HTTP
          │                  │                  │ (stdio/HTTP)     │
          ▼                  ▼                  ▼                  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                       CQRS Pattern API (.NET 9)                         │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │                    REST API Endpoints                          │    │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐    │    │
│  │  │  GET /api/   │  │  POST /api/  │  │  PATCH /api/     │    │    │
│  │  │  employees   │  │  employees   │  │  employees/{id}  │    │    │
│  │  └──────┬───────┘  └──────┬───────┘  └────────┬─────────┘    │    │
│  └─────────┼──────────────────┼───────────────────┼──────────────┘    │
│            │                  │                   │                    │
│  ┌─────────▼──────────────────▼───────────────────▼──────────────┐    │
│  │              MCP Server Endpoint (HTTP)                        │    │
│  │                POST /mcp/request                               │    │
│  │  ┌──────────────────────────────────────────────────────┐     │    │
│  │  │  MCP Request Router                                  │     │    │
│  │  │  - employee.getAll                                   │     │    │
│  │  │  - employee.add                                      │     │    │
│  │  │  - employee.update                                   │     │    │
│  │  │  - employee.patch                                    │     │    │
│  │  └──────────────────┬───────────────────────────────────┘     │    │
│  └─────────────────────┼───────────────────────────────────────────┘    │
│                        │                                                │
│  ┌─────────────────────▼───────────────────────────────────────────┐   │
│  │                    MediatR (CQRS Mediator)                      │   │
│  │  ┌─────────────┐              ┌─────────────┐                  │   │
│  │  │  Queries    │              │  Commands   │                  │   │
│  │  │  (Read)     │              │  (Write)    │                  │   │
│  │  └──────┬──────┘              └──────┬──────┘                  │   │
│  └─────────┼──────────────────────────────┼──────────────────────────┘   │
└────────────┼──────────────────────────────┼──────────────────────────────┘
             │                              │
             ▼                              ▼
   ┌─────────────────┐          ┌─────────────────┐
   │   Read Database │          │  Write Database │
   │   (MySQL)       │          │   (MySQL)       │
   │                 │          │                 │
   │  - Optimized    │◄─────────│  - Source of    │
   │    for queries  │  Sync    │    Truth        │
   └─────────────────┘          └─────────────────┘


┌─────────────────────────────────────────────────────────────────────────┐
│              Standalone MCP Server (stdio transport)                    │
│                                                                         │
│  ┌──────────────────────────────────────────────────────────────┐     │
│  │  CQRSPattern.McpServer.exe (stdio)                           │     │
│  │                                                               │     │
│  │  Tools:                                                       │     │
│  │  - query_entities       (GET /api/{entityType})              │     │
│  │  - execute_command      (POST /api/commands/{type})          │     │
│  │  - get_entity_by_id     (GET /api/{entityType}/{id})         │     │
│  │  - api_health_check     (GET /health)                        │     │
│  │                                                               │     │
│  │  Transport: stdio (for IDE integration)                      │     │
│  └────────────────────┬─────────────────────────────────────────┘     │
└───────────────────────┼───────────────────────────────────────────────┘
                        │
                        │ HTTP calls to API
                        │ (configured via mcp.json)
                        ▼
              CQRS Pattern API (port 5000/5001)
```

## Component Description

### 1. Client Layer

Multiple types of clients can interact with the system:

- **Web Browsers**: Use REST API endpoints for traditional web applications
- **Mobile Apps**: Use REST API endpoints via HTTP
- **VS Code + Copilot**: Use MCP Server with stdio transport for AI-powered interactions
- **HTTP Clients**: Can use either REST endpoints or MCP HTTP endpoint

### 2. CQRS Pattern API

The main .NET 9 API application providing:

#### REST API Endpoints
- `GET /api/employees` - Retrieve all employees
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `PATCH /api/employees/{id}` - Partial update
- And more endpoints for other entities

#### MCP HTTP Endpoint
- `POST /mcp/request` - Unified MCP protocol endpoint
  - Accepts method-based requests
  - Routes to appropriate CQRS handlers
  - Returns standardized responses

#### MCP Request Router
Handles MCP method routing:
- `employee.getAll` → GetAllEmployeesQuery
- `employee.add` → AddEmployeeCommand
- `employee.update` → UpdateEmployeeCommand
- `employee.patch` → PatchEmployeeCommand

### 3. MediatR (CQRS Mediator)

Implements the CQRS pattern:

- **Queries (Read Operations)**
  - Route to read database
  - Optimized for fast data retrieval
  - No side effects

- **Commands (Write Operations)**
  - Route to write database
  - Maintain data integrity
  - Can trigger events

### 4. Database Layer

**Separate databases for read and write**:

- **Write Database**: Source of truth for all data
- **Read Database**: Optimized for queries, synchronized from write database

### 5. Standalone MCP Server

A separate .NET application (`CQRSPattern.McpServer`) that:

- Runs as a stdio server for IDE integration
- Acts as a proxy to the main API
- Provides MCP tools for common operations
- Configured via `mcp.json`

## Communication Flows

### Flow 1: REST API Request
```
Client → HTTP Request → API Controller → MediatR → Handler → Database → Response
```

### Flow 2: Direct MCP HTTP Request
```
Client → MCP Request (HTTP) → API MCP Endpoint → MCP Router → MediatR → Handler → Database → MCP Response
```

### Flow 3: MCP HTTP Proxy Request
```
Client → MCP Request (HTTP) → MCP Server (port 5002) → API MCP Endpoint → MCP Router → MediatR → Handler → Database → Response → MCP Server → Client
```

### Flow 4: MCP stdio Request (from IDE)
```
IDE → stdio → MCP Server → HTTP Request → API → MediatR → Handler → Database → Response → MCP Server → stdio → IDE
```

## Configuration

### mcp.json Structure

The `mcp.json` file configures MCP server connections:

```json
{
  "mcpServers": {
    "cqrspattern-http-direct": {
      "type": "http",
      "url": "http://localhost:5000/mcp/request"
    },
    "cqrspattern-http-proxy": {
      "type": "http",
      "url": "http://localhost:5002/mcp/request"
    },
    "cqrspattern-stdio": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "CQRSPatternMcpServer/..."],
      "env": {
        "CQRS_API_URL": "http://localhost:5000"
      }
    }
  }
}
```

## Benefits of This Architecture

1. **Multiple Access Modes**: Supports REST, direct MCP HTTP, MCP HTTP proxy, and MCP stdio
2. **Flexibility**: Clients can choose the most appropriate interface for their use case
3. **Separation of Concerns**: Clear boundaries between read/write operations
4. **Scalability**: Read and write databases can scale independently
5. **AI Integration**: MCP support enables AI assistants to interact with the API
6. **Development vs Production**: HTTP proxy mode for development, direct mode for production
7. **Standardization**: MCP provides consistent error handling and request/response format
8. **Monitoring**: HTTP proxy mode allows additional logging and request/response inspection

## Security Considerations

- API keys can be configured via environment variables
- HTTPS support for production deployments
- Request validation at multiple layers
- Separation of read/write operations limits potential damage

## Performance Optimization

- Read database optimized for query performance
- Write database ensures data integrity
- Caching strategies for frequently accessed data
- Async/await pattern throughout for scalability
