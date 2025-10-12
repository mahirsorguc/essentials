using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity;

/// <summary>
/// Represents the host for a modular application.
/// Manages the application lifecycle and provides access to services.
/// </summary>
public sealed class ApplicationHost : IDisposable, IAsyncDisposable
{
    private readonly ModuleLifecycleManager _lifecycleManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ModuleContext _context;
    private bool _disposed;

    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider Services => _serviceProvider;

    /// <summary>
    /// Gets the module context.
    /// </summary>
    public ModuleContext Context => _context;

    /// <summary>
    /// Gets the collection of loaded modules.
    /// </summary>
    public IReadOnlyList<ModuleDescriptor> Modules => _lifecycleManager.Modules;

    internal ApplicationHost(
        ModuleLifecycleManager lifecycleManager,
        IServiceProvider serviceProvider,
        ModuleContext context)
    {
        _lifecycleManager = lifecycleManager ?? throw new ArgumentNullException(nameof(lifecycleManager));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a required service from the service provider.
    /// </summary>
    public T GetRequiredService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service from the service provider.
    /// </summary>
    public T? GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    /// <summary>
    /// Creates a new service scope.
    /// </summary>
    public IServiceScope CreateScope()
    {
        return _serviceProvider.CreateScope();
    }

    /// <summary>
    /// Runs the application until it's stopped.
    /// </summary>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        
        cancellationToken.Register(() => tcs.TrySetResult(true));

        await tcs.Task;
    }

    /// <summary>
    /// Disposes the application host and shuts down all modules.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _lifecycleManager.Shutdown();

        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Asynchronously disposes the application host and shuts down all modules.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _lifecycleManager.Shutdown();

        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _disposed = true;
    }
}
