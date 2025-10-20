using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that validates commands/queries using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly EssentialsMediatROptions _essentialsMediatROptions;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger, IOptions<EssentialsMediatROptions> essentialsMediatROptions)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _essentialsMediatROptions = essentialsMediatROptions.Value;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_essentialsMediatROptions.FluentValidationEnabled || !_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await next(cancellationToken);
        }

        var requestName = typeof(TRequest).Name;
        _logger.LogWarning("Validation failed for {RequestName}. Errors: {Errors}",
            requestName,
            string.Join(", ", failures.Select(f => f.ErrorMessage)));

        throw new ValidationException(failures);

    }
}
