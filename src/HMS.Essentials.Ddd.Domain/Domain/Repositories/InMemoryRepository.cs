using HMS.Essentials.Domain.Entities;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace HMS.Essentials.Domain.Repositories;

/// <summary>
/// In-memory implementation of a repository for testing and development purposes.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public class InMemoryRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TEntity> _entities = new();
    private readonly object _lockObject = new();

    /// <inheritdoc/>
    public override Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = GetQueryable().FirstOrDefault(predicate);
        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_entities.Values.ToList());
    }

    /// <inheritdoc/>
    public override Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = GetQueryable().Where(predicate).ToList();
        return Task.FromResult(entities);
    }

    /// <inheritdoc/>
    public override IQueryable<TEntity> GetQueryable()
    {
        return _entities.Values.AsQueryable();
    }

    /// <inheritdoc/>
    public override Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        lock (_lockObject)
        {
            // Generate ID for new entities if needed (only for int and long types with default values)
            if (EqualityComparer<TKey>.Default.Equals(entity.Id, default))
            {
                if (typeof(TKey) == typeof(int))
                {
                    var maxId = _entities.Keys.Any() 
                        ? _entities.Keys.Max(k => Convert.ToInt32(k)) 
                        : 0;
                    SetEntityId(entity, (TKey)(object)(maxId + 1));
                }
                else if (typeof(TKey) == typeof(long))
                {
                    var maxId = _entities.Keys.Any() 
                        ? _entities.Keys.Max(k => Convert.ToInt64(k)) 
                        : 0L;
                    SetEntityId(entity, (TKey)(object)(maxId + 1L));
                }
            }

            if (!_entities.TryAdd(entity.Id, entity))
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} already exists.");
            }
        }

        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        lock (_lockObject)
        {
            if (!_entities.ContainsKey(entity.Id))
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} does not exist.");
            }

            _entities[entity.Id] = entity;
        }

        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        lock (_lockObject)
        {
            _entities.TryRemove(entity.Id, out _);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In-memory repository doesn't need to save changes
        return Task.FromResult(0);
    }

    /// <summary>
    /// Sets the entity ID using reflection (for testing purposes only).
    /// </summary>
    private static void SetEntityId(TEntity entity, TKey id)
    {
        var propertyInfo = typeof(TEntity).GetProperty(nameof(IEntity<TKey>.Id));
        if (propertyInfo != null && propertyInfo.CanWrite)
        {
            propertyInfo.SetValue(entity, id);
            return;
        }

        // Property is read-only, need to set the backing field
        // Try to find the backing field in the entity type hierarchy
        var currentType = typeof(TEntity);
        while (currentType != null && currentType != typeof(object))
        {
            // Try auto-property backing field pattern
            var backingField = currentType.GetField($"<{nameof(IEntity<TKey>.Id)}>k__BackingField",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            if (backingField != null)
            {
                backingField.SetValue(entity, id);
                return;
            }

            // Try other common patterns
            backingField = currentType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.DeclaredOnly)
                .FirstOrDefault(f => f.Name.Equals("_id", StringComparison.OrdinalIgnoreCase) || 
                                     f.Name.Contains("id", StringComparison.OrdinalIgnoreCase));
            
            if (backingField != null)
            {
                backingField.SetValue(entity, id);
                return;
            }

            currentType = currentType.BaseType;
        }
    }

    /// <summary>
    /// Clears all entities from the repository.
    /// </summary>
    public void Clear()
    {
        lock (_lockObject)
        {
            _entities.Clear();
        }
    }
}

/// <summary>
/// In-memory repository for entities with integer keys.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
public class InMemoryRepository<TEntity> : InMemoryRepository<TEntity, int>
    where TEntity : class, IEntity<int>
{
}
