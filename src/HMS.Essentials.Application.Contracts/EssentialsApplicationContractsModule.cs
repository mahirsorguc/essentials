using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.Essentials;

[DependsOn(
    typeof(EssentialsDomainSharedModule)
)]
public class EssentialsApplicationContractsModule : EssentialsModule
{
}