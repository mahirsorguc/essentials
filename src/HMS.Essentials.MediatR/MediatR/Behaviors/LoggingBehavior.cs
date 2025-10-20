using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that logs command/query execution.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly EssentialsMediatROptions _essentialsMediatROptions;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, IOptions<EssentialsMediatROptions> essentialsMediatROptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _essentialsMediatROptions = essentialsMediatROptions.Value;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_essentialsMediatROptions.RequestLoggingEnabled)
        {
            return await next(cancellationToken);
            
        }
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next(cancellationToken);
            
            _logger.LogInformation("Handled {RequestName} successfully", requestName);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}
