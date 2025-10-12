using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Essentials.Modularity;

internal static class TestHelper
{
    public static ModuleContext CreateModuleContext()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        return new ModuleContext(services, configuration);
    }

    public static ModuleContext CreateModuleContext(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().Build();
        return new ModuleContext(services, configuration);
    }

    public static ModuleContext CreateModuleContext(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        return new ModuleContext(services, configuration);
    }

    public static ModuleContext CreateModuleContext(IServiceCollection services, IConfiguration configuration)
    {
        return new ModuleContext(services, configuration);
    }
}
