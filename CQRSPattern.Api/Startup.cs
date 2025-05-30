using System.Text.Json.Serialization;
using Autofac;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.FeatureManagement;
using Scalar.AspNetCore;

namespace CQRSPattern.Api;

/// <summary>
/// Startup class called from Program.cs
/// </summary>
public partial class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<Registrations>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true; // This is important for https, by default it is disabled for https.
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

        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
            );
        });

        services.AddHttpContextAccessor();
        services.AddResponseCaching();

        LoadConfiguration(services);
        LoadMediator(services);
        LoadScalar(services);

        LoadHealthChecks(services);

        services.AddFeatureManagement(Configuration.GetSection("FeatureManagement"));
        services
            .AddMvc(options => { })
            .AddControllersAsServices()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );
        services.AddMemoryCache();
        services.AddApiVersioning(options => options.ReportApiVersions = true);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Validate architecture in development and staging environments
        if (env.IsDevelopment() || env.IsStaging())
        {
            ValidateArchitecture(app.ApplicationServices);
        }
    
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
    
        app.UseResponseCompression();
        app.UseStatusCodePages();
        app.UseRouting();
        app.UseCors("AllowAll");
    
        UseScalar(ref app);
    
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapScalarApiReference(opt =>
            {
                opt.Title = $"CQRS API Documentation - {env.EnvironmentName}";
                if (env.IsDevelopment())
                    opt.Theme = ScalarTheme.DeepSpace;
                else if (env.IsStaging())
                    opt.Theme = ScalarTheme.BluePlanet;
                else
                    opt.Theme = ScalarTheme.Purple;
            });
    
            endpoints.MapHealthChecks(
                "/health/ready",
                new HealthCheckOptions()
                {
                    Predicate = (check) => true,
                    ResponseWriter = WriteResponse,
                }
            );
    
            endpoints.MapHealthChecks(
                "/health/live",
                new HealthCheckOptions()
                {
                    Predicate = (check) => false,
                    ResponseWriter = WriteResponse,
                }
            );
        });
    
        app.UseCookiePolicy();
    }
    
    private void ValidateArchitecture(IServiceProvider serviceProvider)
    {
        // Create a scope to resolve the architecture validator
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
        
        try
        {
            var architectureValidator = new Architecture.ArchitectureValidator(
                scope.ServiceProvider.GetRequiredService<ILogger<Architecture.ArchitectureValidator>>());
            
            var isValid = architectureValidator.ValidateArchitecture();
            
            if (!isValid)
            {
                logger.LogWarning("Architecture validation failed. The application may not be structured according to the defined architecture rules.");
                
                // Optionally, you can make this a critical error and shut down in development to enforce architecture compliance
                // In production, we typically want to continue running even if validation fails
                if (Environment.GetEnvironmentVariable("ENFORCE_ARCHITECTURE") == "true")
                {
                    throw new Exception("Architecture validation failed. Fix the violations before running the application.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while validating architecture");
        }
    }
}
