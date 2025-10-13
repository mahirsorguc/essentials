using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Application;

[DependsOn(
    typeof(EssentialsApplicationContractsModule),
    typeof(EssentialsDomainModule)
)]
public class EssentialsApplicationModule : EssentialsModule
{
}