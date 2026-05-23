---
name: design
description: >
  Load when designing, architecting, or implementing patterns in C#.
  Covers SOLID, KISS, YAGNI, DRY, composition, Singleton, and Factory.
---

# Design Principles and Patterns

## SOLID

| | Principle | In Practice |
|---|---|---|
| S | Single Responsibility | One class, one reason to change |
| O | Open / Closed | Open to extension, closed to modification |
| L | Liskov Substitution | Subtypes must work wherever the base type works |
| I | Interface Segregation | Small, focused interfaces over large, general ones |
| D | Dependency Inversion | Depend on abstractions, not concrete implementations |

## KISS

Prefer the obvious solution. If a junior engineer cannot understand it in 5 minutes, simplify it.

## YAGNI

Do not build something until it is needed. Solve today's requirement, not tomorrow's guess.

```csharp
// Bad: built "just in case" when only one format exists.
public interface IExporter { }
public class ExporterFactory { }

// Good: solve what exists now.
public string ExportToCsv(Report report) { }
```

## DRY

Keep one source of truth per piece of knowledge: logic, configuration, and data schemas.

## Composition Over Inheritance

Use inheritance only for true "is-a" relationships. Prefer interfaces and delegation.

## Pattern: Singleton

Use when exactly one instance must coordinate shared state. Avoid when it creates hidden global state.

```csharp
public sealed class AppConfig
{
    private static readonly Lazy<AppConfig> InstanceHolder =
        new(() => new AppConfig());

    public static AppConfig Instance => InstanceHolder.Value;

    private AppConfig()
    {
    }
}
```

## Pattern: Factory

Use when creation is complex, varies by type, or must be decoupled from consumers.

```csharp
// Bad
var handler = type == "email" ? new EmailHandler() : new SmsHandler();

// Good
var handler = NotificationFactory.Create(type);
```
