namespace HMS.Essentials.AspNetCore.Serilog.Configuration;

/// <summary>
/// Configuration options for Serilog integration.
/// </summary>
public class SerilogOptions
{
    /// <summary>
    /// Gets or sets whether Serilog is enabled.
    /// Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum log level.
    /// Default is "Information".
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// Gets or sets whether to write logs to console.
    /// Default is true.
    /// </summary>
    public bool WriteToConsole { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to write logs to file.
    /// Default is true.
    /// </summary>
    public bool WriteToFile { get; set; } = true;

    /// <summary>
    /// Gets or sets the log file path.
    /// Default is "Logs/log-.txt".
    /// </summary>
    public string LogFilePath { get; set; } = "Logs/log-.txt";

    /// <summary>
    /// Gets or sets the log file rolling interval (Day, Hour, Minute, etc.).
    /// Default is "Day".
    /// </summary>
    public string RollingInterval { get; set; } = "Day";

    /// <summary>
    /// Gets or sets the number of log files to retain.
    /// Default is 31 days.
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// Gets or sets the file size limit in bytes.
    /// Default is 10MB (10485760 bytes).
    /// </summary>
    public long? FileSizeLimitBytes { get; set; } = 10485760;

    /// <summary>
    /// Gets or sets whether to enrich logs with machine name.
    /// Default is true.
    /// </summary>
    public bool EnrichWithMachineName { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enrich logs with thread ID.
    /// Default is true.
    /// </summary>
    public bool EnrichWithThreadId { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enrich logs with environment name.
    /// Default is true.
    /// </summary>
    public bool EnrichWithEnvironmentName { get; set; } = true;

    /// <summary>
    /// Gets or sets the output template for console logs.
    /// </summary>
    public string? ConsoleOutputTemplate { get; set; }

    /// <summary>
    /// Gets or sets the output template for file logs.
    /// </summary>
    public string? FileOutputTemplate { get; set; }

    /// <summary>
    /// Gets or sets whether to override the default ASP.NET Core logging.
    /// Default is true.
    /// </summary>
    public bool OverrideMinimumLevel { get; set; } = true;

    /// <summary>
    /// Gets or sets custom minimum level overrides for specific namespaces.
    /// Example: { "Microsoft.AspNetCore": "Warning", "System": "Warning" }
    /// </summary>
    public Dictionary<string, string> MinimumLevelOverrides { get; set; } = new()
    {
        ["Microsoft.AspNetCore"] = "Warning",
        ["Microsoft.EntityFrameworkCore"] = "Warning",
        ["System"] = "Warning"
    };
}
