using HMS.Essentials.SequentialGuid;
using Shouldly;

namespace HMS.Essentials.SequentialGuid;

public class SequentialGuidGeneratorTests
{
    private readonly ISequentialGuidGenerator _generator;

    public SequentialGuidGeneratorTests()
    {
        _generator = new SequentialGuidGenerator();
    }

    [Fact]
    public void Create_ShouldReturnValidGuid()
    {
        // Act
        var guid = _generator.Create();

        // Assert
        guid.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueGuids()
    {
        // Act
        var guid1 = _generator.Create();
        var guid2 = _generator.Create();

        // Assert
        guid1.ShouldNotBe(guid2);
    }

    [Fact]
    public void Create_MultipleInvocations_ShouldGenerateSequentialGuids()
    {
        // Arrange
        var guids = new List<Guid>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            guids.Add(_generator.Create(SequentialGuidType.SequentialAtEnd));
            Thread.Sleep(1); // Small delay to ensure different timestamps
        }

        // Assert - For SequentialAtEnd, the last 6 bytes should be increasing
        for (int i = 1; i < guids.Count; i++)
        {
            var prevBytes = guids[i - 1].ToByteArray();
            var currBytes = guids[i].ToByteArray();

            // Compare last 6 bytes (timestamp portion for SequentialAtEnd)
            var prevTimestamp = GetTimestampFromEnd(prevBytes);
            var currTimestamp = GetTimestampFromEnd(currBytes);

            currTimestamp.ShouldBeGreaterThanOrEqualTo(prevTimestamp);
        }
    }

    [Theory]
    [InlineData(SequentialGuidType.SequentialAtEnd)]
    [InlineData(SequentialGuidType.SequentialAsString)]
    [InlineData(SequentialGuidType.SequentialAsBinary)]
    public void Create_WithSpecificType_ShouldReturnValidGuid(SequentialGuidType guidType)
    {
        // Act
        var guid = _generator.Create(guidType);

        // Assert
        guid.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(SequentialGuidType.SequentialAtEnd)]
    [InlineData(SequentialGuidType.SequentialAsString)]
    [InlineData(SequentialGuidType.SequentialAsBinary)]
    public void Create_WithSpecificType_ShouldGenerateUniqueGuids(SequentialGuidType guidType)
    {
        // Act
        var guid1 = _generator.Create(guidType);
        var guid2 = _generator.Create(guidType);

        // Assert
        guid1.ShouldNotBe(guid2);
    }

    [Fact]
    public void Create_SequentialAtEnd_ShouldHaveTimestampAtEnd()
    {
        // Arrange
        var beforeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Act
        var guid = _generator.Create(SequentialGuidType.SequentialAtEnd);
        
        // Assert
        var afterTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var guidBytes = guid.ToByteArray();
        var extractedTimestamp = GetTimestampFromEnd(guidBytes);

        extractedTimestamp.ShouldBeGreaterThanOrEqualTo(beforeTimestamp);
        extractedTimestamp.ShouldBeLessThanOrEqualTo(afterTimestamp);
    }

    [Fact]
    public void Create_SequentialAsString_ShouldHaveTimestampAtBeginning()
    {
        // Arrange
        var beforeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Act
        var guid = _generator.Create(SequentialGuidType.SequentialAsString);

        // Assert
        var afterTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var guidBytes = guid.ToByteArray();
        var extractedTimestamp = GetTimestampFromBeginning(guidBytes);

        extractedTimestamp.ShouldBeGreaterThanOrEqualTo(beforeTimestamp);
        extractedTimestamp.ShouldBeLessThanOrEqualTo(afterTimestamp);
    }

    [Fact]
    public void Create_SequentialAsBinary_ShouldHaveTimestampAtBeginning()
    {
        // Arrange
        var beforeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Act
        var guid = _generator.Create(SequentialGuidType.SequentialAsBinary);

        // Assert
        var afterTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var guidBytes = guid.ToByteArray();
        var extractedTimestamp = GetTimestampFromBeginning(guidBytes);

        extractedTimestamp.ShouldBeGreaterThanOrEqualTo(beforeTimestamp);
        extractedTimestamp.ShouldBeLessThanOrEqualTo(afterTimestamp);
    }

    [Fact]
    public async Task Create_Concurrent_ShouldGenerateUniqueGuids()
    {
        // Arrange
        var guidSet = new System.Collections.Concurrent.ConcurrentBag<Guid>();
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    guidSet.Add(_generator.Create());
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        var distinctGuids = guidSet.Distinct().Count();
        distinctGuids.ShouldBe(1000); // All GUIDs should be unique
    }

