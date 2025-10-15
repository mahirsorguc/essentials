using HMS.Essentials.Modularity;
using HMS.MainApp.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add HMS Essentials module system to the application
builder.AddModuleSystem<MainAppWebApiModule>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

// Get the ApplicationHost to demonstrate loaded modules
var appHost = app.GetApplicationHost();
await DemonstrateLoadedModules(appHost);

app.Run();

static async Task DemonstrateLoadedModules(ApplicationHost app)
{
    Console.WriteLine("\n┌─ MODULE INFORMATION ─────────────────────────────────────┐");
    Console.WriteLine("\nLoaded Modules:");
    foreach (var module in app.Modules.OrderBy(m => m.Priority).Reverse())
    {
        Console.WriteLine($"  • {module.Name} (Priority: {module.Priority}, State: {module.State})");
    }
}

namespace HMS.MainApp.WebApi
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}