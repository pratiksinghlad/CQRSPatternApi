using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace CQRSPattern.Api.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and returning consistent error responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "An error occurred while processing your request.",
            status = (int)HttpStatusCode.InternalServerError,
            traceId = context.TraceIdentifier,
            errors = new Dictionary<string, string[]>
            {
                { "Error", new[] { GetErrorMessage(exception) } }
            }
        };

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }

    private string GetErrorMessage(Exception exception)
    {
        // In development, return the full exception details
        // In production, return a generic message
        return _environment.IsDevelopment() 
            ? exception.ToString() 
            : "An error occurred processing your request. Please try again later.";
    }
}

/// <summary>
/// Extension methods for using the exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds the exception handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}