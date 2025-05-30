﻿using FluentValidation;
using MediatR;
using ValidationException = FluentValidation.ValidationException;

namespace CQRSPattern.Infrastructure.Mediator
{
    /// <summary>
    /// Class used to map Mediatr Validation classes to the DI container
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class MediatorValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// CTor
        /// </summary>
        /// <param name="validators"></param>
        public MediatorValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// <see cref="IPipelineBehavior{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures.Distinct(new ValidationFailureComparer()));
            }

            return next();
        }

        private readonly IEnumerable<IValidator<TRequest>> _validators;
    }
}
