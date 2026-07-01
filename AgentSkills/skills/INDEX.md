# Skills Index

Load the relevant skill from `AgentSkills/skills/` before starting any task.
Start with `AgentSkills/OPERATING.md` for the shared prerequisite and completion workflow.

`AgentSkills/` is the single source of truth for all skills and agents.
Do not duplicate skill content under `.github/`, `.copilot/`, `.codex/`, or other tool-specific folders.
All AI tools and IDEs should load skills from this folder.

## Skills

| Skill | Load when | Path |
|---|---|---|
| **core** | Every task | [SKILL.md](core/SKILL.md) |
| **code-standards** | Writing, reviewing, or refactoring C# code | [SKILL.md](code-standards/SKILL.md) |
| **design** | Designing, architecting, or implementing patterns | [SKILL.md](design/SKILL.md) |
| **dotnet-best-practices** | Writing, reviewing, refactoring, or optimizing .NET/C# code | [SKILL.md](dotnet-best-practices/SKILL.md) |
| **csharp-xunit** | Get best practices for XUnit unit testing, including data-driven tests | [SKILL.md](csharp-xunit/SKILL.md) |
| **mcp_dotnet** | Building, reviewing, or extending .NET MCP server tools, transports, and integration points | [SKILL.md](mcp_dotnet/SKILL.md) |
| **dotnet-api** | Creating, structuring, or reviewing .NET 10 Web API projects with Swagger/OpenAPI, CORS, compression, auth, versioning, and error handling | [SKILL.md](dotnet-api/SKILL.md) |
| **harness** | Changing agent instructions, validation scripts, CI workflows, hooks, or operational guardrails | [SKILL.md](harness/SKILL.md) |
