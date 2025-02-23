namespace CQRSPattern.Application.Services
{
    public interface IServiceContext
    {
        /// <summary>
        /// Correlationid which connects all calls.
        /// </summary>
        Guid CorrelationId { get; set; }

        /// <summary>
        /// Authenticated entity (user) with claims.
        /// </summary>
        string AuthenticatedUserEmail { get; set; }

        /// <summary>
        /// When in context of a bus, messageid is known;
        /// </summary>
        Guid? MessageId { get; set; }
    }
}
