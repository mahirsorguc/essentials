using HMS.Essentials.MediatR;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Domain.Events;

[DependsOn(typeof(EssentialsMediatRDomainEventsModule))]
public class EssentialsDomainEventsModule : EssentialsModule
{
}