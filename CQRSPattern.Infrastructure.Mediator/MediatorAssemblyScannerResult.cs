namespace CQRSPattern.Infrastructure.Mediator
{
    /// <summary>
    /// Result of scanning for mediator types.
    /// </summary>
    public class MediatorAssemblyScannerResult
    {
        /// <summary>
        /// Validator interface type, e.g. IPreProcessor&lt;AddLogicMediatorCommand&gt;
        /// <code>IPreProcessor&lt;AddLogicMediatorCommand&gt;</code>
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <summary>
        /// Concrete type that implements the InterfaceType
        /// <code>AddLogicMediatorCommandPreProcessor : IPreProcessor&lt;AddLogicMediatorCommand&gt;</code>
        /// </summary>
        public Type Implementation { get; private set; }

        /// <summary>
        /// CTor
        /// </summary>
        /// <param name="interface">Type of interface.</param>
        /// <param name="implementation">Type of implementation of interface.</param>
        public MediatorAssemblyScannerResult(Type @interface, Type implementation)
        {
            InterfaceType = @interface;
            Implementation = implementation;
        }
    }
}
