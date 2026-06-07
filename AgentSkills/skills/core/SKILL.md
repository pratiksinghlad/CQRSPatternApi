---
name: core
description: >
  Load on every task. Non-negotiable principles that govern
  all work: how to think, what to change, and how to verify.
---

# Core Principles

> Simplification is the ultimate sophistication.
> Write code that is simple, small, easy to delete, and adds nothing unnecessary.

1. **Think Before Coding** – Understand the goal before writing a line. If requirements are ambiguous, **stop and ask** — assumptions compound. Surface tradeoffs to the caller, not just in comments.
2. **Simplicity First** - Write the minimum code that correctly solves the problem. Avoid speculative features.
3. **Surgical Changes** – Touch only what the task requires. Avoid "while I'm here" changes. Refactoring is a separate, explicit task — never bundled into a feature or fix.
4. **Goal-Driven Testing** – Define success before writing code. Tests are the contract. Cover the failure path — a test suite that only passes on happy inputs is not a contract, it's a wish.
5. **No Race Conditions** - Treat shared mutable state as dangerous. Prefer immutability.
6. **Agent Memory**  – Every task is a learning loop. Read `AgentSkills/memory/index.md` at task start, then load only the domain file(s) matching your task. Before closing, check if a new lesson is warranted — if so, read `AgentSkills/memory/schema.md`, append one line to the correct domain file, and update the count in `index.md`.

7. **Fail Loudly, Recover Gracefully** – Errors are a first-class output. When something goes wrong, fail loudly with context (what, where, relevant IDs). Never swallow exceptions silently. Recovery paths must be as deliberate as the happy path.

8. **Prefer Reversible Moves** – When two approaches solve the problem equally well, choose the one that can be undone. Flag irreversible changes (schema drops, destructive migrations, hard deletes) explicitly before executing them.

## Pre-submit Checklist

- [ ] No magic literals: important values are named.
- [ ] Each function does exactly one thing.
- [ ] No argument list longer than 4; use an object, options type, or record.
- [ ] No nesting deeper than 2 levels; prefer early returns.
- [ ] No duplicated logic; shared behavior is extracted.
- [ ] No comment merely restates the code.
- [ ] No race condition on shared mutable state.
- [ ] Commit message states what changed and why, when a commit is made.
- [ ] Only required code changed; nothing extra.
- [ ] SOLID respected, especially Single Responsibility.
- [ ] Checked `AgentSkills/memory/index.md`, loaded relevant domain lessons, and appended any new lesson to the correct domain file.
- [ ] No sensitive data (keys, PII, tokens) in logs, errors, or responses.
- [ ] All external inputs validated before use; outputs sanitized before rendering.
- [ ] Errors carry context (what failed, where, relevant IDs) — no bare "something went wrong".
- [ ] Code is as simple as possible, but no simpler. No speculative generality.
- [ ] Code is small and focused; if it grows, it should be split.
- [] Structured log entries used; no raw string concatenation in logs.
- [] Correlation/trace ID propagated on all cross-service calls.
- [] All async operations have cancellation path.
- [ ] API responses use correct HTTP status codes (no 200 for errors).
- [ ] Pagination/filtering implemented for any collection endpoint.
- [ ] Breaking API changes go in a new version; existing contracts preserved.
- [ ] Tests cover the failure path, not just the happy path.
- [ ] Test names describe behavior, not implementation ("returns 404 when user not found", not "testGetUser").
- [ ] No test depends on external state or order of execution.