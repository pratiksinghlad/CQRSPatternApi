---
trigger: always_on
---

# Antigravity & Gemini Project Instructions

- All code must target .NET 9 and use C# 13 features and syntax[4][9].
- Follow the official C# 13 coding conventions and best practices.
- Use modern C# 13 features such as:
  - `params` collections (supporting Span<T>, IEnumerable<T>, etc.)
  - The new `lock` type and semantics
  - The `\e` escape sequence for Unicode U+001B
  - Method group natural type improvements
  - Implicit indexer access in object initializers
  - `ref` locals and `unsafe` contexts in iterators/async methods
  - `ref struct` types implementing interfaces
  - Partial properties and indexers in partial types[4][9]
- All generated code must be idiomatic, clear, and maintainable.
- Use XML documentation comments for all public types and members.
- Prefer expressive variable and method names; avoid abbreviations.
- Write unit tests for all new methods using xUnit.
- Avoid obsolete patterns and APIs; use the latest .NET 9 libraries.
- Ensure thread safety and proper use of async/await patterns.
- Use dependency injection for services and data access.
- All code should compile without warnings or errors.
- Include comments where logic is non-trivial.

# Project Structure Guidelines

- Organize code into folders by feature or domain.
- Place interfaces in an `Interfaces` folder.
- Place implementation classes in appropriate feature folders.
- Place unit tests in a parallel `Tests` project.

# Example Usage

- When generating a service, use constructor injection for dependencies.
- When writing async methods, always use `Task` or `Task<T>` return types and include `Async` in method names.

# MUST DO

- After all changes change project should run and unit test should run without any errors.
- After all changes update the README.md file.
- Design principles should be DRY, SOLID and clean.
- Use dependency injection.
- Write Code simple, maintable, testable and readable.
- Add comments where logic is complex. for public method add XML comments.
- Simplicity is the ultimate sophistication for code.
- High code quality and c# microsoft standards should be followed.
