# Lesson Logging Schema

Load this file **only when writing a new lesson** at the end of a task.
Never load at task start — it adds no value during execution.

## Format

Append one line per lesson to the correct domain file in `memory/lessons/`:

```
RULE: <imperative sentence — what to always do or never do> #tag YYYY-MM-DD
```

## Rules for a good entry

- Start with **Always**, **Never**, or an action verb.
- Keep it under 20 words. The rule only — no symptom narrative, no root cause prose.
- Add one `#tag` describing the symptom class (e.g. `#build-fail`, `#wrong-path`, `#cqrs-violation`).
- Pick the correct domain from `memory/index.md`. If unsure, use `infra.md`.
- After appending, **increment the lesson count** for that domain in `memory/index.md`.

## Example

```
RULE: Never place business logic in MediatR Handlers; delegate to domain entities or services. #cqrs-violation 2026-05-31
```
