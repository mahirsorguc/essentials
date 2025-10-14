namespace HMS.Essentials.EntityFrameworkCore.Configuration;

/// <summary>
/// Provides connection string information for database contexts.
/// </summary>
public interface IConnectionStringProvider
{
    /// <summary>
    /// Gets the connection string for a specific database context.
    /// </summary>
    /// <param name="contextName">The name of the database context.</param>
    /// <returns>The connection string or null if not found.</returns>
    string? GetConnectionString(string? contextName = null);
}

/// <summary>
/// Default implementation of connection string provider using configuration.
/// </summary>
public class DefaultConnectionStringProvider : IConnectionStringProvider
{
    private readonly Dictionary<string, string> _connectionStrings;
    private readonly string? _defaultConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultConnectionStringProvider"/> class.
    /// </summary>
    /// <param name="defaultConnectionString">The default connection string.</param>
    public DefaultConnectionStringProvider(string? defaultConnectionString = null)
    {
        _defaultConnectionString = defaultConnectionString;
        _connectionStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Adds or updates a connection string for a specific context.
    /// </summary>
    /// <param name="contextName">The name of the database context.</param>
    /// <param name="connectionString">The connection string.</param>
    public void AddConnectionString(string contextName, string connectionString)
    {
        _connectionStrings[contextName] = connectionString;
    }

    /// <inheritdoc/>
    public string? GetConnectionString(string? contextName = null)
    {
        if (string.IsNullOrWhiteSpace(contextName))
        {
            return _defaultConnectionString;
        }

        return _connectionStrings.TryGetValue(contextName, out var connectionString)
            ? connectionString
            : _defaultConnectionString;
    }
}
