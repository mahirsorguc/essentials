using HMS.Essentials.Application.Queries;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppApplicationContractsModule),
    typeof(EssentialsApplicationQueriesModule))]
public class MainAppApplicationQueriesModule : EssentialsModule
{
}