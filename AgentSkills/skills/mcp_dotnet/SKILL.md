---
name: mcp_dotnet
description: >
  Load when building, reviewing, or extending .NET MCP server tools,
  transports, contracts, and IDE/client integration.
---

# MCP .NET Skill

Use this skill for Model Context Protocol work in this repository.

## Project Rules

- Keep MCP tools thin; route behavior through the same MediatR commands and queries used by REST endpoints.
- Do not duplicate business logic in MCP tool methods.
- Treat MCP tool contracts as public API: use stable names, clear descriptions, explicit input types, and small output DTOs.
- Pass `CancellationToken` through all tool, mediator, EF Core, and HTTP boundaries.
- Keep stdout reserved for MCP protocol messages when using stdio transport; send logs to stderr.
- Validate tool inputs before dispatching commands or queries.
- Avoid exposing raw entities, stack traces, connection strings, secrets, or internal schema details.
- Add or update tests for tool behavior and contract mapping when MCP behavior changes.

## Tool Design Checklist

- [ ] Tool name is action-oriented and stable, for example `get_employee_by_id`.
- [ ] Description tells the model when to use the tool.
- [ ] Inputs are minimal and strongly typed.
- [ ] Output is a DTO or result object, not an EF entity.
- [ ] Tool delegates to MediatR or an existing application service.
- [ ] Errors are structured and safe to show to an AI client.
- [ ] Cancellation token is accepted and passed through.
- [ ] New tools are documented in the relevant MCP docs when public usage changes.

## Implementation Shape

```csharp
[McpTool("get_employee_by_id", "Gets one employee by id.")]
public static async Task<EmployeeDto?> GetEmployeeByIdAsync(
    Guid employeeId,
    IMediator mediator,
    CancellationToken cancellationToken)
{
    return await mediator.Send(
        new GetEmployeeByIdQuery(employeeId),
        cancellationToken);
}
```

## Transport Notes

- HTTP transport can share ASP.NET Core middleware for auth, validation, logging, and error handling.
- Stdio transport must not write normal logs to stdout.
- Keep `mcp.json` entries simple and point to built server artifacts or stable commands.
- If a client cannot discover tools, verify the server builds, starts, and returns valid MCP initialize/list-tools responses.
