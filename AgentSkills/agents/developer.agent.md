---
name: developer
description: >
  Load when implementing features in .NET C# API.
  Always load architect guidance first if structure is not defined.
  Always follows CQRS and loads code-standards plus dotnet-best-practices.
---

# Developer Agent

## Stack

- .NET 10 and modern C#.
- ASP.NET Core Minimal APIs or controllers.
- MediatR for CQRS dispatch.
- FluentValidation for input validation.
- EF Core for persistence.
- MCP Server SDK for AI tool surface.

## Implementation Rules

- Use records for commands, queries, and DTOs when immutability is appropriate.
- Mark classes `sealed` unless inheritance is intentionally supported.
- Keep controllers thin; they should validate transport concerns and dispatch through MediatR.
- Keep business rules in domain entities or domain/application services, not controllers or handlers.
- Use repositories or data access abstractions where the project already follows that convention.
- Use async all the way; never block with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.
- Pass `CancellationToken` through request, handler, EF Core, HTTP, and MCP boundaries.
- Optimize read queries with `.AsNoTracking()`, DTO projection, pagination, and no N+1 queries.
- Keep API payloads small and avoid excessive logging in hot paths.

## CQRS Pattern

```csharp
public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<OrderItem> Items)
    : IRequest<Guid>;

public sealed class CreateOrderHandler(IOrderRepository repository)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerId, command.Items);
        await repository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}
```

## Controller Rule

Controllers dispatch only.

```csharp
[HttpPost]
public async Task<IActionResult> Create(
    CreateOrderRequest request,
    CancellationToken cancellationToken)
{
    var orderId = await mediator.Send(
        new CreateOrderCommand(request.CustomerId, request.Items),
        cancellationToken);

    return CreatedAtAction(nameof(GetById), new { orderId }, null);
}
```

## MCP Tool Rule

MCP tools should reuse the same command/query handlers as REST endpoints.

```csharp
[McpTool("create_order", "Creates a new order")]
public static async Task<CreateOrderResult> CreateOrderAsync(
    Guid customerId,
    IReadOnlyList<OrderItem> items,
    IMediator mediator,
    CancellationToken cancellationToken)
{
    var orderId = await mediator.Send(
        new CreateOrderCommand(customerId, items),
        cancellationToken);

    return new CreateOrderResult(orderId);
}
```
