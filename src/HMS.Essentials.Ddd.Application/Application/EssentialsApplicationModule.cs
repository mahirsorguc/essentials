using HMS.Essentials.Application.Commands;
using HMS.Essentials.Application.Queries;
using HMS.Essentials.Domain;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Application;

[DependsOn(
    typeof(EssentialsApplicationCommandsModule),
    typeof(EssentialsApplicationQueriesModule),
    typeof(EssentialsDomainModule)
)]
public class EssentialsApplicationModule : EssentialsModule
{
}