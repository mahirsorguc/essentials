using HMS.Essential.Logging;
using Moq;
using Shouldly;

namespace HMS.Essentials.Data;

public class DataRepositoryTests
{
    private readonly Mock<ILogService> _mockLogService;
    private readonly DataRepository _repository;

    public DataRepositoryTests()
    {
        _mockLogService = new Mock<ILogService>();
        _repository = new DataRepository(_mockLogService.Object);
    }

    [Fact]
    public void Constructor_Should_Throw_When_LogService_Is_Null()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new DataRepository(null!));
    }

    [Fact]
    public void Constructor_Should_Log_Initialization()
    {
        // Arrange & Act
        var logService = new Mock<ILogService>();
        var repository = new DataRepository(logService.Object);

        // Assert
        logService.Verify(l => l.LogInfo("DataRepository initialized."), Times.Once);
    }

    [Fact]
    public async Task GetDataAsync_Should_Return_Empty_Collection_Initially()
    {
        // Act
        var result = await _repository.GetDataAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetDataAsync_Should_Log_Retrieval()
    {
        // Act
        await _repository.GetDataAsync();

        // Assert
        _mockLogService.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Retrieving") && s.Contains("items"))), Times.Once);
    }

    [Fact]
    public async Task SaveDataAsync_Should_Add_Data_To_Repository()
    {
        // Arrange
        const string testData = "Test Data";

        // Act
        await _repository.SaveDataAsync(testData);
        var result = await _repository.GetDataAsync();

        // Assert
        result.ShouldContain(testData);
        result.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SaveDataAsync_Should_Log_Save_Operation()
    {
        // Arrange
        const string testData = "Test Data";

        // Act
        await _repository.SaveDataAsync(testData);

        // Assert
        _mockLogService.Verify(l => l.LogInfo($"Saved data: {testData}"), Times.Once);
    }

    [Fact]
    public async Task SaveDataAsync_Should_Throw_When_Data_Is_Null()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => _repository.SaveDataAsync(null!));
    }

    [Fact]
    public async Task SaveDataAsync_Should_Throw_When_Data_Is_Empty()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => _repository.SaveDataAsync(string.Empty));
    }

    [Fact]
    public async Task SaveDataAsync_Should_Throw_When_Data_Is_Whitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => _repository.SaveDataAsync("   "));
    }

    [Fact]
    public async Task SaveDataAsync_Should_Add_Multiple_Items()
    {
        // Arrange
        const string data1 = "Data 1";
        const string data2 = "Data 2";
        const string data3 = "Data 3";

        // Act
        await _repository.SaveDataAsync(data1);
        await _repository.SaveDataAsync(data2);
        await _repository.SaveDataAsync(data3);
        var result = await _repository.GetDataAsync();

        // Assert
        result.Count().ShouldBe(3);
        result.ShouldContain(data1);
        result.ShouldContain(data2);
        result.ShouldContain(data3);
    }

    [Fact]
    public async Task GetDataAsync_Should_Return_All_Saved_Items()
    {
        // Arrange
        var testData = new[] { "Item1", "Item2", "Item3", "Item4" };

        // Act
        foreach (var data in testData)
        {
            await _repository.SaveDataAsync(data);
        }
        var result = await _repository.GetDataAsync();

        // Assert
        result.Count().ShouldBe(testData.Length);
        foreach (var data in testData)
        {
            result.ShouldContain(data);
        }
    }

    [Fact]
    public async Task Repository_Should_Maintain_Order_Of_Items()
    {
        // Arrange
        var testData = new[] { "First", "Second", "Third" };

        // Act
        foreach (var data in testData)
        {
            await _repository.SaveDataAsync(data);
        }
        var result = (await _repository.GetDataAsync()).ToArray();

        // Assert
        result[0].ShouldBe("First");
        result[1].ShouldBe("Second");
        result[2].ShouldBe("Third");
    }

    [Fact]
    public async Task SaveDataAsync_Should_Return_CompletedTask()
    {
        // Arrange
        const string testData = "Test";

        // Act
        var task = _repository.SaveDataAsync(testData);

        // Assert
        task.ShouldNotBeNull();
        task.IsCompleted.ShouldBeTrue();
        
        // Ensure task completes without errors
        await task;
    }

    [Fact]
    public async Task GetDataAsync_Should_Return_CompletedTask()
    {
        // Act
        var task = _repository.GetDataAsync();

        // Assert
        task.ShouldNotBeNull();
        task.IsCompleted.ShouldBeTrue();
        
        // Ensure task completes without errors
        await task;
    }
}
