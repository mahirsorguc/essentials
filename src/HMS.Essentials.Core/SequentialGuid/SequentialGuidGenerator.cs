using System.Security.Cryptography;

namespace HMS.Essentials.SequentialGuid;

/// <summary>
/// Default implementation of ISequentialGuidGenerator.
/// Generates sequential GUIDs that are optimized for database indexing and reduce fragmentation.
/// </summary>
public class SequentialGuidGenerator : ISequentialGuidGenerator
{
    /// <summary>
    /// The default sequential GUID type to use (SQL Server compatible).
    /// </summary>
    private const SequentialGuidType DefaultGuidType = SequentialGuidType.SequentialAtEnd;

    /// <summary>
    /// Creates a new sequential GUID using the default type (SequentialAtEnd for SQL Server).
    /// </summary>
    /// <returns>A new sequential GUID.</returns>
    public Guid Create()
    {
        return Create(DefaultGuidType);
    }

    /// <summary>
    /// Creates a new sequential GUID of the specified type.
    /// The implementation uses the current UTC timestamp to ensure sequential ordering.
    /// </summary>
    /// <param name="guidType">The type of sequential GUID to generate based on the target database.</param>
    /// <returns>A new sequential GUID optimized for the specified database type.</returns>
    public Guid Create(SequentialGuidType guidType)
    {
        // Generate 10 random bytes for the random portion
        var randomBytes = new byte[10];
        RandomNumberGenerator.Fill(randomBytes);

        // Get timestamp as Unix time in milliseconds
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        // Convert timestamp to bytes (6 bytes, 48 bits is enough for ~8900 years from epoch)
        var timestampBytes = new byte[6];
        timestampBytes[0] = (byte)(timestamp >> 40);
        timestampBytes[1] = (byte)(timestamp >> 32);
        timestampBytes[2] = (byte)(timestamp >> 24);
        timestampBytes[3] = (byte)(timestamp >> 16);
        timestampBytes[4] = (byte)(timestamp >> 8);
        timestampBytes[5] = (byte)timestamp;

        // Create the GUID bytes based on the requested type
        var guidBytes = new byte[16];

        switch (guidType)
        {
            case SequentialGuidType.SequentialAsString:
            case SequentialGuidType.SequentialAsBinary:
                // For string and binary, put timestamp at the beginning
                Buffer.BlockCopy(timestampBytes, 0, guidBytes, 0, 6);
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);
                
                // For string representation, ensure proper UUID format
                if (guidType == SequentialGuidType.SequentialAsString)
                {
                    // Set version (4) and variant (RFC 4122) bits
                    guidBytes[7] = (byte)((guidBytes[7] & 0x0F) | 0x40); // Version 4
                    guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80); // Variant RFC 4122
                }
                break;

            case SequentialGuidType.SequentialAtEnd:
                // For SQL Server, put timestamp at the end
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                Buffer.BlockCopy(timestampBytes, 0, guidBytes, 10, 6);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(guidType), guidType, "Invalid sequential GUID type.");
        }

        return new Guid(guidBytes);
    }
}
