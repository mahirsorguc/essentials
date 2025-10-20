using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that measures and logs performance of command/query execution.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;
    private readonly EssentialsMediatROptions _essentialsMediatROptions;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, IOptions<EssentialsMediatROptions> essentialsMediatROptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _essentialsMediatROptions = essentialsMediatROptions.Value;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_essentialsMediatROptions.PerformanceLoggingEnabled)
        {
            return await next(cancellationToken);
        }
        
        _timer.Start();

        var response = await next(cancellationToken);

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // Log if request takes more than 500ms
        {
            var requestName = typeof(TRequest).Name;
            
            _logger.LogWarning(
                "Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds)",
                requestName,
                elapsedMilliseconds);
        }

        return response;
    }
}
