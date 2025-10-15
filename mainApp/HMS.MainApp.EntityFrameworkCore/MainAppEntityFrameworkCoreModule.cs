using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(typeof(MainAppDomainModule))]
public class MainAppEntityFrameworkCoreModule : EssentialsModule
{
}
