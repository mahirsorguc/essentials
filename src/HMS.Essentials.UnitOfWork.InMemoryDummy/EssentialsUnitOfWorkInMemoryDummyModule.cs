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
    }
}