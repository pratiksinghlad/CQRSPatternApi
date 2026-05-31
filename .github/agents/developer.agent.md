---
name: developer
description: >
  Load when implementing features in .NET C# API.
  Always load architect skill first if structure is not defined.
  Always follows CQRS. Always loads code-standards and dotnet-best-practices.
---

# Developer Agent

## Stack
- .NET 10, C#, Minimal API or Controller API
- MediatR for CQRS dispatch
- FluentValidation for input validation
- EF Core for SQL
- MCP Server SDK for tool surface

## CQRS Implementation Pattern

### Command
```csharp
// Features/Orders/Commands/CreateOrder/CreateOrderCommand.cs
public sealed record CreateOrderCommand(Guid CustomerId, List<OrderItem> Items)
    : IRequest<Guid>;

// Features/Orders/Commands/CreateOrder/CreateOrderHandler.cs
public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;

    public CreateOrderHandler(IOrderRepository repository)
        => _repository = repository;

    public async Task<Guid> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerId, command.Items);
        await _repository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}
```

### Query
```csharp
// Features/Orders/Queries/GetOrderById/GetOrderByIdQuery.cs
public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;

// Features/Orders/Queries/GetOrderById/GetOrderByIdHandler.cs
public sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderReadRepository _readRepository;

    public GetOrderByIdHandler(IOrderReadRepository readRepository)
        => _readRepository = readRepository;

    public async Task<OrderDto?> Handle(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken)
        => await _readRepository.GetByIdAsync(query.OrderId, cancellationToken);
}
```

### Controller (dispatch only — no logic)
```csharp
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var orderId = await _mediator.Send(
            new CreateOrderCommand(request.CustomerId, request.Items),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { orderId }, null);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetById(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _mediator.Send(
            new GetOrderByIdQuery(orderId),
            cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }
}
```

### MCP Tool (if feature needs MCP surface)
```csharp
// Mcp/Tools/OrderTools.cs
public static class OrderTools
{
    [McpTool("create_order", "Creates a new order")]
    public static async Task<CreateOrderResult> CreateOrder(
        [McpParameter("customerId")] Guid customerId,
        [McpParameter("items")] List<OrderItem> items,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var orderId = await mediator.Send(
            new CreateOrderCommand(customerId, items),
            cancellationToken);

        return new CreateOrderResult(orderId);
    }
}
```

## Rules
- Records for Commands, Queries, and DTOs — immutable by default
- sealed on every class not designed for inheritance
- No business logic in handlers — delegate to domain entities
- No raw SQL strings — use parameters always
- Async all the way — no .Result or .Wait()
- Optimize EF Core queries for read paths: avoid N+1 queries, use `.AsNoTracking()`, project DTOs, and paginate lists
- Keep API payloads small and avoid excessive logging in hot paths
