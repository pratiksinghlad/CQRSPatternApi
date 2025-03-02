using Autofac;

namespace CQRSPattern.Migrator;

public static partial class Program
{
    private static void ConfigureContainer(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterModule<Registrations>();
    }
}
