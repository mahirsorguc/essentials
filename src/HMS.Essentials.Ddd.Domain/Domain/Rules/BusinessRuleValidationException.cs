namespace HMS.Essentials.Domain.Rules;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public class BusinessRuleValidationException : Exception
{
    /// <summary>
    /// Gets the business rule that was violated.
    /// </summary>
    public IBusinessRule BrokenRule { get; }

    /// <summary>
    /// Gets additional details about the validation failure.
    /// </summary>
    public string Details { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleValidationException"/> class.
    /// </summary>
    /// <param name="brokenRule">The business rule that was violated.</param>
    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleValidationException"/> class.
    /// </summary>
    /// <param name="brokenRule">The business rule that was violated.</param>
    /// <param name="details">Additional details about the validation failure.</param>
    public BusinessRuleValidationException(IBusinessRule brokenRule, string details)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = details;
    }

    /// <summary>
    /// Returns a string representation of the exception.
    /// </summary>
    public override string ToString()
    {
        return $"{BrokenRule.GetType().Name}: {BrokenRule.Message}";
    }
}
