using HMS.Essentials.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that wraps command execution in a unit of work transaction.
/// Automatically commits the transaction if the command succeeds.
/// Only applies to commands decorated with the <see cref="UnitOfWorkAttribute"/>.
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Response type.</typeparam>
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, ICommand<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

    public UnitOfWorkBehavior(
        IUnitOfWork unitOfWork,
        ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var requestName = requestType.Name;

        // Check if the command has UnitOfWorkAttribute
        var unitOfWorkAttribute = requestType.GetCustomAttributes(typeof(UnitOfWorkAttribute), true)
            .FirstOrDefault() as UnitOfWorkAttribute;

        // If attribute is not present or disabled, skip transaction management
        if (unitOfWorkAttribute == null || !unitOfWorkAttribute.IsEnabled)
        {
            _logger.LogDebug("Unit of work is not enabled for {RequestName}, skipping transaction management", requestName);
            return await next(cancellationToken);
        }

        try
        {
            _logger.LogDebug("Starting transaction for {RequestName}", requestName);
            
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next(cancellationToken);

            // Auto-commit if enabled
            if (unitOfWorkAttribute.AutoCommit)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogDebug("Transaction committed for {RequestName}", requestName);
            }
            else
            {
                _logger.LogDebug("Auto-commit is disabled for {RequestName}, transaction left open", requestName);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back.", requestName);
            
            // Auto-rollback if enabled
            if (unitOfWorkAttribute.AutoRollback)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            
            throw;
        }
    }
}
