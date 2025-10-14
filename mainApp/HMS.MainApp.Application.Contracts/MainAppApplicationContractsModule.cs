using HMS.Essentials.Application;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppDomainSharedModule),
    typeof(EssentialsApplicationContractsModule)
)]
public class MainAppApplicationContractsModule : EssentialsModule
{
}