namespace HMS.Essentials.Modularity.Exceptions;

/// <summary>
/// Exception thrown when module initialization fails.
/// </summary>
public class ModuleInitializationException : ModuleException
{
    public ModuleInitializationException(string message, Type moduleType)
        : base(message, moduleType)
    {
    }

    public ModuleInitializationException(string message, Type moduleType, Exception innerException)
        : base(message, moduleType, innerException)
    {
    }
}
