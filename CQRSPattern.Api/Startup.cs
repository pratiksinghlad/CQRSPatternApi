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
}
