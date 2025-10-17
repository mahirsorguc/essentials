using HMS.Essentials.Application;
using HMS.Essentials.AutoMapper.Extensions;
using HMS.Essentials.FluentValidation.Extensions;
using HMS.Essentials.Modularity;

namespace HMS.MainApp;

[DependsOn(
    typeof(MainAppApplicationCommandsModule),
    typeof(MainAppApplicationQueriesModule),
    typeof(MainAppDomainModule),
    typeof(EssentialsApplicationModule)
)]
public class MainAppApplicationModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Register AutoMapper and scan for profiles in the current assembly
        context.Services.AddEssentialsAutoMapper(typeof(MainAppApplicationModule).Assembly);

        context.Services.AddValidatorsFromAssembly(typeof(MainAppApplicationModule).Assembly);
    }
}