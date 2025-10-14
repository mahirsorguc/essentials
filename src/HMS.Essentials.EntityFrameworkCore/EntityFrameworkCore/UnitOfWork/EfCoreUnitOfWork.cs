using HMS.Essentials.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace HMS.Essentials.EntityFrameworkCore.UnitOfWork;

/// <summary>
/// Entity Framework Core implementation of the Unit of Work pattern.
/// Manages database transactions and coordinates repository operations.
/// </summary>
/// <typeparam name="TDbContext">Type of the DbContext.</typeparam>
public class EfCoreUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly ILogger<EfCoreUnitOfWork<TDbContext>>? _logger;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreUnitOfWork{TDbContext}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public EfCoreUnitOfWork(TDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreUnitOfWork{TDbContext}"/> class with logging support.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public EfCoreUnitOfWork(TDbContext dbContext, ILogger<EfCoreUnitOfWork<TDbContext>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool HasActiveTransaction => _currentTransaction != null;

    /// <inheritdoc/>
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            
            _logger?.LogDebug("SaveChangesAsync completed. {Count} entities affected.", result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while saving changes in Unit of Work.");
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _logger?.LogWarning("A transaction is already active. Cannot begin a new transaction.");
            throw new InvalidOperationException("A transaction is already active.");
        }

        try
        {
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            
            _logger?.LogDebug("Transaction started. Transaction ID: {TransactionId}", 
                _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while beginning transaction.");
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger?.LogWarning("No active transaction to commit.");
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
            
            _logger?.LogDebug("Transaction committed successfully. Transaction ID: {TransactionId}", 
                _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while committing transaction. Rolling back...");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    /// <inheritdoc/>
    public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger?.LogWarning("No active transaction to rollback.");
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            
            _logger?.LogDebug("Transaction rolled back. Transaction ID: {TransactionId}", 
                _currentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while rolling back transaction.");
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// Gets the underlying DbContext.
    /// </summary>
    public TDbContext GetDbContext() => _dbContext;

    /// <summary>
    /// Disposes the unit of work and associated resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work and associated resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_currentTransaction != null)
            {
                _logger?.LogWarning("Disposing Unit of Work with an active transaction. Rolling back...");
                
                // Synchronous disposal - transaction should be completed before disposal
                _currentTransaction.Rollback();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }

            // Note: DbContext is typically managed by DI container, so we don't dispose it here
            // unless this UnitOfWork owns the context instance
        }

        _disposed = true;
    }
}
