namespace HMS.MainApp.Weathers;

/// <summary>
/// Weather forecast data transfer object.
/// </summary>
public record WeatherForecastDto(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Gets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
