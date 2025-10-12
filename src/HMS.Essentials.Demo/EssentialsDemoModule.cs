using HMS.Essentials.Data;
using HMS.Essentials.Domain;
using HMS.Essentials.Domain.Extensions;
using HMS.Essentials.Entities;
using HMS.Essentials.Modularity;
using HMS.Essentials.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials;

/// <summary>
/// Root module for the demo application.
/// This module demonstrates the repository pattern with various entities and services.
/// </summary>
[DependsOn(
    typeof(EssentialsDataModule),
    typeof(EssentialsDomainModule)
)]
public class EssentialsDemoModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        Console.WriteLine("[DemoModule] Configuring services...");
        
        // Register in-memory repositories for demo entities
        context.Services.AddInMemoryRepository<Product, int>();
        context.Services.AddInMemoryRepository<Customer, Guid>();
        context.Services.AddInMemoryRepository<Order, Guid>();
        
        // Register demo services
        context.Services.AddScoped<ProductService>();
        context.Services.AddScoped<OrderService>();
        
        Console.WriteLine("[DemoModule] Repository pattern services registered.");
    }

    public override void Initialize(ModuleContext context)
    {
        Console.WriteLine("[DemoModule] Initialized successfully.");
        Console.WriteLine("[DemoModule] Ready to demonstrate repository pattern features.");
    }

    public override void Shutdown(ModuleContext context)
    {
        Console.WriteLine("[DemoModule] Shutting down...");
    }
}