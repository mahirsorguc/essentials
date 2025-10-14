using HMS.Essentials.Application;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppApplicationCommandsModule),
    typeof(MainAppApplicationQueriesModule),
    typeof(MainAppDomainModule),
    typeof(EssentialsApplicationModule)
)]
public class MainAppApplicationModule : EssentialsModule
{
}