---
name: dotnet-api
description: >
  Load when creating, structuring, or reviewing .NET 10 Web API projects with Swagger/OpenAPI, CORS, compression, auth, versioning, and error handling.
---

# SKILL: .NET 10 RESTful API — Best Practices

## 1. URL & RESOURCE NAMING

- **Nouns only**, plural: `/api/v1/articles`, `/api/v1/users`
- HTTP verb = action; path = resource
- Nest max 2 levels: `GET /articles/123/comments`
- Beyond 2 levels → return URI in JSON body: `"author": "/users/3"`

| ❌ Bad | ✅ Good |
|---|---|
| `GET /getArticles` | `GET /articles` |
| `POST /createUser` | `POST /users` |

---

## 2. HTTP METHODS

| Method | Use | Idempotent |
|---|---|---|
| GET | Retrieve | ✅ |
| POST | Create | ❌ |
| PUT | Full replace | ✅ |
| PATCH | Partial update | ✅ |
| DELETE | Remove | ✅ |

---

## 3. RESPONSE FORMAT & HEADERS

- Always return `application/json`
- Use `camelCase` for JSON keys (default in .NET)
- No outer wrappers — return resource directly

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
```

---

## 4. HTTP STATUS CODES

| Code | When |
|---|---|
| 200 | Success (GET, PUT, PATCH) |
| 201 | Created (POST) — include `Location` header |
| 204 | No content (DELETE) |
| 400 | Bad request / validation failed |
| 401 | Not authenticated |
| 403 | Authenticated but forbidden |
| 404 | Resource not found |
| 500 | Unhandled server error |

---

## 5. GLOBAL ERROR HANDLING

Use `IExceptionHandler` (dotnet 8+) — no stack traces in production.

```csharp
// GlobalExceptionHandler.cs
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." }, ct);
        return true;
    }
}

// Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
app.UseExceptionHandler();
```

Structured error shape — never raw exceptions:
```json
{ "status": 400, "error": "Validation failed", "code": "INVALID_EMAIL" }
```

---

## 6. FILTERING, SORTING & PAGINATION

Query params only — never in the body for GET.

```
GET /products?category=electronics&sort=-price&page=2&limit=20
```

```csharp
[HttpGet]
public IActionResult GetProducts([FromQuery] string? category, [FromQuery] string? sort,
    [FromQuery] int page = 1, [FromQuery] int limit = 20) { ... }
```

- `sort=price` → ascending, `sort=-price` → descending
- Always return pagination metadata: `{ total, page, limit, data: [] }`

---

## 7. VERSIONING

```csharp
// Program.cs
builder.Services.AddApiVersioning(o => {
    o.DefaultApiVersion = new ApiVersion(1);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
}).AddApiExplorer(o => {
    o.GroupNameFormat = "'v'V";
    o.SubstituteApiVersionInUrl = true;
});
```

```csharp
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/articles")]
public class ArticlesController : ControllerBase { }
```

Package: `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer`

---

## 8. SWAGGER / OPENAPI

Add to `.csproj`:
```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

Setup: `AddEndpointsApiExplorer()` + `AddSwaggerGen()` with:
- `SwaggerDoc` per version
- `AddSecurityDefinition("Bearer", ...)` + matching `AddSecurityRequirement`
- `IncludeXmlComments(...)` for XML doc file

Controller annotation:
```csharp
/// <summary>Get all articles</summary>
/// <response code="200">Returns list of articles</response>
[HttpGet]
[ProducesResponseType(typeof(List<ArticleDto>), 200)]
[ProducesResponseType(404)]
public IActionResult GetAll([FromQuery] int page = 1) { ... }
```

Expose only in Development:
```csharp
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
```

---

## 9. RESPONSE COMPRESSION

```csharp
builder.Services.AddResponseCompression(o => {
    o.EnableForHttps = true;
    o.Providers.Add<BrotliCompressionProvider>();
    o.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

app.UseResponseCompression(); // must be first in pipeline
```

Package: `Microsoft.AspNetCore.ResponseCompression` (built-in)

---

## 10. CORS (No Wildcard in Production)

```csharp
builder.Services.AddCors(o => o.AddPolicy("ApiPolicy", p => p
    .WithOrigins("https://myfrontend.com")   // ❌ never .AllowAnyOrigin() in prod
    .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
    .WithHeaders("Authorization", "Content-Type")
    .AllowCredentials()));

app.UseCors("ApiPolicy"); // before UseAuthentication
```

Dev-only: use a separate named policy with `AllowAnyOrigin()`.

---

## 11. SECURITY

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true, ValidIssuer = config["Jwt:Issuer"],
            ValidateAudience = true, ValidAudience = config["Jwt:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });
```

- `app.UseHttpsRedirection()` — mandatory
- Never expose DB schema in endpoint design
- Use `[Authorize]` / `[AllowAnonymous]` explicitly on controllers

---

## 12. CACHING

Options in order of complexity:

| Strategy | Use when |
|---|---|
| `[OutputCache(Duration = 60)]` | Single-server, simple GET caching |
| `IDistributedCache` + Redis | Multi-instance / shared cache |
| `Cache-Control` header | Client/CDN-side caching |

```csharp
// Distributed Redis
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = "localhost:6379");

// Client hint
Response.Headers.CacheControl = "public, max-age=60";
```

Register `AddOutputCache()` + `app.UseOutputCache()` for attribute-based caching.

---

## 13. RATE LIMITING (dotnet 7+ built-in)

```csharp
builder.Services.AddRateLimiter(o => o.AddFixedWindowLimiter("fixed", opt => {
    opt.PermitLimit = 100;
    opt.Window = TimeSpan.FromMinutes(1);
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    opt.QueueLimit = 5;
}));

app.UseRateLimiter();

[HttpGet, EnableRateLimiting("fixed")]
public IActionResult Get() { ... }
```

---

## 14. INPUT VALIDATION

```csharp
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ctx =>
        new BadRequestObjectResult(new {
            status = 400,
            errors = ctx.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
        }));
```

Use `FluentValidation` for complex rules; Data Annotations for simple constraints.

---

## 15. PIPELINE ORDER (Critical)

```csharp
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseCors("ApiPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();
app.UseExceptionHandler();
app.MapControllers();
```

---

## QUICK CHECKLIST

- [ ] Routes: plural nouns, no verbs, max 2 nesting levels
- [ ] `201 Created` returns `Location` header; `204` for DELETE
- [ ] `IExceptionHandler` registered — structured JSON errors, no stack traces
- [ ] Collections: filtering + sorting + pagination with metadata envelope
- [ ] Versioning: `Asp.Versioning.Mvc`, `ReportApiVersions = true`
- [ ] Swagger: XML docs + `ProducesResponseType` on every action + JWT security scheme
- [ ] Compression: Brotli + Gzip, `EnableForHttps = true`, first in pipeline
- [ ] CORS: explicit `WithOrigins` — `AllowAnyOrigin` dev policy only
- [ ] JWT: all 3 validate flags on (`Issuer`, `Audience`, `Lifetime`)
- [ ] Rate limiting: `AddFixedWindowLimiter` + `EnableRateLimiting` per controller/action
- [ ] `InvalidModelStateResponseFactory` returns structured 400 with field errors
- [ ] DTOs only in responses — no raw EF entities exposed
