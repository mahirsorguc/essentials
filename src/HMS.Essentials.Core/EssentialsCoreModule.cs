using HMS.Essentials.Modularity;
using HMS.Essentials.SequentialGuid;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials;

public class EssentialsCoreModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register Sequential GUID Generator
        context.Services.AddSingleton<ISequentialGuidGenerator, SequentialGuidGenerator>();
    }
}