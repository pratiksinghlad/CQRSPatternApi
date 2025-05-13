using CQRSPattern.Application.Mediator;
using MediatR;

namespace CQRSPattern.Migrator.Infra
{
    public class MediatorScope : IMediatorScope
    {
        private readonly IMediator _mediator;

        public MediatorScope(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishAsync(
            object notification,
            CancellationToken cancellationToken = default
        )
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        public async Task PublishAsync<TNotification>(
            TNotification notification,
            CancellationToken cancellationToken = default
        )
            where TNotification : INotification
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        public async Task<TResponse> SendAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default
        )
        {
            return await _mediator.Send(request, cancellationToken);
        }

        public async Task<object?> SendAsync(
            object request,
            CancellationToken cancellationToken = default
        )
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}
