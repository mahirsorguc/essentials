using System.Reflection;
using HMS.Essentials.MediatR.Behaviors;
using HMS.Essentials.Modularity;
using HMS.Essentials.UnitOfWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.MediatR;

/// <summary>
///     MediatR module that configures CQRS pattern services.
///     Provides Command, Query, and Domain Event handling capabilities.
/// </summary>
[DependsOn(
    typeof(EssentialsMediatRCommandsModule),
    typeof(EssentialsMediatRQueriesModule),
    typeof(EssentialsMediatRDomainEventsModule),
    typeof(EssentialsUnitOfWorkModule)
)]
public class EssentialsMediatRModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register MediatR and scan this assembly for handlers/notifications
        context.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(EssentialsMediatRModule).Assembly);
        });

        // Register our custom mediator wrapper (optional - keeps a project-specific IMediator)
        context.Services.AddScoped<IMediator, Mediator>();

        // Register pipeline behaviors (order matters: validators should run early)
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
    }
}

/// <summary>
///     Extension methods for registering MediatR handlers discovered via reflection.
///     These helpers are optional; MediatR's RegisterServicesFromAssembly is used in the module.
/// </summary>
public static class MediatRServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRHandlers(this IServiceCollection services, Assembly assembly)
    {
        // Use MediatR's registration to register handlers, requests, notifications, etc.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        return services;
    }

    public static IServiceCollection AddMediatRHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddMediatRHandlers(assembly);
        }

        return services;
    }
}