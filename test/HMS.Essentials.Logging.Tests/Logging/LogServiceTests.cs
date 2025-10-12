using HMS.Essential.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace HMS.Essentials.Logging;

public class LogServiceTests
{
    private readonly Mock<ILogger<LogService>> _mockLogger;
    private readonly LogService _logService;

    public LogServiceTests()
    {
        _mockLogger = new Mock<ILogger<LogService>>();
        _logService = new LogService(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Logger_Is_Null()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new LogService(null!));
    }

    [Fact]
    public void LogInfo_Should_Call_Logger_LogInformation()
    {
        // Arrange
        const string message = "Test info message";

        // Act
        _logService.LogInfo(message);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogWarning_Should_Call_Logger_LogWarning()
    {
        // Arrange
        const string message = "Test warning message";

        // Act
        _logService.LogWarning(message);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_Without_Exception_Should_Call_Logger_LogError()
    {
        // Arrange
        const string message = "Test error message";

        // Act
        _logService.LogError(message);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogError_With_Exception_Should_Call_Logger_LogError_With_Exception()
    {
        // Arrange
        const string message = "Test error message";
        var exception = new InvalidOperationException("Test exception");

        // Act
        _logService.LogError(message, exception);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogInfo_Should_Handle_Empty_String()
    {
        // Act & Assert
        Should.NotThrow(() => _logService.LogInfo(string.Empty));
    }

    [Fact]
    public void LogWarning_Should_Handle_Empty_String()
    {
        // Act & Assert
        Should.NotThrow(() => _logService.LogWarning(string.Empty));
    }

    [Fact]
    public void LogError_Should_Handle_Empty_String()
    {
        // Act & Assert
        Should.NotThrow(() => _logService.LogError(string.Empty));
    }

    [Fact]
    public void LogInfo_Should_Handle_Null_String()
    {
        // Act & Assert
        Should.NotThrow(() => _logService.LogInfo(null!));
    }

    [Fact]
    public void LogWarning_Should_Handle_Null_String()
    {
        // Act & Assert
        Should.NotThrow(() => _logService.LogWarning(null!));
    }

    [Fact]
    public void LogError_Should_Handle_Null_String()
    {
        // Act & Assert
        Should.NotThrow(() => _logService.LogError(null!));
    }

    [Fact]
    public void Multiple_LogInfo_Calls_Should_All_Be_Logged()
    {
        // Arrange
        const string message1 = "Message 1";
        const string message2 = "Message 2";
        const string message3 = "Message 3";

        // Act
        _logService.LogInfo(message1);
        _logService.LogInfo(message2);
        _logService.LogInfo(message3);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(3));
    }

    [Fact]
    public void Should_Be_Able_To_Log_Different_Levels()
    {
        // Arrange
        const string infoMsg = "Info";
        const string warnMsg = "Warning";
        const string errorMsg = "Error";

        // Act
        _logService.LogInfo(infoMsg);
        _logService.LogWarning(warnMsg);
        _logService.LogError(errorMsg);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
