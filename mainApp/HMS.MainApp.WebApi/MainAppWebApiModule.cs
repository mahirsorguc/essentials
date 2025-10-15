using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppApplicationModule),
    typeof(MainAppApplicationCommandsModule),
    typeof(MainAppApplicationQueriesModule),
    typeof(MainAppEntityFrameworkCoreModule)
)]
public class MainAppWebApiModule : EssentialsModule
{
}