using HMS.Essentials.Domain.Events;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Domain;

[DependsOn(
    typeof(EssentialsDomainSharedModule),
    typeof(EssentialsDomainEventsModule)
    )]
public class EssentialsDomainModule : EssentialsModule
{
    /// <inheritdoc />
    public override void ConfigureServices(ModuleContext context)
    {
        // Note: Repository registrations should be done in the consuming application
        // as they are typically entity-specific. The module provides the infrastructure.

        Console.WriteLine("[RepositoryModule] Services configured.");
    }

    /// <inheritdoc />
    public override void Initialize(ModuleContext context)
    {
        Console.WriteLine("[RepositoryModule] Advanced repository pattern initialized.");
        Console.WriteLine("[RepositoryModule] Features:");
        Console.WriteLine("  - Generic Repository Pattern (IRepository<TEntity, TKey>)");
        Console.WriteLine("  - Read-Only Repository (IReadOnlyRepository<TEntity, TKey>)");
        Console.WriteLine("  - Specification Pattern (ISpecification<T>)");
        Console.WriteLine("  - In-Memory Implementation for testing");
        Console.WriteLine("  - Support for custom key types (int, long, Guid, etc.)");
    }

    /// <inheritdoc />
    public override void Shutdown(ModuleContext context)
    {
        Console.WriteLine("[RepositoryModule] Shutting down...");
    }
}