using HMS.Essentials.Application.Commands;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppApplicationContractsModule),
    typeof(EssentialsApplicationCommandsModule)
)]
public class MainAppApplicationCommandsModule : EssentialsModule
{
}