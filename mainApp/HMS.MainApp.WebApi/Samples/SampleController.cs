using HMS.MainApp.Samples;
using HMS.MainApp.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HMS.MainApp.WebApi.Samples;

[ApiController]
[Route("api/[controller]")]
public class SampleController : MainAppControllerBase
{
    private readonly IMediator _mediator;

    public SampleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SampleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSample([FromBody] CreateSampleDto createSampleDto)
    {
        var createSampleCommand = new CreateSampleCommand { CreateSampleDto = createSampleDto };
        var result = await _mediator.Send(createSampleCommand);
        return Ok(result);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(SampleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSamples()
    {
        var getSamplesQuery = new GetSamplesQuery();
        var result = await _mediator.Send(getSamplesQuery);
        return Ok(result);
    }
}