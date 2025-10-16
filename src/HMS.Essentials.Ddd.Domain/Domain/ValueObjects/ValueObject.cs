namespace HMS.Essentials.Domain.ValueObjects;

/// <summary>
/// Base class for value objects in Domain-Driven Design.
/// Value objects are immutable objects that are defined by their attributes rather than a unique identity.
/// Two value objects are equal if all their attributes are equal.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Gets the atomic values that compose this value object for equality comparison.
    /// Override this method to provide the components that define equality.
    /// </summary>
    /// <returns>An enumerable of equality components.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Creates a copy of the value object.
    /// </summary>
    /// <returns>A copy of this value object.</returns>
    public ValueObject GetCopy()
    {
        return (ValueObject)MemberwiseClone();
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name} [{string.Join(", ", GetEqualityComponents())}]";
    }
}
