using HMS.Essentials.MediatR;
using HMS.Essentials.Modularity;

namespace HMS.Essentials.Application.Commands;

[DependsOn(
    typeof(EssentialsApplicationContractsModule),
    typeof(EssentialsMediatRCommandsModule)
)]
public class EssentialsApplicationCommandsModule : EssentialsModule
{
}