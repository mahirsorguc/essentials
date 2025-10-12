namespace HMS.Essentials.Modularity.Exceptions;

/// <summary>
/// Base exception for all module-related errors.
/// </summary>
public class ModuleException : Exception
{
    /// <summary>
    /// Gets the module type that caused the exception.
    /// </summary>
    public Type? ModuleType { get; }

    public ModuleException(string message) : base(message)
    {
    }

    public ModuleException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ModuleException(string message, Type? moduleType) : base(message)
    {
        ModuleType = moduleType;
    }

    public ModuleException(string message, Type? moduleType, Exception innerException) 
        : base(message, innerException)
    {
        ModuleType = moduleType;
    }
}
