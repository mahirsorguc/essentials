namespace HMS.Essentials.MediatR;

/// <summary>
/// Attribute to enable Unit of Work transaction management for commands.
/// When applied to a command, the UnitOfWorkBehavior will automatically wrap
/// the command execution in a transaction.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class UnitOfWorkAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether the transaction is enabled.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to automatically commit the transaction
    /// after successful command execution. Default is true.
    /// </summary>
    public bool AutoCommit { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to automatically rollback the transaction
    /// on exception. Default is true.
    /// </summary>
    public bool AutoRollback { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkAttribute"/> class.
    /// </summary>
    public UnitOfWorkAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkAttribute"/> class.
    /// </summary>
    /// <param name="isEnabled">Whether the unit of work is enabled.</param>
    public UnitOfWorkAttribute(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}
