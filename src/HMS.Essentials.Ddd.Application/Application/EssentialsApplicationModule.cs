using HMS.Essentials.Application.Commands;
using HMS.Essentials.Application.Queries;
using HMS.Essentials.AutoMapper;
using HMS.Essentials.Domain;
using HMS.Essentials.MediatR;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Application;

[DependsOn(
    typeof(EssentialsAutomapperModule),
    typeof(EssentialsApplicationCommandsModule),
    typeof(EssentialsApplicationQueriesModule),
    typeof(EssentialsDomainModule),
    typeof(EssentialsMediatRModule)
)]
public class EssentialsApplicationModule : EssentialsModule
{
}