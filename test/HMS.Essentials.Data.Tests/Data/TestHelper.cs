using HMS.Essential.Logging;
using HMS.Essentials.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HMS.Essentials.Data;

internal static class TestHelper
{
    public static ModuleContext CreateModuleContext()
    {
        var services = new ServiceCollection();
        
        // Add mock ILogService
        var mockLogService = new Mock<ILogService>();
        services.AddSingleton(mockLogService.Object);
        
        var configuration = new ConfigurationBuilder().Build();
        return new ModuleContext(services, configuration);
    }

    public static ModuleContext CreateModuleContext(IServiceCollection services)
    {
        // Add mock ILogService if not already registered
        if (!services.Any(s => s.ServiceType == typeof(ILogService)))
        {
            var mockLogService = new Mock<ILogService>();
            services.AddSingleton(mockLogService.Object);
        }
        
        var configuration = new ConfigurationBuilder().Build();
        return new ModuleContext(services, configuration);
    }
}
