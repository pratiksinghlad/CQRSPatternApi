using CQRSPattern.Infrastructure.Mediator;
using MediatR;
using MediatR.Pipeline;

namespace CQRSPattern.Api;

public partial class Startup
{
    /// <summary>
    /// Load mediator decorator functionality.
    /// </summary>
    /// <param name="services"></param>
    public void LoadMediator(IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MediatorValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
    }
}
