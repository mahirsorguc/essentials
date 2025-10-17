using HMS.Essentials.EntityFrameworkCore;
using HMS.Essentials.EntityFrameworkCore.Configuration;
using HMS.Essentials.EntityFrameworkCore.UnitOfWork;
using HMS.Essentials.Modularity;
using HMS.Essentials.UnitOfWork;
using HMS.MainApp.EntityFrameworkCore.Samples;
using HMS.MainApp.Samples;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.MainApp.EntityFrameworkCore;

[DependsOn(
    typeof(MainAppDomainModule),
    typeof(EssentialsEntityFrameworkCoreModule)
    )]
public class MainAppEntityFrameworkCoreModule : EssentialsModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        var configuration = context.Configuration;
        var services = context.Services;

        // Get the connection string from configuration
        var connectionString = configuration?.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' not found in configuration. " +
                "Please ensure appsettings.json contains a ConnectionStrings:Default entry.");
        }

        // Determine if we're in development mode
        var isDevelopment = context.Environment?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? false;

        // Register DbContext with SQL Server
        services.AddDbContext<MainAppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                // Configure SQL Server specific options
                // Note: EnableRetryOnFailure is disabled because it's incompatible with user-initiated transactions
                // used by the UnitOfWork pattern. If retry logic is needed, it should be implemented
                // at the application layer (e.g., using Polly) outside of EF Core's execution strategy.
                
                // sqlOptions.EnableRetryOnFailure(
                //     maxRetryCount: 5,
                //     maxRetryDelay: TimeSpan.FromSeconds(30),
                //     errorNumbersToAdd: null);
            });

            // Enable detailed errors in development
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging(isDevelopment);
        });

        // Register connection string provider with the Default connection
        var connectionStringProvider = services
            .BuildServiceProvider()
            .GetService<IConnectionStringProvider>();

        if (connectionStringProvider is DefaultConnectionStringProvider defaultProvider)
        {
            defaultProvider.AddConnectionString("Default", connectionString);
            defaultProvider.AddConnectionString(nameof(MainAppDbContext), connectionString);
        }

        // Register UnitOfWork with MainAppDbContext
        services.AddScoped<IUnitOfWork, EfCoreUnitOfWork<MainAppDbContext>>();

        ConfigureRepositories(services);
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<ISampleRepository, EfCoreSampleRepository>();
    }
}
