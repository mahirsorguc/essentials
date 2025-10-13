using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.UnitOfWork.InMemoryDummy;

[DependsOn(typeof(EssentialsUnitOfWorkModule))]
public class EssentialsUnitOfWorkInMemoryDummyModule : EssentialsModule
{
    /// <inheritdoc/>
    public override void ConfigureServices(ModuleContext context)
    {
        // Register Unit of Work implementation
        context.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        Console.WriteLine("[UnitOfWorkModule] Services configured.");
    }

    /// <inheritdoc/>
    public override void Initialize(ModuleContext context)
    {
        Console.WriteLine("[UnitOfWorkModule] Unit of Work pattern initialized.");
        Console.WriteLine("[UnitOfWorkModule] Features:");
        Console.WriteLine("  - Transaction Management (Begin/Commit/Rollback)");
        Console.WriteLine("  - SaveChanges with cancellation support");
        Console.WriteLine("  - In-Memory implementation for testing");
        Console.WriteLine("  - Extensible for EF Core, Dapper, or other ORMs");
    }

    /// <inheritdoc/>
    public override void Shutdown(ModuleContext context)
    {
        Console.WriteLine("[UnitOfWorkModule] Shutting down...");
    }
}