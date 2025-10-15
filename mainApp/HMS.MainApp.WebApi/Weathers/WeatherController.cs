using HMS.MainApp.Weathers;
using HMS.MainApp.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HMS.MainApp.WebApi.Weathers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : MainAppControllerBase
{
    private readonly IMediator _mediator;

    public WeatherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the weather forecast.
    /// </summary>
    /// <param name="days">Number of days to forecast (default: 5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Weather forecast collection.</returns>
    [HttpGet("forecast")]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecastDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWeatherForecast([FromQuery] int days = 5, CancellationToken cancellationToken = default)
    {
        var query = new GetWeatherForecastQuery { Days = days };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}