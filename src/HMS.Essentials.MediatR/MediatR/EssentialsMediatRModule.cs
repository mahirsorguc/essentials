using HMS.Essentials.FluentValidation;
using HMS.Essentials.MediatR.Behaviors;
using HMS.Essentials.Modularity;
using HMS.Essentials.ObjectMapping;
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
    typeof(EssentialsUnitOfWorkModule),
    typeof(EssentialsFluentValidationModule),
    typeof(EssentialsObjectMappingModule)
)]
public class EssentialsMediatRModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register our custom mediator wrapper (optional - keeps a project-specific IMediator)
        context.Services.AddScoped<IMediator, Mediator>();

        // Register pipeline behaviors (order matters: validators should run early)
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
    }
}