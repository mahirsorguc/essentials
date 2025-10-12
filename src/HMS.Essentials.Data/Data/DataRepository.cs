using HMS.Essential.Logging;

namespace HMS.Essentials.Data;

/// <summary>
/// Repository interface for data operations.
/// </summary>
public interface IDataRepository
{
    Task<IEnumerable<string>> GetDataAsync();
    Task SaveDataAsync(string data);
}

/// <summary>
/// Simple in-memory implementation of the data repository.
/// </summary>
public class DataRepository : IDataRepository
{
    private readonly List<string> _data = new();
    private readonly ILogService _logService;

    public DataRepository(ILogService logService)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _logService.LogInfo("DataRepository initialized.");
    }

    public Task<IEnumerable<string>> GetDataAsync()
    {
        _logService.LogInfo($"Retrieving {_data.Count} items from repository.");
        return Task.FromResult<IEnumerable<string>>(_data);
    }

    public Task SaveDataAsync(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Data cannot be null or empty.", nameof(data));

        _data.Add(data);
        _logService.LogInfo($"Saved data: {data}");
        return Task.CompletedTask;
    }
}
