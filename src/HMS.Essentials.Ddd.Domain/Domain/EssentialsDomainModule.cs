using HMS.Essentials.Domain.Events;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Domain;

[DependsOn(
    typeof(EssentialsDomainSharedModule),
    typeof(EssentialsDomainEventsModule)
    )]
public class EssentialsDomainModule : EssentialsModule
{
}