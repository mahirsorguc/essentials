namespace HMS.Essentials.UnitOfWork.InMemoryDummy;

/// <summary>
/// Base implementation of the Unit of Work pattern for in-memory scenarios.
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private bool _hasActiveTransaction;

    /// <inheritdoc/>
    public bool HasActiveTransaction => _hasActiveTransaction;

    /// <inheritdoc/>
    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In-memory implementation - no real persistence
        return Task.FromResult(0);
    }

    /// <inheritdoc/>
    public virtual Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_hasActiveTransaction)
        {
            throw new InvalidOperationException("A transaction is already active.");
        }

        _hasActiveTransaction = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!_hasActiveTransaction)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        _hasActiveTransaction = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!_hasActiveTransaction)
        {
            throw new InvalidOperationException("No active transaction to rollback.");
        }

        _hasActiveTransaction = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                if (_hasActiveTransaction)
                {
                    RollbackTransactionAsync().GetAwaiter().GetResult();
                }
            }

            _disposed = true;
        }
    }
}
