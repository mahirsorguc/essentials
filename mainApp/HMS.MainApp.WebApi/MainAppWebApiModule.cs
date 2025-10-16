using HMS.Essentials.AspNetCore.Serilog;
using HMS.Essentials.MediatR;
using HMS.Essentials.Modularity;
using HMS.Essentials.Swashbuckle;

namespace HMS.MainApp.WebApi;

[DependsOn(
    typeof(EssentialsAspNetCoreSerilogModule),
    typeof(EssentialsSwashbuckleModule),
    typeof(MainAppApplicationModule),
    typeof(MainAppApplicationCommandsModule),
    typeof(MainAppApplicationQueriesModule),
    typeof(MainAppEntityFrameworkCoreModule)
)]
public class MainAppWebApiModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // Add controller services
        context.Services.AddControllers();

        // Configure MediatR to scan application assemblies for handlers
        context.Services.AddMediatRHandlers(typeof(MainAppApplicationModule).Assembly);
    }
}