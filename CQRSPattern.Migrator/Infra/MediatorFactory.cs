using Autofac;
using CQRSPattern.Application.Mediator;

namespace CQRSPattern.Migrator.Infra;

public class MediatorFactory : IMediatorFactory
{
    private ILifetimeScope _lifetimeScope;

    public MediatorFactory(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    public IMediatorScope CreateScope()
    {
        return _lifetimeScope.Resolve<IMediatorScope>();
    }
}
