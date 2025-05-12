using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.FeatureManagement;

namespace CQRSPattern.Api.Extensions;

/// <summary>
/// Extension methods for configuring services in Startup
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures API-specific services
    /// </summary>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );

        services.AddApiVersioning(options => options.ReportApiVersions = true);
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddResponseCaching();
        services.AddFeatureManagement(configuration.GetSection("FeatureManagement"));

        return services;
    }

    /// <summary>
    /// Configures response compression
    /// </summary>
    public static IServiceCollection AddCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[]
                {
                    "application/json",
                    "application/javascript",
                    "text/css",
                    "text/html",
                    "text/json",
                    "text/plain",
                    "text/xml",
                }
            );
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        return services;
    }

    /// <summary>
    /// Configures CORS policies
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );

            // Consider adding more restricted policies for production
            options.AddPolicy(
                "Production",
                policy =>
                {
                    policy.WithOrigins("https://yourdomain.com").AllowAnyMethod().AllowAnyHeader();
                }
            );
        });

        return services;
    }
}
