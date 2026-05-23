---
name: core
description: >
  Load on every task. Non-negotiable principles that govern
  all work: how to think, what to change, and how to verify.
---

# Core Principles

> Simplification is the ultimate sophistication.
> Write code that is simple, small, easy to delete, and adds nothing unnecessary.

1. **Think Before Coding** - Understand the goal. Surface tradeoffs. Ask when requirements are unclear.
2. **Simplicity First** - Write the minimum code that correctly solves the problem. Avoid speculative features.
3. **Surgical Changes** - Touch only what the task requires. Do not improve unrelated code.
4. **Goal-Driven Testing** - Define success before writing code. Tests are the contract.
5. **No Race Conditions** - Treat shared mutable state as dangerous. Prefer immutability.

## Pre-submit Checklist

- [ ] No magic literals: important values are named.
- [ ] Each function does exactly one thing.
- [ ] No argument list longer than 3; use an object, options type, or record.
- [ ] No nesting deeper than 2 levels; prefer early returns.
- [ ] No duplicated logic; shared behavior is extracted.
- [ ] No comment merely restates the code.
- [ ] No race condition on shared mutable state.
- [ ] Commit message states what changed and why, when a commit is made.
- [ ] Only required code changed; nothing extra.
- [ ] SOLID respected, especially Single Responsibility.
