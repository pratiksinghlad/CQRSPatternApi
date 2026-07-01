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

## Prerequisite Gate

Every task starts with the shared operating contract in
`AgentSkills/OPERATING.md`.

Before editing, confirm that you have:

- [ ] Read `AgentSkills/memory/index.md` and loaded only the relevant domain
      lesson file(s).
- [ ] Read `AgentSkills/skills/INDEX.md`.
- [ ] Loaded this core skill.
- [ ] Loaded task-specific skills and agent definitions only when they apply.
- [ ] Identified the expected verification command(s).
- [ ] Checked the worktree context so existing user changes are preserved.

## Pre-submit Checklist

### Scope

- [ ] Only required files changed; no unrelated refactors or metadata churn.
- [ ] Public documentation changed only when behavior, setup, commands, or usage changed.
- [ ] Commit message states what changed and why, when a commit is made.
- [ ] Memory was updated only if a mistake or durable project-specific rule was discovered.

### Design and Maintainability

- [ ] Code is as simple as possible, but no simpler.
- [ ] No speculative generality or unused extension points.
- [ ] Each function does one thing.
- [ ] No argument list longer than 4; use an object, options type, or record.
- [ ] No nesting deeper than 2 levels; prefer early returns.
- [ ] No duplicated logic; shared behavior is extracted only when it is real reuse.
- [ ] SOLID respected, especially Single Responsibility.
- [ ] Important values are named; no magic literals.
- [ ] Comments explain why, not what the code already says.

### Reliability and Security

- [ ] No race condition on shared mutable state.
- [ ] All async operations have a cancellation path.
- [ ] No sensitive data such as keys, PII, or tokens appears in logs, errors, commits, or responses.
- [ ] External inputs are validated before use.
- [ ] Rendered outputs are sanitized where applicable.
- [ ] Errors include useful context: what failed, where, and relevant IDs.
- [ ] Structured log entries are used; no raw string concatenation in logs.
- [ ] Correlation or trace IDs are propagated on cross-service calls.

### API and Contract Behavior

- [ ] API responses use correct HTTP status codes; errors are not returned as `200`.
- [ ] Collection endpoints include pagination and filtering where applicable.
- [ ] Existing API contracts are preserved unless a versioned breaking change is intentional.
- [ ] MCP, REST, and application contracts stay aligned when they expose the same behavior.

### Tests and Verification

- [ ] Tests cover failure paths, not just happy paths.
- [ ] Test names describe behavior, not implementation.
- [ ] Tests do not depend on external state or execution order.
- [ ] Relevant build, test, format, and harness commands were run, or skipped with a clear reason.
