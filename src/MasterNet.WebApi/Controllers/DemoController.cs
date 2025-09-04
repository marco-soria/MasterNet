using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/demo")]
public class DemoController(IConfiguration configuration, IWebHostEnvironment environment) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _environment = environment;

    [HttpGet("getstring")]
    public string GetName()
    {
        return "namae";
    }

    [HttpGet("environment")]
    public IActionResult GetEnvironment()
    {
       var message =  _configuration.GetValue<string>("MyVariable");
       var environment = _environment.EnvironmentName;

        return Ok(new { Environment = environment, Message = message });
    }

}