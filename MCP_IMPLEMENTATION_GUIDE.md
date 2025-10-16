# MCP Server Implementation Guide

## Overview

This document provides detailed information about the MCP (Model Context Protocol) server implementation in the CQRS Pattern API.

## Architecture

The MCP server is implemented as an additional layer on top of the existing CQRS infrastructure:

```
┌─────────────────────────────────────────┐
│         MCP Controller                  │
│         (McpController)                 │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│       MCP Request Router                │
│      (McpRequestRouter)                 │
│                                         │
│  - Routes method to handler             │
│  - Error handling & logging             │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│         MCP Method Handlers             │
│  - EmployeeGetAllHandler                │
│  - EmployeeAddHandler                   │
│  - EmployeeUpdateHandler                │
│  - EmployeePatchHandler                 │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│      Existing CQRS Infrastructure       │
│  - Mediator (MediatR)                   │
│  - Command/Query Handlers               │
│  - Repositories                         │
│  - DbContexts (Read/Write)              │
└─────────────────────────────────────────┘
```

## Components

### 1. Models (`CQRSPattern.Api/Features/Mcp/Models`)

#### McpRequest
Represents an incoming MCP request:
```csharp
{
  "method": "string",      // Required: The method to execute
  "params": { },           // Optional: Method parameters
  "id": "string"           // Optional: Request ID for tracking
}
```

#### McpResponse
Represents an MCP response:
```csharp
{
  "success": true|false,   // Indicates success/failure
  "result": { },           // Result data (when successful)
  "error": { },            // Error info (when failed)
  "id": "string"           // Echoed request ID
}
```

#### McpError
Error information in responses:
```csharp
{
  "code": "ERROR_CODE",    // Machine-readable error code
  "message": "string",     // Human-readable message
  "details": { }           // Optional additional details
}
```

### 2. Router (`CQRSPattern.Api/Features/Mcp/Services`)

#### IMcpRequestRouter / McpRequestRouter
- Routes incoming MCP requests to appropriate handlers
- Handles all protocol-level errors
- Provides comprehensive logging
- Maps exceptions to appropriate error codes

### 3. Handlers (`CQRSPattern.Api/Features/Mcp/Handlers`)

Each handler implements `IMcpMethodHandler` and processes specific methods:

- **EmployeeGetAllHandler**: Handles `employee.getAll`
- **EmployeeAddHandler**: Handles `employee.add`
- **EmployeeUpdateHandler**: Handles `employee.update`
- **EmployeePatchHandler**: Handles `employee.patch`

### 4. Controller (`CQRSPattern.Api/Features/Mcp`)

#### McpController
- Exposes the `POST /mcp/request` endpoint
- Validates incoming requests
- Delegates to the router
- Always returns HTTP 200 (protocol-level success)

## Error Handling

The MCP server uses a layered error handling approach:

### Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `INVALID_REQUEST` | 400 | Request body missing or malformed |
| `INVALID_METHOD` | 200* | Method name is missing or empty |
| `METHOD_NOT_FOUND` | 200* | Requested method not supported |
| `INVALID_PARAMS` | 200* | Parameters incorrectly formatted |
| `VALIDATION_ERROR` | 200* | Business validation failed |
| `NOT_FOUND` | 200* | Requested resource not found |
| `INTERNAL_ERROR` | 200* | Unexpected server error |

\* These return HTTP 200 with `success: false` in the body (MCP convention)

### Exception Mapping

The router automatically maps exceptions:

```csharp
try {
    // Execute handler
} catch (JsonException ex) {
    return Error("INVALID_PARAMS", ex.Message);
} catch (ArgumentException ex) {
    return Error("VALIDATION_ERROR", ex.Message);
} catch (KeyNotFoundException ex) {
    return Error("NOT_FOUND", ex.Message);
} catch (Exception ex) {
    return Error("INTERNAL_ERROR", "An unexpected error occurred");
}
```

## Adding New Methods

To add a new MCP method:

### 1. Create a Handler

```csharp
public sealed class EmployeeDeleteHandler : IMcpMethodHandler
{
    private readonly IMediatorFactory _mediatorFactory;
    private readonly ILogger<EmployeeDeleteHandler> _logger;

    public EmployeeDeleteHandler(
        IMediatorFactory mediatorFactory, 
        ILogger<EmployeeDeleteHandler> logger)
    {
        _mediatorFactory = mediatorFactory;
        _logger = logger;
    }

    public async Task<object?> HandleAsync(
        object? methodParams, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing employee.delete");

        if (methodParams == null)
        {
            throw new ArgumentException("Parameters required for employee.delete");
        }

        var json = JsonSerializer.Serialize(methodParams);
        var deleteRequest = JsonSerializer.Deserialize<DeleteRequest>(json);

        if (deleteRequest?.Id <= 0)
        {
            throw new ArgumentException("Invalid employee ID");
        }

        var scope = _mediatorFactory.CreateScope();
        await scope.SendAsync(new DeleteEmployeeCommand(deleteRequest.Id), cancellationToken);

        return new { message = "Employee deleted successfully" };
    }

    private sealed record DeleteRequest
    {
        public int Id { get; init; }
    }
}
```

