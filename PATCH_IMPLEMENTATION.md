# HTTP PATCH Implementation for Employee API

This implementation provides two modern approaches for HTTP PATCH operations:

## 1. Standard PATCH with Optional<T> Pattern (Recommended)

**Endpoint:** `PATCH /api/employees/{id}`

**Benefits:**

- Distinguishes between "not provided" vs "explicitly set to null"
- Type-safe with compile-time checking
- Works seamlessly with JSON serialization
- Uses modern C# 13 patterns

**Example Request:**

```json
PATCH /api/employees/1
Content-Type: application/json

{
  "firstName": "John",
  "lastName": null,
  "birthDate": "1990-01-01T00:00:00Z"
}
```

This will:

- Update firstName to "John"
- Set lastName to null (explicitly)
- Update birthDate to the specified date
- Leave gender and hireDate unchanged

## 2. JSON Patch Operations

**Endpoint:** `PATCH /api/employees/{id}/jsonpatch`

**Benefits:**

- Supports complex operations (add, remove, replace, move, copy, test)
- Industry standard (RFC 6902)
- Atomic operations
- Better for complex scenarios

**Swagger UI Format:**

In Swagger UI, you'll see an array input. Use this format:

```json
[
  { "op": "replace", "path": "/FirstName", "value": "Jane" },
  { "op": "replace", "path": "/BirthDate", "value": "1985-06-15T00:00:00Z" },
  { "op": "test", "path": "/Gender", "value": "Female" },
  { "op": "remove", "path": "/LastName" }
]
```

**Supported Operations:**

- `add` - Adds a value to the specified path
- `remove` - Removes the value at the specified path  
- `replace` - Replaces the value at the specified path
- `move` - Moves a value from one path to another (requires `from` field)
- `copy` - Copies a value from one path to another (requires `from` field)
- `test` - Tests that the value at the specified path equals the given value

**Valid Paths:** `/FirstName`, `/LastName`, `/Gender`, `/BirthDate`, `/HireDate`

## Implementation Features

### C# 13 Modern Patterns Used

- `Optional<T>` struct with proper JSON serialization
- Collection expressions `[]` for arrays
- Enhanced pattern matching with `is not null`
- Modern null coalescing patterns
- Expression-bodied members
- File-scoped namespaces

### .NET 9 Features

- Entity Framework ExecuteUpdateAsync (planned, currently using standard approach due to expression tree limitations)
- Modern async/await patterns
- Improved JSON serialization
- Better performance with minimal allocations

### Architecture Benefits

- CQRS pattern compliance
- Mediator pattern for decoupling
- Repository pattern for data access
- Proper validation with FluentValidation
- Comprehensive error handling
- Thread-safe operations

## Performance Considerations

1. **Standard PATCH**: Uses FindAsync + SaveChangesAsync (2 DB operations)
2. **JSON Patch**: Same approach but with more flexibility
3. **Future Enhancement**: Can be upgraded to use ExecuteUpdateAsync for single DB operation

## Validation

Both approaches include:

- Input validation with Data Annotations
- FluentValidation for complex business rules
- Model state validation
- Proper HTTP status codes (400, 404, 204, 500)

## Error Handling

- Comprehensive exception handling
- Proper HTTP status code responses
- Detailed error messages for debugging
- Graceful handling of not found scenarios

## Testing

The implementation includes unit tests for:

- Command handlers
- Validators
- Repository methods
- Controller endpoints

This provides a production-ready HTTP PATCH implementation following Microsoft guidelines and .NET 9 best practices.
