using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that provides centralized error handling for all requests.
/// Catches and logs exceptions, allowing for graceful error handling and recovery.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class ErrorHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ErrorHandlingBehavior<TRequest, TResponse>> _logger;
    private readonly EssentialsMediatROptions _essentialsMediatROptions;

    public ErrorHandlingBehavior(
        ILogger<ErrorHandlingBehavior<TRequest, TResponse>> logger,
        IOptions<EssentialsMediatROptions> essentialsMediatROptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var options = essentialsMediatROptions ?? throw new ArgumentNullException(nameof(essentialsMediatROptions));
        _essentialsMediatROptions = options.Value;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_essentialsMediatROptions.ErrorHandlingEnabled)
        {
            return await next(cancellationToken);
        }

        var requestName = typeof(TRequest).Name;

        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred while processing {RequestName}: {ExceptionType} - {ExceptionMessage}",
                requestName,
                ex.GetType().Name,
                ex.Message);

            // Re-throw the exception to allow proper handling by upper layers
            // This behavior is for logging and centralized error handling only
            throw;
        }
    }
}
