# JSON Patch Examples for Employee API

## Correct JSON Patch Operations

### 1. Replace Operation (Most Common)

```json
[
  {
    "op": "replace",
    "path": "/FirstName",
    "value": "Jane"
  }
]
```

### 2. Multiple Replace Operations

```json
[
  {
    "op": "replace",
    "path": "/FirstName", 
    "value": "Jane"
  },
  {
    "op": "replace",
    "path": "/LastName",
    "value": "Smith"
  }
]
```

### 3. Add Operation (for optional properties)

```json
[
  {
    "op": "add",
    "path": "/Gender",
    "value": "Female"
  }
]
```

### 4. Remove Operation

```json
[
  {
    "op": "remove",
    "path": "/Gender"
  }
]
```

### 5. Test Operation (validation)

```json
[
  {
    "op": "test",
    "path": "/FirstName",
    "value": "John"
  },
  {
    "op": "replace",
    "path": "/FirstName",
    "value": "Jane"
  }
]
```

### 6. Move Operation (requires "from")

```json
[
  {
    "op": "move",
    "from": "/FirstName",
    "path": "/LastName"
  }
]
```

### 7. Copy Operation (requires "from")

```json
[
  {
    "op": "copy",
    "from": "/FirstName", 
    "path": "/LastName"
  }
]
```

## Important Notes

1. **Property Names**: Use exact property names with correct casing:
   - ✅ `/FirstName` (correct)
   - ❌ `/firstname` (incorrect)
   - ❌ `/first_name` (incorrect)

2. **Available Properties**:
   - `/Id` (read-only)
   - `/FirstName` (required)
   - `/LastName` (required)
   - `/Gender` (required)
   - `/BirthDate` (optional)
   - `/HireDate` (optional)

3. **Operations requiring "from"**:
   - `move`: Moves a value from one location to another
   - `copy`: Copies a value from one location to another

4. **Operations NOT requiring "from"**:
   - `add`: Adds a new value
   - `remove`: Removes a value
   - `replace`: Replaces an existing value
   - `test`: Tests if a value equals the specified value

## Common Errors

### ❌ Incorrect: lowercase path
```json
[
  {
    "op": "replace",
    "path": "/firstname",
    "value": "Jane"
  }
]
```
**Error**: Property 'firstname' not found

### ❌ Incorrect: missing "from" for move operation
```json
[
  {
    "op": "move",
    "path": "/LastName",
    "value": "Jane"
  }
]
```
**Error**: Move operations require a "from" path

### ✅ Correct: proper move operation
```json
[
  {
    "op": "move",
    "from": "/FirstName",
    "path": "/LastName"
  }
]
```

## Curl Examples

### Replace First Name:
```bash
curl -X PATCH "https://localhost:7001/api/employees/1/jsonpatch" \
  -H "Content-Type: application/json-patch+json" \
  -d '[{"op": "replace", "path": "/FirstName", "value": "Jane"}]'
```

### Multiple Updates:
```bash
curl -X PATCH "https://localhost:7001/api/employees/1/jsonpatch" \
  -H "Content-Type: application/json-patch+json" \
  -d '[
    {"op": "replace", "path": "/FirstName", "value": "Jane"},
    {"op": "replace", "path": "/LastName", "value": "Smith"}
  ]'
```
