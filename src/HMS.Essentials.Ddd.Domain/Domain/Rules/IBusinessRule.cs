namespace HMS.Essentials.Domain.Rules;

/// <summary>
/// Represents a business rule that can be validated.
/// Business rules encapsulate domain invariants and constraints.
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    /// Gets the error message that describes why the rule is broken.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Checks whether the business rule is broken.
    /// </summary>
    /// <returns>True if the rule is broken, false otherwise.</returns>
    bool IsBroken();
}
