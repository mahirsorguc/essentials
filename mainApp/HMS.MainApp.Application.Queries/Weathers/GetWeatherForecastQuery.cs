using HMS.Essentials.MediatR;

namespace HMS.MainApp.Weathers;

/// <summary>
/// Query to get weather forecast.
/// </summary>
public record GetWeatherForecastQuery : QueryBase<IEnumerable<WeatherForecastDto>>
{
    /// <summary>
    /// Gets or sets the number of days to forecast.
    /// </summary>
    public int Days { get; init; } = 5;
}
