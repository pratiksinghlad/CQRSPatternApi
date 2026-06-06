---
name: dotnet-best-practices
description: >
  Load when writing, reviewing, refactoring, or optimizing .NET/C# code.
  Covers project conventions, CQRS, EF Core, async, testing, performance,
  logging, security, and maintainability.
---

# .NET/C# Best Practices

Use this skill to keep .NET/C# changes simple, maintainable, testable, and aligned with this repository.

## Repository Defaults

- Follow the target framework and language version declared in `Directory.Build.props`.
- Current defaults are `net10.0` and `LangVersion` set to `latest`.
- Use modern C# features only when they improve readability or correctness.
- Prefer clear, boring code over clever code.
- Keep changes scoped to the current feature or bug.

## Documentation and Structure

- Add XML documentation comments for public APIs when they are part of the external or shared surface.
- Include meaningful parameter and return descriptions when they add information beyond the name.
- Organize code by feature/domain when the project already follows that structure.
- Keep controllers, endpoints, handlers, validators, repositories, and DTOs in predictable folders.
- Use namespaces that match the project and feature structure.

## CQRS and Architecture

- Commands mutate state and should return nothing, an ID, or a small result.
- Queries read state and must not mutate data.
- Keep controllers and minimal API endpoints thin; they should validate transport concerns and dispatch through MediatR.
- Put business rules in domain/application services or entities, not controllers.
- Use one handler per command or query.
- Prefer records for commands, queries, and DTOs when immutability is appropriate.
- Mark classes `sealed` unless inheritance is intentionally supported.
- Use interfaces for boundaries that need substitution, testing, or multiple implementations.

## Dependency Injection and Services

- Use constructor injection or primary constructors for required dependencies.
- Validate required dependencies when nullable flow does not already prove safety.
- Register services with the narrowest correct lifetime:
  - `Singleton` for stateless, thread-safe services.
  - `Scoped` for request-bound services and EF Core `DbContext`.
  - `Transient` for lightweight stateless services.
- Do not resolve services manually from `IServiceProvider` unless there is a clear framework boundary.
- Avoid hidden global state.

## Async/Await

- Use async/await for I/O, database, network, and long-running operations.
- Do not block async work with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`.
- Prefer async all the way through the call chain.
- Accept and pass `CancellationToken` on request, handler, EF Core, and HTTP calls.
- Use `ConfigureAwait(false)` in reusable library code when appropriate; it is usually unnecessary in ASP.NET Core request code.
- Avoid `async void` except for event handlers.

## EF Core and Database Performance

- Avoid N+1 queries. Use projections, `Include`, explicit joins, or separate optimized queries.
- Use `.AsNoTracking()` for read-only queries.
- Select only required columns into DTOs instead of loading full entities.
- Add pagination for list endpoints; avoid returning unbounded result sets.
- Filter in the database, not in memory.
- Use indexes for frequent filters, joins, and ordering paths.
- Prefer `AnyAsync` over `CountAsync` when checking existence.
- Avoid lazy loading in APIs unless there is a specific measured reason.
- Use `SplitQuery` only when it avoids cartesian explosion from large includes.
- Batch writes where possible and call `SaveChangesAsync` once per unit of work.
- Consider `ExecuteUpdateAsync` and `ExecuteDeleteAsync` for bulk updates/deletes.
- Consider compiled queries only for measured hot paths.
- Always parameterize raw SQL if raw SQL is truly needed.

## Memory and Allocation Performance

- Avoid unnecessary object creation in hot paths.
- Use `StringBuilder` for repeated string concatenation in loops or large text generation.
- Be careful with LINQ in hot paths; simple loops can reduce allocations and improve clarity.
- Avoid exceptions for normal control flow.
- Prefer `Array.Empty<T>()` over new empty arrays.
- Reuse static readonly immutable data when safe.
- Use pooling, `Span<T>`, `Memory<T>`, or `ValueTask` only when measurement shows a real benefit.

## Caching

- Cache frequently used, slow-changing data to reduce database and network load.
- Use in-memory caching for single-instance or per-node data.
- Use distributed caching such as Redis for multi-instance deployments or shared cache state.
- Define clear cache keys, expiration, invalidation, and size limits.
- Avoid caching sensitive user data unless encryption, isolation, and expiry are correct.
- Prevent cache stampedes for expensive entries with locking or single-flight behavior.
- Do not cache data that must be strongly consistent unless the domain explicitly allows staleness.

## API Payload and Response Performance

- Return DTOs that contain only the fields the caller needs.
- Use pagination, filtering, and sorting parameters for collection endpoints.
- Avoid sending large object graphs.
- Enable response compression for compressible text responses such as JSON, HTML, CSS, and JavaScript.
- Do not compress already compressed content such as images, videos, zip files, or PDFs.
- Use ETags, `Last-Modified`, or cache headers where HTTP caching is safe.
- Prefer streaming for large downloads instead of buffering the full payload in memory.

## Logging and Observability

- Use structured logging with message templates and named properties.
- Avoid excessive logging in hot paths, especially per-item logs in loops.
- Do not use string interpolation for expensive log messages unless guarded by `logger.IsEnabled(...)`.
- Log enough context to diagnose failures without logging secrets or personal data.
- Use correlation IDs, request IDs, or tracing context across request boundaries.
- Use metrics/tracing for performance-critical paths instead of relying only on logs.

## Efficient Data Structures

- Use `Dictionary<TKey, TValue>` for fast key lookups.
- Use `HashSet<T>` for uniqueness checks and membership tests.
- Avoid repeated linear searches over large collections.
- Use `List<T>` when order matters and append/read operations dominate.
- Choose immutable or read-only collections when shared state should not change.

## HTTP and External Calls

- Use `IHttpClientFactory` for outbound HTTP calls.
- Set timeouts and pass cancellation tokens.
- Avoid retry storms; use bounded retries with backoff for transient failures.
- Do not make external calls inside tight loops when batching is possible.
- Prefer resilient integration patterns for unreliable dependencies.

## Security

- Validate and sanitize external input.
- Use FluentValidation or existing validation patterns for request validation.
- Never concatenate user input into SQL, file paths, shell commands, or logs.
- Do not log secrets, tokens, passwords, connection strings, or sensitive personal data.
- Use least-privilege configuration for database and external services.

## Testing Standards

- Use xUnit for unit tests.
- Use FluentAssertions where it improves readability.
- Use Moq or the existing repository mocking style for dependencies.
- Follow Arrange, Act, Assert.
- Test success, failure, validation, null/empty input, and cancellation paths when relevant.
- Keep tests deterministic and independent.

## Performance Workflow

- Measure before optimizing unless the issue is obvious and local.
- Prefer simple fixes before complex optimizations.
- Use realistic data sizes when validating performance.
- Consider tools such as logs, metrics, traces, `dotnet-counters`, `dotnet-trace`, BenchmarkDotNet, or database query plans.
- Document any non-obvious optimization with the measured reason.
- Do not trade correctness, security, or maintainability for small unmeasured gains.

## Review Checklist

- [ ] No avoidable N+1 queries.
- [ ] Read-only EF Core queries use `.AsNoTracking()`.
- [ ] Queries project only required columns.
- [ ] Async code does not block on tasks.
- [ ] Cancellation tokens flow through I/O boundaries.
- [ ] No excessive allocations in hot paths.
- [ ] Caching has clear expiration and invalidation.
- [ ] Logs are structured and not excessive.
- [ ] API responses avoid unnecessary payload size.
- [ ] Compression and HTTP caching are used only where appropriate.
- [ ] Data structures match access patterns.
- [ ] Tests cover the changed behavior.
