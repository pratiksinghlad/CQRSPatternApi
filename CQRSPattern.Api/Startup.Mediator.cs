using CQRSPattern.Application.Features.Employee.Add;
using CQRSPattern.Infrastructure.Mediator;
using FluentValidation;
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

        AssemblyScanner
            .FindValidatorsInAssembly(typeof(AddEmployeeCommandValidator).Assembly)
            .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));
    }
}
