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
}