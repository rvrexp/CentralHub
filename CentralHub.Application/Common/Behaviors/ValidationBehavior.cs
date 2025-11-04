using FluentValidation; 
using FluentValidation.Results; 
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CentralHub.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                // No validators for this request, continue.
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            // Run all validators and collect failures
            var validationFailures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            ))
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList(); // This is a List<ValidationFailure>

            if (validationFailures.Any())
            {

                throw new ValidationException(validationFailures);
            }

            // Validation succeeded.
            return await next();
        }
    }
}
