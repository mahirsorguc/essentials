using HMS.Essentials.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that wraps command execution in a unit of work transaction.
/// Automatically commits the transaction if the command succeeds.
/// Only applies to commands decorated with the <see cref="UnitOfWorkAttribute"/>.
/// 
/// <para>
/// <b>Nested Transaction Handling:</b><br/>
/// This behavior intelligently handles nested transaction scenarios where a command may dispatch 
/// another command that also requires a unit of work. When a transaction is already active:
/// <list type="bullet">
/// <item>The nested command participates in the existing transaction</item>
/// <item>No new transaction is started</item>
/// <item>The nested command does not commit or rollback the transaction</item>
/// <item>Only the outermost command (transaction owner) manages commit/rollback</item>
/// </list>
/// This prevents "nested transaction not supported" errors and ensures proper transaction boundaries.
/// </para>
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

        // Check if a transaction is already active (nested command scenario)
        var isNestedTransaction = _unitOfWork.HasActiveTransaction;
        
        if (isNestedTransaction)
        {
            _logger.LogDebug("Transaction already active for {RequestName}, participating in existing transaction", requestName);
            // Don't start a new transaction, just participate in the existing one
            return await next(cancellationToken);
        }

        // We are starting a new transaction, so we own it and are responsible for commit/rollback
        var transactionOwner = true;

        try
        {
            _logger.LogDebug("Starting new transaction for {RequestName}", requestName);
            
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next(cancellationToken);

            // Auto-commit if enabled and we own the transaction
            if (transactionOwner && unitOfWorkAttribute.AutoCommit)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogDebug("Transaction committed for {RequestName}", requestName);
            }
            else if (!unitOfWorkAttribute.AutoCommit)
            {
                _logger.LogDebug("Auto-commit is disabled for {RequestName}, transaction left open", requestName);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back.", requestName);
            
            // Auto-rollback if enabled and we own the transaction
            if (transactionOwner && unitOfWorkAttribute.AutoRollback)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            
            throw;
        }
    }
}
