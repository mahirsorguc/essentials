namespace HMS.Essentials.Domain.Entities;

/// <summary>
/// Base class for entities with a single primary key.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public abstract class Entity<TKey> : IEntity<TKey>
{
    protected Entity()
    {
        
    }
    
    protected Entity(TKey id)
    {
        Id = id;
    }

    /// <inheritdoc/>
    public virtual TKey Id { get; }

    /// <inheritdoc/>
    public virtual object[] GetKeys()
    {
        return new object[] { Id! };
    }

    /// <summary>
    /// Checks if this entity is transient (not persisted to database yet).
    /// </summary>
    /// <returns>True if the entity is transient, false otherwise.</returns>
    public virtual bool IsTransient()
    {
        if (EqualityComparer<TKey>.Default.Equals(Id, default))
        {
            return true;
        }

        // Workaround for EF Core - it can track entities with default ids
        if (typeof(TKey) == typeof(int))
        {
            return Convert.ToInt32(Id) <= 0;
        }

        if (typeof(TKey) == typeof(long))
        {
            return Convert.ToInt64(Id) <= 0;
        }

        return false;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TKey> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (IsTransient() || other.IsTransient())
        {
            return false;
        }

        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (IsTransient())
        {
            return base.GetHashCode();
        }

        return HashCode.Combine(GetType(), Id);
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{GetType().Name} {Id}]";
    }
}

/// <summary>
/// Base class for entities with an integer primary key.
/// </summary>
public abstract class Entity : Entity<int>
{
    public Entity()
    {
        
    }
    protected Entity(int id) : base(id)
    {
    }
}

/// <summary>
/// Base class for entities with a GUID primary key.
/// </summary>
public abstract class GuidEntity : Entity<Guid>
{
    public GuidEntity()
    {
        
    }
    
    protected GuidEntity(Guid id) : base(id)
    {
    }
}
