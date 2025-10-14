using HMS.Essentials.Application;
using HMS.Essentials.Data;
using HMS.Essentials.Domain;
using HMS.Essentials.Domain.Extensions;
using HMS.Essentials.Entities;
using HMS.Essentials.EntityFrameworkCore;
using HMS.Essentials.Modularity;
using HMS.Essentials.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials;

/// <summary>
///     Root module for the demo application.
///     This module demonstrates the repository pattern with various entities and services.
/// </summary>
[DependsOn(
    typeof(EssentialsApplicationModule),
    typeof(EssentialsEntityFrameworkCoreModule),
    typeof(EssentialsDataModule),
    typeof(EssentialsDomainModule)
)]
public class EssentialsDemoModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register in-memory repositories for demo entities
        context.Services.AddInMemoryRepository<Product, int>();
        context.Services.AddInMemoryRepository<Customer, Guid>();
        context.Services.AddInMemoryRepository<Order, Guid>();

        // Register demo services
        context.Services.AddScoped<ProductService>();
        context.Services.AddScoped<OrderService>();
    }
}