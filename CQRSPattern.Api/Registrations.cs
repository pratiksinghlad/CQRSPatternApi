using Autofac;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Factories;
using CQRSPattern.Infrastructure.Persistence.Repositories.Read;
using CQRSPattern.Application.Mediator;
using CQRSPattern.Application.Infrastructure.Infra;
using CQRSPattern.Infrastructure.Mediator;
using MediatR;
using Microsoft.Extensions.Options;

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

        RegisterRepositories(ref builder);
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
        builder.Register(c =>
        {
            var config = c.Resolve<IOptions<ConnectionStrings>>();
            var logger = c.Resolve<ILogger<ReadDbContext>>();
            return new ReadDbContext(config, logger);
        }).As<IReadDbContext>().AsSelf().InstancePerLifetimeScope();

        builder.Register(c =>
        {
            var config = c.Resolve<IOptions<ConnectionStrings>>();
            var logger = c.Resolve<ILogger<WriteDbContext>>();
            return new WriteDbContext(config, logger);
        }).As<IWriteDbContext>().AsSelf().InstancePerLifetimeScope();
    }

    private static void RegisterRepositories(ref ContainerBuilder builder)
    {
        builder.RegisterType<EmployeeReadRepository>().AsImplementedInterfaces();
        builder.RegisterType<EmployeeWriteRepository>().AsImplementedInterfaces();
    }
}