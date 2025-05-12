using Autofac;
using CQRSPattern.Application.Mediator;

namespace CQRSPattern.Infrastructure.Mediator
{
    /// <summary>
    /// Factory used to add features in the future for mediator scopes
    /// </summary>
    public class MediatorFactory : IMediatorFactory
    {
        /// <summary>
        /// CTor
        /// </summary>
        /// <param name="lifetimeScope"></param>
        public MediatorFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Create mediatorscope instance
        /// </summary>
        /// <returns></returns>
        public IMediatorScope CreateScope()
        {
            return _lifetimeScope.Resolve<IMediatorScope>();
        }

        private ILifetimeScope _lifetimeScope;
    }
}
