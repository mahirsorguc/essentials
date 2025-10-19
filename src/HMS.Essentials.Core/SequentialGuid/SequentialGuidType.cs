namespace HMS.Essentials.SequentialGuid;

/// <summary>
/// Describes the type of sequential GUID to generate.
/// Different database systems store GUIDs differently, so the byte order matters.
/// </summary>
public enum SequentialGuidType
{
    /// <summary>
    /// Sequential at the end (suitable for SQL Server).
    /// SQL Server stores GUIDs using a specific byte order where the last 6 bytes are most significant for indexing.
    /// </summary>
    SequentialAtEnd,

    /// <summary>
    /// Sequential as string (suitable for MySQL).
    /// MySQL stores GUIDs as strings, so we need the sequential part at the beginning.
    /// </summary>
    SequentialAsString,

    /// <summary>
    /// Sequential as binary (suitable for PostgreSQL and Oracle).
    /// PostgreSQL stores GUIDs as 16-byte binary values, so we need the sequential part at the beginning.
    /// </summary>
    SequentialAsBinary
}
