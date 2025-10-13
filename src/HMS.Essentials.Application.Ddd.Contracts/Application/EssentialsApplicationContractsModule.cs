using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Application;

[DependsOn(
    typeof(EssentialsDomainSharedModule)
)]
public class EssentialsApplicationContractsModule : EssentialsModule
{
}