using BookLending.Application.Common.Responses;
using BookLending.Domain.Enums;
using FluentValidation;
using MediatR;

namespace BookLending.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
         where TResponse : ResponseDtoBase, new()
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var errors = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                if (errors.Count > 0)
                {
                    return new TResponse
                    {
                        IsSuccess = false,
                        ErrorCode = ErrorType.ValidationFailed,
                        Message = string.Join(" | ", errors)
                    };
                }
            }
            return await next();
        }
    }
}