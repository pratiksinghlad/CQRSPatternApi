# CQRS Pattern with MCP Server in .NET 9
![.Net](https://img.shields.io/badge/-.NET%209.0-blueviolet?logo=dotnet) 
![Mysql](https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=white)
![EF](https://img.shields.io/badge/-Entity_Framework-8C3D65?logo=dotnet&logoColor=white)
![Openapi](https://img.shields.io/badge/Docs-OpenAPI%209.0-success?style=flat-square)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?logo=swagger&logoColor=white)
![XUnit](https://img.shields.io/nuget/v/xunit?label=xunit)
![MSTest](https://img.shields.io/nuget/v/Microsoft.NET.Test.Sdk?label=Microsoft.NET.Test.Sdk)
![Rest](https://img.shields.io/badge/rest-40AEF0?logo=rest&logoColor=white)
![HTTP3](https://img.shields.io/badge/HTTP%203-v3.0-brightgreen)
![HTTP2](https://img.shields.io/badge/HTTP%202-v2.0-blue)
![HTTP1](https://img.shields.io/badge/HTTP%201-v1.1-orange)
![NetArchTest](https://img.shields.io/badge/NetArchTest-1.3.2-blue)
![MCP](https://img.shields.io/badge/MCP-Server-success)

Command Query Responsibility Segregation (CQRS) is a software architectural pattern that separates the operations of reading data (queries) from modifying data (commands). This distinction allows for independent optimization of each operation, enhancing performance, scalability, and security in applications.

This repository demonstrates the **CQRS (Command Query Responsibility Segregation)** pattern using **.NET 9** with an integrated **MCP (Model Context Protocol) server**. The project incorporates the **MediatR** library to enable flexible and standardized querying of data. It also features **OpenAPI documentation** for seamless exploration and understanding of the API. The application uses **Entity Framework** for data access, with two separate databases: one for write operations and another for read operations.
**HTTP/3/2/1** fallback code supports **Brotli** compression and falls back to **Gzip** for **response compression**.
**XUnit** Unit test for our projects.

## 🚀 What's New: MCP Server

This API now includes an **MCP (Model Context Protocol) server** that provides a standardized interface for processing requests. The MCP endpoint (`POST /mcp/request`) accepts method-based requests and routes them to the appropriate handlers in the CQRS infrastructure.

![cqrs_pattern](./Screenshots/CQRS_Pattern.jpg)

## 📋 Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
  - [Running the Application](#running-the-application)
- [MCP Server](#mcp-server)
  - [What is MCP?](#what-is-mcp)
  - [MCP Endpoint](#mcp-endpoint)
  - [Available Methods](#available-methods)
  - [Request/Response Format](#requestresponse-format)
  - [Example Requests](#example-requests)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Architecture](#architecture)

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL Server](https://www.mysql.com/downloads/) (or use Docker)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/pratiksinghlad/CQRSPatternApi.git
cd CQRSPatternApi
```

2. **Restore dependencies**

```bash
dotnet restore
```

3. **Build the solution**

```bash
dotnet build
```

### Configuration

The application uses multiple configuration files for different environments:

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development environment settings
- `appsettings.Production.json` - Production environment settings
- `appsettings.Local.json` - Local development overrides (optional)
- `secrets/appsettings.secrets.json` - Sensitive data (gitignored)
- **`mcp.json`** - MCP server configuration (NEW)

#### MCP Configuration (mcp.json)

The `mcp.json` file configures how clients connect to the MCP server. It supports both HTTP and stdio transports:

```json
{
  "mcpServers": {
    "cqrspattern-http": {
      "type": "http",
      "url": "http://localhost:5000/mcp/request",
      "description": "CQRS Pattern API MCP Server - HTTP Transport"
    },
    "cqrspattern-stdio": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "CQRSPatternMcpServer/CQRSPattern.McpServer.csproj"],
      "env": {
        "CQRS_API_URL": "http://localhost:5000"
      }
    }
  },
  "settings": {
    "apiUrl": "http://localhost:5000",
    "apiUrlHttps": "https://localhost:5001"
  }
}
```

#### Environment Variables

Key configuration settings:

```json
{
  "ConnectionStrings": {
    "ReadDb": "Server=localhost;Database=cqrs_read;User=root;Password=your_password;",
    "WriteDb": "Server=localhost;Database=cqrs_write;User=root;Password=your_password;"
  }
}
```

You can also use environment variables:
- `ConnectionStrings__ReadDb`
- `ConnectionStrings__WriteDb`
- `CQRS_API_URL` - Base URL for the CQRS API (used by MCP server)
- `CQRS_API_KEY` - Optional API key for authentication

For more configuration conventions, see [copilot-instructions.md](.github/copilot-instructions.md).

### Running the Application

#### Using .NET CLI

```bash
# Run the API
cd CQRSPattern.Api
dotnet run

# The API will be available at:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
```

#### Using Docker

```bash
# Build and run with Docker Compose
docker-compose up --build

# The API will be available at http://localhost:8080
```

#### Running Database Migrations

```bash
# Navigate to the migrator project
cd CQRSPattern.Migrator

# Run migrations
dotnet run
```

#### Running the MCP Server (Standalone)

The standalone MCP server provides stdio-based communication for IDE integration (e.g., VS Code with GitHub Copilot):

```bash
# Build the MCP server first
dotnet build CQRSPatternMcpServer/CQRSPattern.McpServer.csproj

# Run the MCP server (make sure the API is running first)
dotnet run --project CQRSPatternMcpServer/CQRSPattern.McpServer.csproj

# Or run the compiled DLL directly
dotnet CQRSPatternMcpServer/bin/Debug/net9.0/CQRSPattern.McpServer.dll
```

**Note**: The MCP server needs the main API to be running since it acts as a proxy to the API endpoints.

**Configuration**: The MCP server reads configuration from `mcp.json` to determine the API URL and other settings.

## MCP Server

### What is MCP?

The **Model Context Protocol (MCP)** is a standardized protocol for handling structured requests in a language-agnostic way. This implementation provides a unified endpoint that accepts method-based requests and routes them to the appropriate CQRS handlers.

### MCP Endpoint

The MCP server is accessible via a single endpoint:

```
POST /mcp/request
Content-Type: application/json
```

### Available Methods

| Method | Description | Parameters Required |
|--------|-------------|---------------------|
| `employee.getAll` | Retrieves all employees | None |
| `employee.add` | Creates a new employee | firstName, lastName, gender, birthDate, hireDate |
| `employee.update` | Updates an existing employee | id, request (with all employee fields) |
| `employee.patch` | Partially updates an employee | id, request (with optional fields) |

### Request/Response Format

#### Request Structure

```json
{
  "method": "string",      // Required: The method to execute
  "params": { },           // Optional: Method parameters
  "id": "string"           // Optional: Request ID for tracking
}
```

#### Response Structure

**Success Response:**
```json
{
  "success": true,
  "result": { },           // Method result data
  "error": null,
  "id": "string"           // Echoed request ID
}
```

**Error Response:**
```json
{
  "success": false,
  "result": null,
  "error": {
    "code": "ERROR_CODE",
    "message": "Error description",
    "details": { }         // Optional additional error details
  },
  "id": "string"
}
```

### Error Codes

| Code | Description |
|------|-------------|
| `INVALID_REQUEST` | Request body is missing or malformed |
| `INVALID_METHOD` | Method name is missing or empty |
| `METHOD_NOT_FOUND` | Specified method is not supported |
| `INVALID_PARAMS` | Parameters are invalid or incorrectly formatted |
| `VALIDATION_ERROR` | Request validation failed |
| `NOT_FOUND` | Requested resource not found |
| `INTERNAL_ERROR` | Unexpected server error |

### Example Requests

#### 1. Get All Employees

**Request:**
```bash
curl -X POST https://localhost:5001/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.getAll",
    "id": "req-001"
  }'
```

**Response:**
```json
{
  "success": true,
  "result": [
    {
      "id": 1,
      "firstName": "John",
      "lastName": "Doe",
      "gender": "Male",
      "birthDate": "1990-01-01T00:00:00Z",
      "hireDate": "2020-01-01T00:00:00Z"
    }
  ],
  "error": null,
  "id": "req-001"
}
```

#### 2. Add New Employee

**Request:**
```bash
curl -X POST https://localhost:5001/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.add",
    "params": {
      "firstName": "Jane",
      "lastName": "Smith",
      "gender": "Female",
      "birthDate": "1992-05-15T00:00:00Z",
      "hireDate": "2021-03-01T00:00:00Z"
    },
    "id": "req-002"
  }'
```

**Response:**
```json
{
  "success": true,
  "result": {
    "message": "Employee created successfully"
  },
  "error": null,
  "id": "req-002"
}
```

#### 3. Update Employee

**Request:**
```bash
curl -X POST https://localhost:5001/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.update",
    "params": {
      "id": 1,
      "request": {
        "firstName": "John",
        "lastName": "Doe-Smith",
        "gender": "Male",
        "birthDate": "1990-01-01T00:00:00Z",
        "hireDate": "2020-01-01T00:00:00Z"
      }
    },
    "id": "req-003"
  }'
```

**Response:**
```json
{
  "success": true,
  "result": {
    "message": "Employee updated successfully"
  },
  "error": null,
  "id": "req-003"
}
```

#### 4. Partial Update (Patch) Employee

**Request:**
```bash
curl -X POST https://localhost:5001/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.patch",
    "params": {
      "id": 1,
      "request": {
        "firstName": "Johnny"
      }
    },
    "id": "req-004"
  }'
```

**Response:**
```json
{
  "success": true,
  "result": {
    "message": "Employee patched successfully"
  },
  "error": null,
  "id": "req-004"
}
```

#### 5. Error Example - Method Not Found

**Request:**
```bash
curl -X POST https://localhost:5001/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.delete",
    "params": { "id": 1 },
    "id": "req-005"
  }'
```

**Response:**
```json
{
  "success": false,
  "result": null,
  "error": {
    "code": "METHOD_NOT_FOUND",
    "message": "Method 'employee.delete' is not supported. Available methods: employee.getAll, employee.add, employee.update, employee.patch"
  },
  "id": "req-005"
}
```


# Project structure / technology
`Onion Layer principle` Dependencies can only be made 1 way. From outside to the inside.
To communicate from controller to our business logic layer (CQRSPattern.Application)
* .NET 9: Technology
* Entity Framework Core: ORM mapper of Microsoft
* MediatR: Framework written by J. Bogard to decouple code more easily
* NetArchTest: Validates architectural boundaries at runtime and during tests
* MCP Server: Standardized protocol endpoint for handling structured requests

## Key Components

1. **CQRSPattern.Api**: Main API project with controllers and API endpoints
   - Contains employee and weather forecast controllers
   - **MCP Server**: Unified MCP endpoint (`/mcp/request`) for handling method-based requests
   - Implements Server-Sent Events (SSE) for real-time updates
   - Includes architecture validation on startup

2. **CQRSPattern.Application**: Core application logic
   - Defines queries, commands, and models
   - Contains repository interfaces
   - Defines mediator interfaces for CQRS pattern

3. **CQRSPattern.Infrastructure.Mediator**: Implements the mediator pattern
   - Provides factory and scope for mediator operations

4. **CQRSPattern.Infrastructure.Persistence**: Data access layer
   - Separate DbContexts for read and write operations
   - Repository implementations for data access
   - Database migrations

5. **CQRSPattern.Migrator**: Database migration tool
   - Standalone executable for applying migrations

6. **Test Projects**: Various test projects for different layers
   - Application.Test
   - Infrastructure.Persistence.Test
   - Shared.Test (common test utilities)

7. **Directory.Packages.props**: Central package management file for consistent dependency versions across all projects

## Testing

### Running All Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Running Specific Test Projects

```bash
# Application tests
dotnet test CQRSPattern.Application.Test/CQRSPattern.Application.Test/

# Infrastructure tests
dotnet test CQRSPattern.Infrastructure.Persistence.Test/
```

### Testing the MCP Endpoint

You can test the MCP endpoint using:

1. **cURL** (see examples above)
2. **Postman** - Import the OpenAPI spec from `/scalar/v1`
3. **Scalar API Documentation** - Navigate to `/scalar` when the API is running
4. **Integration Tests** - Run the test suite with `dotnet test`

## Architecture

This solution follows Clean Architecture principles with clear separation of concerns.

For a detailed architecture explanation with diagrams, see **[ARCHITECTURE.md](ARCHITECTURE.md)**.

### Key Architectural Components

1. **CQRS Pattern API** - Main .NET 9 API with REST and MCP endpoints
2. **MCP Server** - Standalone stdio server for IDE integration
3. **MediatR** - Command/Query mediator implementation
4. **Dual Database** - Separate read and write databases for optimal performance
5. **Client Flexibility** - Support for REST, MCP HTTP, and MCP stdio protocols

### Quick Architecture Overview

```
Clients (Web/Mobile/IDE) 
    ↓
CQRS API (REST + MCP HTTP Endpoint)
    ↓
MediatR (Commands/Queries)
    ↓
Read DB ← Write DB
```

For complete architecture details including communication flows, see [ARCHITECTURE.md](ARCHITECTURE.md).

## Hosting projects: CQRSPattern.Api
The executing code runs from these projects.

## Additional Resources

- **[Architecture Documentation](ARCHITECTURE.md)** - Detailed architecture with diagrams and flows
- **[Copilot Instructions](.github/copilot-instructions.md)** - Project conventions, coding standards, and setup rules
- **[PATCH Implementation](PATCH_IMPLEMENTATION.md)** - Details on HTTP PATCH support
- **[Swagger PATCH Examples](SWAGGER_PATCH_EXAMPLE.md)** - Examples for using PATCH endpoints
- **[JSON Patch Examples](JsonPatchExample.md)** - JSON Patch operation examples
- **[MCP Server README](CQRSPatternMcpServer/README.md)** - Standalone MCP server documentation

## API Documentation

When the application is running, you can access the interactive API documentation:

- **Scalar UI**: Navigate to `/scalar` for a modern API documentation interface
- **OpenAPI Spec**: Available at `/scalar/v1/openapi.json`

The documentation includes:
- All REST endpoints (Employee, Weather Forecast, SSE)
- MCP endpoint with request/response examples
- Health check endpoints
- Authentication requirements (if configured)

## Health Checks

The API provides health check endpoints for monitoring:

- `GET /health/ready` - Returns 200 OK when the application is ready to serve requests
- `GET /health/live` - Returns 200 OK when the application is alive

## MCP vs REST Endpoints

### When to use MCP:
- Programmatic access with a standardized protocol
- Method-based routing with consistent error handling
- Building automation tools or scripts
- Integrating with other MCP-compatible systems
- AI-powered interactions (e.g., GitHub Copilot)

### When to use REST:
- Traditional HTTP operations (GET, POST, PUT, PATCH, DELETE)
- Browser-based applications
- Following REST conventions
- Standard HTTP semantics

Both approaches access the same underlying CQRS infrastructure, so choose based on your needs.

## Client Communication Guide

### Option 1: Using the HTTP MCP Endpoint

The HTTP MCP endpoint provides a unified interface for all operations using the MCP protocol.

**Endpoint**: `POST http://localhost:5000/mcp/request` or `POST https://localhost:5001/mcp/request`

**Configuration**: Set up in `mcp.json` under `cqrspattern-http`.

#### Example: Get All Employees
```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.getAll",
    "id": "req-001"
  }'
```

#### Example: Add Employee
```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.add",
    "params": {
      "firstName": "Jane",
      "lastName": "Doe",
      "gender": "Female",
      "birthDate": "1990-01-01T00:00:00Z",
      "hireDate": "2021-01-01T00:00:00Z"
    },
    "id": "req-002"
  }'
```

### Option 2: Using the Standalone MCP Server (stdio)

The standalone MCP server is ideal for IDE integration (e.g., VS Code with GitHub Copilot).

**Prerequisites**:
1. Main API must be running
2. MCP server configured in your IDE's MCP client

**Steps**:

1. **Start the API**:
```bash
cd CQRSPattern.Api
dotnet run
```

2. **Start the MCP Server**:
```bash
cd CQRSPatternMcpServer
dotnet run
```

3. **Use from IDE**: The MCP server will be available via stdio transport for AI assistants.

**Configuration**: Set up in `mcp.json` under `cqrspattern-stdio`.

#### Available MCP Tools
- `query_entities` - Query any entity type with pagination
- `execute_command` - Execute CQRS commands
- `get_entity_by_id` - Get specific entity by ID
- `api_health_check` - Check API health status

### Option 3: Using Traditional REST Endpoints

Standard RESTful HTTP endpoints are available for all operations.

#### Employee Endpoints

**Get All Employees**:
```bash
curl -X GET http://localhost:5000/api/employees
```

**Create Employee**:
```bash
curl -X POST http://localhost:5000/api/employees \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "gender": "Male",
    "birthDate": "1990-01-01T00:00:00Z",
    "hireDate": "2020-01-01T00:00:00Z"
  }'
```

**Update Employee**:
```bash
curl -X PUT http://localhost:5000/api/employees/1 \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe-Updated",
    "gender": "Male",
    "birthDate": "1990-01-01T00:00:00Z",
    "hireDate": "2020-01-01T00:00:00Z"
  }'
```

**Partial Update (PATCH)**:
```bash
curl -X PATCH http://localhost:5000/api/employees/1 \
  -H "Content-Type: application/json" \
  -d '{
    "lastName": "Smith"
  }'
```

### Choosing the Right Approach

| Use Case | Recommended Approach | Why |
|----------|---------------------|-----|
| AI Assistant Integration | MCP stdio Server | Native MCP protocol support |
| Web Application | REST Endpoints | Standard HTTP semantics |
| Automation Scripts | MCP HTTP Endpoint | Consistent error handling |
| Mobile App | REST Endpoints | Wide client library support |
| Microservices | MCP HTTP Endpoint | Protocol standardization |

### Authentication

To use API key authentication (if configured):

**REST**:
```bash
curl -X GET http://localhost:5000/api/employees \
  -H "Authorization: Bearer YOUR_API_KEY"
```

**MCP HTTP**:
```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{"method": "employee.getAll"}'
```

**MCP stdio**: Configure `CQRS_API_KEY` in the environment variables section of `mcp.json`.

## Backward Compatibility

All existing REST endpoints remain fully functional:
- `GET /api/employees` - Get all employees
- `POST /api/employees` - Create employee
- `PUT /api/employees/{id}` - Update employee
- `PATCH /api/employees/{id}` - Partial update employee
- `PATCH /api/employees/{id}/jsonpatch` - JSON Patch operations

The MCP endpoint is an additional interface that coexists with REST endpoints.

## Example Included
# CQRS Pattern Solution

This CQRS solution demonstrates a modern approach to building scalable and maintainable .NET applications by separating read and write operations. The implementation follows these key architectural principles:

1. **Command-Query Separation**: Commands (write operations) are handled separately from queries (read operations), allowing each to be optimized independently.

2. **Domain-Driven Design**: The solution structure follows DDD principles with clear boundaries between different domains and concerns.

3. **Event-Driven Architecture**: Changes to data are communicated through events, enabling real-time updates using Server-Sent Events (SSE).

4. **Clean Architecture**: The codebase follows the onion architecture pattern with dependencies flowing from outside to inside layers.

This example includes:
- Employee management with CRUD operations
- Weather forecast service with real-time updates
- Server-Sent Events implementation for pushing updates to clients
- Separate read and write databases demonstrating true CQRS
- Comprehensive test projects for each layer of the application

The solution components work together to showcase how CQRS can be implemented in a real-world .NET application while maintaining clean code and separation of concerns.

## Central Package Management

This solution uses Central Package Management (CPM) to manage NuGet packages across all projects. This ensures consistent package versions throughout the solution and simplifies package updates.

### How It Works

- Package versions are defined in the `Directory.Packages.props` file at the solution root
- All projects automatically inherit these package versions
- No need to specify versions in individual project files

### Usage

#### Adding a package reference to a project

In your project file (.csproj):

Employee Endpoint
![employee_endpoint](./Screenshots/employee_endpoint.jpg)

Multiple DB connections
![connection_strings](./Screenshots/connection_strings.jpg)

SSE Console

![SSE_Console.jpg](./Screenshots/SSE_Console.jpg)

## Libraries:
- **MediatR**: MediatR is a lightweight library designed for implementing the Mediator pattern in .NET applications.
- **OpenAPI Documentation**: Automatically generated API documentation using OpenAPI for better understanding and testing of the 
API.
- **Entity Framework**: Entity Framework (EF) is an open-source object-relational mapping (ORM) framework developed by Microsoft for .NET applications.
- **Scalar**: Replaces Swagger for calling and testing APIs.