using HMS.Essentials.MediatR;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Application.Queries;

[DependsOn(
    typeof(EssentialsApplicationContractsModule),
    typeof(EssentialsMediatRQueriesModule)
)]
public class EssentialsApplicationQueriesModule : EssentialsModule
{
}