namespace HMS.Essentials.EntityFrameworkCore.Configuration;

/// <summary>
/// Options for Entity Framework Core configuration.
/// </summary>
public class EfCoreOptions
{
    /// <summary>
    /// Gets or sets the default connection string.
    /// </summary>
    public string? DefaultConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the connection strings mapped by context name.
    /// </summary>
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to enable detailed errors.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging.
    /// Caution: This may log sensitive information. Use only in development.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum retry count for database operations.
    /// </summary>
    public int MaxRetryCount { get; set; } = 6;

    /// <summary>
    /// Gets or sets the maximum delay between retries in seconds.
    /// </summary>
    public int MaxRetryDelay { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether to use lazy loading proxies.
    /// </summary>
    public bool UseLazyLoadingProxies { get; set; } = false;

    /// <summary>
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    public int? CommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically apply migrations on startup.
    /// </summary>
    public bool AutoMigrateOnStartup { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to seed initial data on startup.
    /// </summary>
    public bool SeedDataOnStartup { get; set; } = false;
}
