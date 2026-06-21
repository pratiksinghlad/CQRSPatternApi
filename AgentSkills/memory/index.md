# Memory Index

Always load this file first. Then load **only** the domain file(s) that match your task.
Do NOT load domain files you do not need. Never load all files at once.

## Domain Map

| Domain | Load when | File | Lessons |
|--------|-----------|------|---------|
| api | CQRS, MediatR, commands, queries, controllers, endpoints | [lessons/api.md](lessons/api.md) | 1 |
| db | EF Core, migrations, schema, seeding, repositories | [lessons/db.md](lessons/db.md) | 0 |
| infra | Build, paths, CI, tooling, portability, AgentSkills setup | [lessons/infra.md](lessons/infra.md) | 4 |
| auth | Authentication, authorisation, JWT, policies, claims | [lessons/auth.md](lessons/auth.md) | 0 |
| testing | xUnit, test data, mocking, coverage, assertions | [lessons/testing.md](lessons/testing.md) | 0 |
| csharp | C# language features, async/await, naming, code style | [lessons/csharp.md](lessons/csharp.md) | 0 |
| design | SOLID, CQRS, Repository, patterns, architecture decisions | [lessons/design.md](lessons/design.md) | 0 |
| mcp | MCP server tools, transports, contracts, .NET MCP integration | [lessons/mcp.md](lessons/mcp.md) | 0 |

## Writing a lesson?

Load `memory/schema.md` for the one-line format. Do not inline logging instructions here.
Update the lesson count in this table after appending to the domain file.
