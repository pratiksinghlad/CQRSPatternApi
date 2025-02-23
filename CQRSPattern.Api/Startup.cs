using Autofac;
using Microsoft.FeatureManagement;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

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
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        services.AddHttpContextAccessor();
        services.AddResponseCaching();

        LoadConfiguration(services);
        LoadMediator(services);
        LoadScalar(services);

        services.AddFeatureManagement(Configuration.GetSection("FeatureManagement"));
        services.AddMvc(options =>
        {
        }).AddControllersAsServices()
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddMemoryCache();
        services.AddApiVersioning(options => options.ReportApiVersions = true);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStatusCodePages();
        app.UseRouting();
        app.UseCors("AllowAll");

        UseScalar(ref app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapScalarApiReference();
        });

        app.UseCookiePolicy();
    }
}