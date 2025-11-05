# Quick Start Guide - MCP Server and API

## Overview

This guide helps you quickly get started with the CQRS Pattern API and MCP Server integration.

## Prerequisites

- .NET 9 SDK installed
- MySQL Server running (or modify connection strings to point to your database)

## Option 1: Using the API Only (Recommended for Quick Start)

This is the simplest way to get started. You only need to run the API.

### Step 1: Update Database Configuration

Edit `CQRSPattern.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "ReadDb": "Server=localhost;Database=cqrs_read;User=root;Password=your_password;",
    "WriteDb": "Server=localhost;Database=cqrs_write;User=root;Password=your_password;"
  }
}
```

### Step 2: Run Database Migrations

```bash
cd CQRSPattern.Migrator
dotnet run
```

### Step 3: Start the API

```bash
cd ../CQRSPattern.Api
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

### Step 4: Test the API

**Test REST endpoint:**
```bash
curl http://localhost:5000/api/employees
```

**Test MCP endpoint:**
```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.getAll",
    "id": "test-001"
  }'
```

**Test Health Check:**
```bash
curl http://localhost:5000/health/ready
```

## Option 2: Using the MCP Server for IDE Integration

This option is for integrating with IDEs like VS Code with GitHub Copilot.

### Step 1: Start the API

Follow steps 1-3 from Option 1 above.

### Step 2: Configure Your IDE

Add to your IDE's MCP configuration (e.g., `.vscode/mcp.json`):

```json
{
  "mcpServers": {
    "cqrspattern": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/full/path/to/CQRSPatternMcpServer/CQRSPattern.McpServer.csproj"
      ],
      "env": {
        "CQRS_API_URL": "http://localhost:5000"
      }
    }
  }
}
```

### Step 3: Use from IDE

Your IDE's AI assistant (e.g., GitHub Copilot) can now use the MCP tools to:
- Query employees
- Execute commands
- Check API health

## Available Endpoints

### REST API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/employees | Get all employees |
| POST | /api/employees | Create employee |
| PUT | /api/employees/{id} | Update employee |
| PATCH | /api/employees/{id} | Partial update |
| GET | /health/ready | Health check |

### MCP Endpoint

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /mcp/request | MCP protocol endpoint |

**Available MCP Methods:**
- `employee.getAll` - Get all employees
- `employee.add` - Add new employee
- `employee.update` - Update employee
- `employee.patch` - Partial update employee

## Example MCP Requests

### Get All Employees

```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.getAll",
    "id": "req-001"
  }'
```

### Add Employee

```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.add",
    "params": {
      "firstName": "John",
      "lastName": "Doe",
      "gender": "Male",
      "birthDate": "1990-01-01T00:00:00Z",
      "hireDate": "2020-01-01T00:00:00Z"
    },
    "id": "req-002"
  }'
```

### Update Employee

```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.update",
    "params": {
      "id": 1,
      "request": {
        "firstName": "Jane",
        "lastName": "Smith",
        "gender": "Female",
        "birthDate": "1990-01-01T00:00:00Z",
        "hireDate": "2020-01-01T00:00:00Z"
      }
    },
    "id": "req-003"
  }'
```

### Partial Update (Patch)

```bash
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.patch",
    "params": {
      "id": 1,
      "request": {
        "lastName": "Updated-LastName"
      }
    },
    "id": "req-004"
  }'
```

## Troubleshooting

### API won't start
- Check if port 5000/5001 is available
- Verify MySQL is running
- Check connection strings in appsettings

### Database connection errors
- Verify MySQL is running
- Check connection strings
- Ensure databases exist (run migrations)

### MCP Server won't connect to API
- Ensure API is running first
- Check `CQRS_API_URL` environment variable
- Verify firewall settings

## Next Steps

- Explore the API documentation at `/scalar` when the API is running
- Read the full [README.md](README.md) for detailed information
- Check [ARCHITECTURE.md](ARCHITECTURE.md) for architecture details
- Review the MCP Server documentation in [CQRSPatternMcpServer/README.md](CQRSPatternMcpServer/README.md)

## Configuration Files Reference

- `mcp.json` - MCP server connection configuration
- `appsettings.json` - API base configuration
- `appsettings.Development.json` - Development environment settings
- `CQRSPatternMcpServer/appsettings.json` - MCP server settings

## Support

For more information, see the comprehensive [README.md](README.md) file in the repository root.
