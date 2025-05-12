using CQRSPattern.Application.Mediator;
using MediatR;

namespace CQRSPattern.Infrastructure.Mediator
{
    /// <summary>
    /// Mediator scope
    /// </summary>
    public class MediatorScope : IMediatorScope
    {
        /// <summary>
        /// CTor
        /// </summary>
        /// <param name="mediator"></param>
        public MediatorScope(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Publish a notificationB
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishAsync(
            object notification,
            CancellationToken cancellationToken = default
        )
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        /// <summary>
        /// Publish a typed notification
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishAsync<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        /// <summary>
        /// Send a request which expects a typed response back
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TResponse> SendAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default
        )
        {
            return await _mediator.Send(request, cancellationToken);
        }

        /// <summary>
        /// Send a request which returns an unknown type (object)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<object?> SendAsync(
            object request,
            CancellationToken cancellationToken = default
        )
        {
            return await _mediator.Send(request, cancellationToken);
        }

        private readonly IMediator _mediator;
    }
}
