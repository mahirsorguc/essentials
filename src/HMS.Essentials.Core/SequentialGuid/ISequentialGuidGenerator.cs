namespace HMS.Essentials.SequentialGuid;

/// <summary>
/// Service for generating sequential GUIDs optimized for database indexing.
/// Sequential GUIDs reduce index fragmentation and improve insert performance.
/// </summary>
public interface ISequentialGuidGenerator
{
    /// <summary>
    /// Creates a new sequential GUID using the default type (SequentialAtEnd for SQL Server).
    /// </summary>
    /// <returns>A new sequential GUID.</returns>
    Guid Create();

    /// <summary>
    /// Creates a new sequential GUID of the specified type.
    /// </summary>
    /// <param name="guidType">The type of sequential GUID to generate based on the target database.</param>
    /// <returns>A new sequential GUID optimized for the specified database type.</returns>
    Guid Create(SequentialGuidType guidType);
}
