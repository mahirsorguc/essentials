using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.Essentials;

[DependsOn(
    typeof(EssentialsApplicationContractsModule),
    typeof(EssentialsDomainModule)
)]
public class EssentialsApplicationModule : EssentialsModule
{
}