using HMS.Essentials.Domain.UnitOfWork;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Domain;

[DependsOn(typeof(EssentialsDomainSharedModule))]
public class EssentialsDomainModule : EssentialsModule
{
    /// <inheritdoc/>
    public override void ConfigureServices(ModuleContext context)
    {
        // Register Unit of Work
        context.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        // Note: Repository registrations should be done in the consuming application
        // as they are typically entity-specific. The module provides the infrastructure.

        Console.WriteLine("[RepositoryModule] Services configured.");
    }

    /// <inheritdoc/>
    public override void Initialize(ModuleContext context)
    {
        Console.WriteLine("[RepositoryModule] Advanced repository pattern initialized.");
        Console.WriteLine("[RepositoryModule] Features:");
        Console.WriteLine("  - Generic Repository Pattern (IRepository<TEntity, TKey>)");
        Console.WriteLine("  - Read-Only Repository (IReadOnlyRepository<TEntity, TKey>)");
        Console.WriteLine("  - Unit of Work Pattern (IUnitOfWork)");
        Console.WriteLine("  - Specification Pattern (ISpecification<T>)");
        Console.WriteLine("  - In-Memory Implementation for testing");
        Console.WriteLine("  - Support for custom key types (int, long, Guid, etc.)");
    }

    /// <inheritdoc/>
    public override void Shutdown(ModuleContext context)
    {
        Console.WriteLine("[RepositoryModule] Shutting down...");
    }
}