using FluentValidation.Results;

namespace HMS.Essentials.FluentValidation.Extensions;

/// <summary>
/// Extension methods for FluentValidation ValidationResult.
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    /// Converts validation errors to a dictionary grouped by property name.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>Dictionary with property names as keys and error messages as values.</returns>
    public static Dictionary<string, List<string>> ToDictionary(this ValidationResult validationResult)
    {
        ArgumentNullException.ThrowIfNull(validationResult);

        return validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToList());
    }

    /// <summary>
    /// Gets all error messages as a single formatted string.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <param name="separator">Separator between error messages (default: newline).</param>
    /// <returns>Formatted error message string.</returns>
    public static string ToErrorString(this ValidationResult validationResult, string separator = "\n")
    {
        ArgumentNullException.ThrowIfNull(validationResult);

        return string.Join(separator, validationResult.Errors.Select(e => e.ErrorMessage));
    }

    /// <summary>
    /// Gets errors for a specific property.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>List of error messages for the property.</returns>
    public static List<string> GetErrorsForProperty(this ValidationResult validationResult, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(validationResult);
        ArgumentNullException.ThrowIfNull(propertyName);

        return validationResult.Errors
            .Where(e => e.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.ErrorMessage)
            .ToList();
    }

    /// <summary>
    /// Checks if a specific property has errors.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>True if property has errors; otherwise false.</returns>
    public static bool HasErrorsForProperty(this ValidationResult validationResult, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(validationResult);
        ArgumentNullException.ThrowIfNull(propertyName);

        return validationResult.Errors
            .Any(e => e.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Converts validation result to a JSON-friendly object.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>Anonymous object with validation details.</returns>
    public static object ToJsonObject(this ValidationResult validationResult)
    {
        ArgumentNullException.ThrowIfNull(validationResult);

        return new
        {
            IsValid = validationResult.IsValid,
            Errors = validationResult.Errors.Select(e => new
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage,
                AttemptedValue = e.AttemptedValue,
                ErrorCode = e.ErrorCode,
                Severity = e.Severity.ToString()
            }).ToList()
        };
    }
}