### 2. Register the Handler

In `Registrations.cs`:

```csharp
private static void RegisterMcpServices(ref ContainerBuilder builder)
{
    builder.RegisterType<McpRequestRouter>()
        .As<IMcpRequestRouter>()
        .InstancePerLifetimeScope();

    // Existing handlers
    builder.RegisterType<EmployeeGetAllHandler>().AsSelf().InstancePerLifetimeScope();
    builder.RegisterType<EmployeeAddHandler>().AsSelf().InstancePerLifetimeScope();
    builder.RegisterType<EmployeeUpdateHandler>().AsSelf().InstancePerLifetimeScope();
    builder.RegisterType<EmployeePatchHandler>().AsSelf().InstancePerLifetimeScope();
    
    // New handler
    builder.RegisterType<EmployeeDeleteHandler>().AsSelf().InstancePerLifetimeScope();
}
```

### 3. Map the Method

In `McpRequestRouter.cs` constructor:

```csharp
_methodHandlers = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
{
    ["employee.getAll"] = typeof(EmployeeGetAllHandler),
    ["employee.add"] = typeof(EmployeeAddHandler),
    ["employee.update"] = typeof(EmployeeUpdateHandler),
    ["employee.patch"] = typeof(EmployeePatchHandler),
    ["employee.delete"] = typeof(EmployeeDeleteHandler), // New method
};
```

### 4. Add Tests

Create comprehensive unit tests following the pattern in `McpRequestRouterTest.cs` and handler tests.

## Testing

### Unit Tests

All MCP components have comprehensive unit tests:
- `EmployeeGetAllHandlerTest.cs` - Tests for GetAll handler
- `EmployeeAddHandlerTest.cs` - Tests for Add handler  
- `McpRequestRouterTest.cs` - Tests for router logic

Run tests with:
```bash
dotnet test
```

### Manual Testing

#### Using curl

```bash
# Test invalid method
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.delete",
    "params": { "id": 1 },
    "id": "test-001"
  }'

# Test getAll
curl -X POST http://localhost:5000/mcp/request \
  -H "Content-Type: application/json" \
  -d '{
    "method": "employee.getAll",
    "id": "test-002"
  }'

# Test add
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
    "id": "test-003"
  }'
```

#### Using PowerShell

```powershell
$body = @{
    method = "employee.getAll"
    id = "test-001"
} | ConvertTo-Json

Invoke-RestMethod -Method Post `
    -Uri "http://localhost:5000/mcp/request" `
    -ContentType "application/json" `
    -Body $body
```

## Best Practices

### 1. Always Validate Parameters
```csharp
if (methodParams == null)
{
    throw new ArgumentException("Parameters required");
}
```

### 2. Use Structured Logging
```csharp
_logger.LogInformation("Executing {Method} with {Params}", 
    "employee.add", JsonSerializer.Serialize(methodParams));
```

### 3. Return Meaningful Results
```csharp
// Good
return new { 
    message = "Employee created successfully",
    id = employeeId 
};

// Avoid
return true;
```

### 4. Use Consistent Error Messages
```csharp
throw new ArgumentException("Invalid employee ID. Must be greater than 0.");
```

### 5. Add Request ID to Logs
```csharp
_logger.LogInformation("Processing request {RequestId}", request.Id);
```

## Performance Considerations

1. **Dependency Injection**: Handlers are registered with `InstancePerLifetimeScope` for proper scoping
2. **Async All The Way**: All methods use async/await consistently
3. **Logging**: Structured logging for performance monitoring
4. **Error Handling**: Fast-path error handling in router

## Security Considerations

1. **Input Validation**: All parameters are validated before processing
2. **Error Messages**: Generic error messages for internal errors
3. **Logging**: Sensitive data should not be logged
4. **Rate Limiting**: Consider adding rate limiting for production
5. **Authentication**: Add authentication middleware if needed

## Monitoring

The MCP server provides detailed logs at various levels:

- **Information**: Method executions, successful operations
- **Warning**: Validation failures, not found errors
- **Error**: Unexpected exceptions, system errors

Example log output:
```
info: CQRSPattern.Api.Features.Mcp.Services.McpRequestRouter[0]
      Routing MCP request: Method=employee.getAll, Id=test-001
info: CQRSPattern.Api.Features.Mcp.Handlers.EmployeeGetAllHandler[0]
      Retrieved 5 employees
info: CQRSPattern.Api.Features.Mcp.Services.McpRequestRouter[0]
      MCP request completed successfully: Method=employee.getAll, Id=test-001
```

## Compatibility

The MCP implementation:
- ✅ Works alongside existing REST endpoints
- ✅ Uses the same CQRS infrastructure
- ✅ Maintains backward compatibility
- ✅ Can be disabled without affecting REST API
- ✅ Supports .NET 9 and C# 13 features

## Future Enhancements

Potential improvements:
1. **Batch Operations**: Support multiple methods in one request
2. **Streaming**: Support for server-sent events via MCP
3. **WebSocket Support**: Real-time bidirectional communication
4. **GraphQL-style Queries**: More flexible query syntax
5. **Method Discovery**: Endpoint to list available methods
6. **API Versioning**: Version-specific method handlers
