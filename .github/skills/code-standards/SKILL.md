---
name: code-standards
description: >
  Load when writing, reviewing, or refactoring C# code.
  Covers naming, structure, comments, and commit hygiene.
---

# Code Standards

Language: C#. Every example is production code, not pseudocode.

## 1. No Magic Values

```csharp
// Bad
if (role == 3) { }

// Good
private const int AdminRoleId = 3;
if (role == AdminRoleId) { }
```

## 2. Meaningful Names

Names explain what a value represents and why it matters. If a comment is needed to explain a variable, rename it.

```csharp
// Bad
int d = 86400;
bool flag = true;

// Good
const int SecondsPerDay = 86400;
bool isUserVerified = true;
```

## 3. One Function, One Responsibility

One function should do one thing. If you cannot summarize it in one sentence, split it.

## 4. Descriptive Booleans

```csharp
// Bad
bool check = true;

// Good
bool hasPermission = true;
bool isSessionExpired = false;
```

## 5. DRY

Keep one source of truth per piece of logic. Apply judgment: premature abstraction is also harmful.

## 6. Early Returns Over Deep Nesting

```csharp
// Bad
void Process(User user)
{
    if (user != null)
    {
        if (user.IsActive)
        {
            // Main logic
        }
    }
}

// Good
void Process(User user)
{
    if (user is null) return;
    if (!user.IsActive) return;

    // Main logic
}
```

## 7. Comment Why, Not What

```csharp
// Bad
// Increment counter.
count++;

// Good
// Back off before HTTP 429; this API caps requests at 100 per minute.
if (requestCount >= RateLimitBackoffThreshold) { }
```

## 8. Limit Function Arguments

More than 3 arguments should usually become a config object, command object, or record.

```csharp
// Bad
CreateUser("Alice", "alice@example.com", true, "admin", "UTC");

// Good
CreateUser(new UserConfig
{
    Name = "Alice",
    Email = "alice@example.com",
    IsVerified = true,
    Role = Role.Admin,
    Timezone = "UTC"
});
```

## 9. Meaningful Commits

```text
Bad:
fix bug
update stuff

Good:
fix: prevent double-submit on payment
refactor: extract shared email validation
```
