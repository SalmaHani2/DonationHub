using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DonationHub.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResult = await validator.ValidateAsync(
                context,
                cancellationToken
            );

            failures.AddRange(validationResult.Errors);
        }

        if (failures.Count == 0)
        {
            return await next();
        }

        var validationErrors = failures
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray()
            );

        return TResponse.Failure(
            ErrorCatalog.Validation.Failed,
            validationErrors
        );
    }
}