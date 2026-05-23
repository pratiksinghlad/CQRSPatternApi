# Copilot Instructions

## Skills

Load skills from `skills/` based on the task.
See `skills/INDEX.md` for what to load and when.

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

## Project Structure

- Organize code by feature or domain.
- Place interfaces in an `Interfaces` folder when the project already follows that convention.
- Place implementation classes in the appropriate feature or infrastructure folder.
- Place tests in the matching test project.

## After Every Code Change

Before finishing a task:

- Run the pre-submit checklist in `skills/core/SKILL.md`.
- Run `dotnet build` for all changed projects.
- Run all relevant unit tests for changed files/projects and verify they pass without errors.
- Run code-style and formatting checks enforced by `.editorconfig`.
- Fix any formatting, analyzer, or style violations before completing the task.
- Update `README.md` or other docs only when behavior, setup, commands, or public usage changes.
- Do not mark the task complete if checks fail. List what failed and why.
