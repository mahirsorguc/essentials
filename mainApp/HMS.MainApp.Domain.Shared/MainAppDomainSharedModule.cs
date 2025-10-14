using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(typeof(EssentialsDomainSharedModule))]
public class MainAppDomainSharedModule : EssentialsModule
{
}