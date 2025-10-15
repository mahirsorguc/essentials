using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle;

namespace HMS.MainApp.WebApi;

[DependsOn(
    typeof(EssentialsSwashbuckleModule),
    typeof(MainAppApplicationModule),
    typeof(MainAppApplicationCommandsModule),
    typeof(MainAppApplicationQueriesModule),
    typeof(MainAppEntityFrameworkCoreModule)
)]
public class MainAppWebApiModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Add controller services
        context.Services.AddControllers();
    }
}