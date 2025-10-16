using System.Reflection;

namespace HMS.Essentials.Domain.ValueObjects;

/// <summary>
/// Base class for creating enumeration types (smart enums).
/// Enumerations are immutable value objects that provide a type-safe alternative to standard enums
/// with support for additional behavior and properties.
/// </summary>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the name of the enumeration value.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the numeric identifier of the enumeration value.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration"/> class.
    /// </summary>
    /// <param name="id">The numeric identifier.</param>
    /// <param name="name">The name of the enumeration value.</param>
    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Returns a string representation of the enumeration.
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Gets all enumeration values of the specified type.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <returns>All enumeration values.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public |
                                        BindingFlags.Static |
                                        BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Gets the absolute difference between two enumeration values.
    /// </summary>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    /// <summary>
    /// Gets an enumeration value from its display name.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="name">The display name.</param>
    /// <returns>The enumeration value.</returns>
    public static T FromDisplayName<T>(string name) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
        return matchingItem;
    }

    /// <summary>
    /// Gets an enumeration value from its numeric identifier.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="id">The numeric identifier.</param>
    /// <returns>The enumeration value.</returns>
    public static T FromId<T>(int id) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(id, "id", item => item.Id == id);
        return matchingItem;
    }

    /// <summary>
    /// Tries to get an enumeration value from its numeric identifier.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="id">The numeric identifier.</param>
    /// <param name="result">The enumeration value if found.</param>
    /// <returns>True if the value was found; otherwise, false.</returns>
    public static bool TryFromId<T>(int id, out T? result) where T : Enumeration
    {
        result = GetAll<T>().FirstOrDefault(item => item.Id == id);
        return result != null;
    }

    /// <summary>
    /// Tries to get an enumeration value from its display name.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="name">The display name.</param>
    /// <param name="result">The enumeration value if found.</param>
    /// <returns>True if the value was found; otherwise, false.</returns>
    public static bool TryFromDisplayName<T>(string name, out T? result) where T : Enumeration
    {
        result = GetAll<T>().FirstOrDefault(item => item.Name == name);
        return result != null;
    }

    private static T Parse<T, TValue>(TValue value, string description, Func<T, bool> predicate)
        where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
        {
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
        }

        return matchingItem;
    }

    /// <summary>
    /// Compares the current instance with another object.
    /// </summary>
    public int CompareTo(object? obj)
    {
        if (obj is not Enumeration other)
        {
            throw new ArgumentException($"Object must be of type {nameof(Enumeration)}");
        }

        return Id.CompareTo(other.Id);
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Enumeration? left, Enumeration? right)
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
    public static bool operator !=(Enumeration? left, Enumeration? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Less than operator.
    /// </summary>
    public static bool operator <(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Less than or equal operator.
    /// </summary>
    public static bool operator <=(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Greater than or equal operator.
    /// </summary>
    public static bool operator >=(Enumeration left, Enumeration right)
    {
        return left.CompareTo(right) >= 0;
    }
}
