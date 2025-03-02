using Autofac;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Factories;
using CQRSPattern.Application.Mediator;
using CQRSPattern.Infrastructure.Mediator;
using MediatR;

namespace CQRSPattern.Api;

/// <summary>
/// Registrations class for DI framework.
/// </summary>
public class Registrations : Module
{
    /// <summary>
    /// Load all registrations.
    /// </summary>
    /// <param name="builder"></param>
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        RegisterMediator(ref builder);

        RegisterInfrastructurePersistence(ref builder);
    }

    private static void RegisterMediator(ref ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
           .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
               .AsClosedTypesOf(typeof(IRequestHandler<,>));

        builder.RegisterType<MediatorFactory>().As<IMediatorFactory>().InstancePerLifetimeScope();
        builder.RegisterType<MediatorScope>().As<IMediatorScope>().InstancePerLifetimeScope();
    }

    private static void RegisterInfrastructurePersistence(ref ContainerBuilder builder)
    {
        builder.RegisterType<MySqlConnectionManager>().As<IMySqlConnectionManager>().InstancePerLifetimeScope();
        builder.RegisterType<RealDbContext>().As<IDatabaseContext>().InstancePerLifetimeScope();
    }
}