using BrighterTest.Lib.Commands;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace BrighterTest.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly IAmACommandProcessor _commandProcessor;

    public HelloController(IAmACommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Test(string name)
    {
        await _commandProcessor.PostAsync(new HelloEvent(name));

        return Ok();
    }
}