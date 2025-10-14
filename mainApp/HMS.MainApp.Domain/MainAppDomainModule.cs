using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppDomainSharedModule),
    typeof(EssentialsDomainModule))]
public class MainAppDomainModule : EssentialsModule
{
}