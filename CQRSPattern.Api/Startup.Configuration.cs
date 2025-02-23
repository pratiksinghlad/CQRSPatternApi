using CQRSPattern.Application.Infra;

namespace CQRSPattern.Api;

public partial class Startup
{
    /// <summary>
    /// Load IOptions configuration
    /// </summary>
    /// <param name="services"></param>
    public void LoadConfiguration(IServiceCollection services)
    {
        services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
    }
}
