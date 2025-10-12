using HMS.Essentials.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials;

public class EssentialsCoreTestsModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        context.Services.AddSingleton<TestServicePlaceholder>();
    }
}