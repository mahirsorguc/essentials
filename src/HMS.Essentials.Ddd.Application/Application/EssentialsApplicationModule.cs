using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;
using HMS.Essentials.UnitOfWork;

namespace HMS.Essentials.Application;

[DependsOn(
    typeof(EssentialsUnitOfWorkModule),
    typeof(EssentialsApplicationContractsModule),
    typeof(EssentialsDomainModule)
)]
public class EssentialsApplicationModule : EssentialsModule
{
}