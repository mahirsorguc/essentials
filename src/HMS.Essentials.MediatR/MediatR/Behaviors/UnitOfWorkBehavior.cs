using HMS.Essentials.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that wraps command execution in a unit of work transaction.
/// Automatically commits the transaction if the command succeeds.
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
        var requestName = typeof(TRequest).Name;

        try
        {
            _logger.LogDebug("Starting transaction for {RequestName}", requestName);
            
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.LogDebug("Transaction committed for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back.", requestName);
            
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }
}
