using FluentValidation;

namespace HMS.Essentials.FluentValidation.Validators;

/// <summary>
/// Abstract base class for validators providing common validation functionality.
/// Includes helper methods for common validation scenarios.
/// </summary>
/// <typeparam name="T">The type being validated.</typeparam>
public abstract class ValidatorBase<T> : AbstractValidator<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatorBase{T}"/> class.
    /// </summary>
    protected ValidatorBase()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
    }

    /// <summary>
    /// Validates that a string is not null, empty, or whitespace.
    /// </summary>
    protected static bool BeNonEmptyString(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Validates that a GUID is not empty.
    /// </summary>
    protected static bool BeNonEmptyGuid(Guid value)
    {
        return value != Guid.Empty;
    }

    /// <summary>
    /// Validates that a value is within a specified range.
    /// </summary>
    protected static bool BeInRange<TValue>(TValue value, TValue min, TValue max) 
        where TValue : IComparable<TValue>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }

    /// <summary>
    /// Validates that a date is not in the past.
    /// </summary>
    protected static bool BeNotInPast(DateTime date)
    {
        return date.Date >= DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Validates that a date is in the past.
    /// </summary>
    protected static bool BeInPast(DateTime date)
    {
        return date.Date < DateTime.UtcNow.Date;
    }

    /// <summary>
    /// Validates that a collection has items.
    /// </summary>
    protected static bool HaveItems<TItem>(IEnumerable<TItem>? collection)
    {
        return collection?.Any() == true;
    }

    /// <summary>
    /// Validates that a collection has no duplicate items.
    /// </summary>
    protected static bool HaveNoDuplicates<TItem>(IEnumerable<TItem>? collection)
    {
        if (collection == null)
        {
            return true;
        }

        var list = collection.ToList();
        return list.Count == list.Distinct().Count();
    }

    /// <summary>
    /// Validates that a string matches a specific pattern (regex).
    /// </summary>
    protected static bool MatchPattern(string? value, string pattern)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
    }
}
