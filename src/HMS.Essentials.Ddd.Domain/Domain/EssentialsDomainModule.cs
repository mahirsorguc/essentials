using HMS.Essentials.Domain.Events;
using HMS.Essentials.MediatR;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Domain;

[DependsOn(
    typeof(EssentialsDomainSharedModule),
    typeof(EssentialsDomainEventsModule),
    typeof(EssentialsMediatRModule)
    
)]
public class EssentialsDomainModule : EssentialsModule
{
}