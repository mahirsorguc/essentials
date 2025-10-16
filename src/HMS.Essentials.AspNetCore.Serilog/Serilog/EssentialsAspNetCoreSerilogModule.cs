using HMS.Essentials.Modularity;

namespace HMS.Essentials.AspNetCore.Serilog;

[DependsOn(typeof(EssentialsAspNetCoreModule))]
public class EssentialsAspNetCoreSerilogModule : EssentialsModule
{
}