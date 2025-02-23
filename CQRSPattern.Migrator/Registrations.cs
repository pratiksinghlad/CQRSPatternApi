using Autofac;
using CQRSPattern.Application.Infrastructure.Persistence.Database;
using CQRSPattern.Application.Infrastructure.Persistence.Factories;

namespace CQRSPattern.Migrator;

public class Registrations : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        RegisterInfrastructurePersistence(ref builder);
    }

    private void RegisterInfrastructurePersistence(ref ContainerBuilder builder)
    {
        builder.RegisterType<SqlConnectionManager>().As<ISqlConnectionManager>().InstancePerLifetimeScope();

        builder.RegisterType<RealDbContext>().As<IDatabaseContext>().InstancePerLifetimeScope();
    }
}