using HMS.Essentials.Modularity;

namespace HMS.Essentials.MediatR;

[DependsOn(typeof(EssentialsMediatRContractsModule))]
public class EssentialsMediatRModule : EssentialsModule
{
}