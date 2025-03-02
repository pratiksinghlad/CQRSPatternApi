using Autofac;
using CQRSPattern.Infrastructure.Persistence.Database;
using CQRSPattern.Infrastructure.Persistence.Factories;

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
        builder.RegisterType<MySqlConnectionManager>().As<IMySqlConnectionManager>().InstancePerLifetimeScope();

        builder.RegisterType<RealDbContext>().As<IDatabaseContext>().InstancePerLifetimeScope();
    }
}