using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace weatherApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
  private static readonly string[] Summaries = new[]
  {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

  private readonly ILogger<WeatherForecastController> _logger;

  public WeatherForecastController(ILogger<WeatherForecastController> logger)
  {
    _logger = logger;
  }

  [HttpGet]
  [Authorize(Policy = Constants.MemberOfDataReadAADGroupPolicyName)]
  public IEnumerable<string> GetAzureAdGroups()
  {
    return HttpContext.User.Identities.SelectMany(i => i.Claims).Where(c => c.Type == Constants.GroupClaimType).Select(c => c.Value);
  }

  [HttpGet]
  [Authorize(Policy = Constants.RequireDataReadRolePolicyName)]
  public IEnumerable<string> GetAzureAdRoles()
  {
    return HttpContext.User.Identities.SelectMany(i => i.Claims).Where(c => c.Type == Constants.RolesClaimType).Select(c => c.Value);
  }

  [HttpGet]
  [Authorize(Policy = Constants.MemberOfDataReadAADGroupPolicyName)]
  [Authorize(Policy = Constants.RequireDataReadRolePolicyName)]
  public IEnumerable<WeatherForecast> GetWeatherForecasts()
  {
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
      Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = Summaries[Random.Shared.Next(Summaries.Length)],

    });
  }
}
