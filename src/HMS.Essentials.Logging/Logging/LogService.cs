using Microsoft.Extensions.Logging;

namespace HMS.Essential.Logging;

/// <summary>
/// Service for application-wide logging operations.
/// </summary>
public interface ILogService
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
}

/// <summary>
/// Implementation of the logging service.
/// </summary>
public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;

    public LogService(ILogger<LogService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogInfo(string message)
    {
        _logger.LogInformation(message);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    public void LogError(string message, Exception? exception = null)
    {
        if (exception != null)
            _logger.LogError(exception, message);
        else
            _logger.LogError(message);
    }
}