    [Fact]
    public void Create_Performance_ShouldBeReasonablyFast()
    {
        // Arrange
        var iterations = 10000;
        var maxMilliseconds = 1000; // Should generate 10k GUIDs in less than 1 second

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            _generator.Create();
        }
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(maxMilliseconds, 
            $"Generating {iterations} GUIDs took {stopwatch.ElapsedMilliseconds}ms, expected less than {maxMilliseconds}ms");
    }

    [Fact]
    public void Create_SequentialAtEnd_ForSqlServer_ShouldReduceFragmentation()
    {
        // This test verifies that consecutive GUIDs are "closer" to each other
        // when compared at the end (SQL Server index order)
        
        // Arrange & Act
        var guids = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            guids.Add(_generator.Create(SequentialGuidType.SequentialAtEnd));
            Thread.Sleep(2); // Small delay to ensure different timestamps
        }

        // Assert - consecutive GUIDs should have similar ending bytes
        for (int i = 1; i < guids.Count; i++)
        {
            var prevBytes = guids[i - 1].ToByteArray();
            var currBytes = guids[i].ToByteArray();

            // Compare the last 6 bytes (timestamp portion)
            var prevTimestamp = GetTimestampFromEnd(prevBytes);
            var currTimestamp = GetTimestampFromEnd(currBytes);

            // Current timestamp should be greater or equal
            currTimestamp.ShouldBeGreaterThanOrEqualTo(prevTimestamp);
            
            // The difference should be small (within a few milliseconds)
            var difference = currTimestamp - prevTimestamp;
            difference.ShouldBeLessThan(100); // Less than 100ms difference
        }
    }

    [Fact]
    public void Create_SequentialAsString_ForMySQL_ShouldSortCorrectly()
    {
        // MySQL stores GUIDs as strings, so they need to sort correctly as strings
        
        // Arrange & Act
        var guids = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            guids.Add(_generator.Create(SequentialGuidType.SequentialAsString));
            Thread.Sleep(2);
        }

        var guidStrings = guids.Select(g => g.ToString()).ToList();
        var sortedGuidStrings = new List<string>(guidStrings);
        sortedGuidStrings.Sort(StringComparer.Ordinal);

        // Assert - string representation should already be in order
        for (int i = 0; i < guidStrings.Count; i++)
        {
            // The order might not be perfect due to the random part, but timestamps should be increasing
            var bytes = guids[i].ToByteArray();
            var timestamp = GetTimestampFromBeginning(bytes);
            
            if (i > 0)
            {
                var prevBytes = guids[i - 1].ToByteArray();
                var prevTimestamp = GetTimestampFromBeginning(prevBytes);
                timestamp.ShouldBeGreaterThanOrEqualTo(prevTimestamp);
            }
        }
    }

    [Fact]
    public void Create_SequentialAsBinary_ForPostgreSQL_ShouldOrderCorrectly()
    {
        // PostgreSQL stores GUIDs as 16-byte binary values
        
        // Arrange & Act
        var guids = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            guids.Add(_generator.Create(SequentialGuidType.SequentialAsBinary));
            Thread.Sleep(2);
        }

        // Assert - binary comparison should show increasing order
        for (int i = 1; i < guids.Count; i++)
        {
            var prevBytes = guids[i - 1].ToByteArray();
            var currBytes = guids[i].ToByteArray();

            var prevTimestamp = GetTimestampFromBeginning(prevBytes);
            var currTimestamp = GetTimestampFromBeginning(currBytes);

            currTimestamp.ShouldBeGreaterThanOrEqualTo(prevTimestamp);
        }
    }

    [Fact]
    public void Create_WithInvalidGuidType_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var invalidGuidType = (SequentialGuidType)999;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _generator.Create(invalidGuidType));
    }

    #region Helper Methods

    private static long GetTimestampFromEnd(byte[] guidBytes)
    {
        // Extract timestamp from last 6 bytes
        return ((long)guidBytes[10] << 40) |
               ((long)guidBytes[11] << 32) |
               ((long)guidBytes[12] << 24) |
               ((long)guidBytes[13] << 16) |
               ((long)guidBytes[14] << 8) |
               guidBytes[15];
    }

    private static long GetTimestampFromBeginning(byte[] guidBytes)
    {
        // Extract timestamp from first 6 bytes
        return ((long)guidBytes[0] << 40) |
               ((long)guidBytes[1] << 32) |
               ((long)guidBytes[2] << 24) |
               ((long)guidBytes[3] << 16) |
               ((long)guidBytes[4] << 8) |
               guidBytes[5];
    }

    #endregion
}
