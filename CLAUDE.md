# Claude Instructions

## Skills

Load skills from `.copilot/skills/` based on the task.
See `.copilot/skills/INDEX.md` for what to load and when.

Codex has a mirrored copy under `.codex/skills/`.

## Project Rules

- Follow the target framework and language version declared in `Directory.Build.props`.
- Current repository defaults: `net10.0` and `LangVersion` set to `latest`.
- Write idiomatic, clear, maintainable C#.
- Prefer expressive variable and method names; avoid abbreviations.
- Use dependency injection for services, data access, and infrastructure concerns.
- Use async/await correctly. Async methods must return `Task` or `Task<T>` and use the `Async` suffix.
- Add XML documentation comments for public types and public members.
- Add comments only where the logic is non-trivial and the comment explains why.
- Write or update xUnit tests for new behavior.
- Generated code must compile without warnings or errors.

## After Every Code Change

Before finishing a task:

- Run the pre-submit checklist in `.copilot/skills/core/SKILL.md`.
- Run the relevant build and test commands for the changed area.
- Update `README.md` or other docs only when behavior, setup, commands, or public usage changes.
- Do not mark the task complete if checks fail. List what failed and why.
