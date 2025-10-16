using HMS.Essentials.MediatR;

namespace HMS.MainApp.Weathers;

/// <summary>
/// Handler for getting weather forecast.
/// </summary>
public class GetWeatherForecastQueryHandler : IQueryHandler<GetWeatherForecastQuery, IEnumerable<WeatherForecastDto>>
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /// <summary>
    /// Handles the get weather forecast query.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of weather forecasts.</returns>
    public Task<IEnumerable<WeatherForecastDto>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        var forecast = Enumerable.Range(1, request.Days).Select(index =>
                new WeatherForecastDto
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToList();

        return Task.FromResult<IEnumerable<WeatherForecastDto>>(forecast);
    }
}
