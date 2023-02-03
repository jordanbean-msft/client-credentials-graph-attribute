using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace weatherApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
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

  [HttpGet(Name = "GetWeatherForecast")]
  //[Authorize(Roles = "Data.Read, Data.Write")]
  [Authorize(Policy = "MemberOfDataReadAADGroup")]
  [Authorize(Policy = "RequireDataReadRole")]
  public IEnumerable<string> Get()
  {
    List<string> groups = new List<string>();
    groups.AddRange(HttpContext.User.Identities.SelectMany(i => i.Claims).Where(c => c.Type == "http://schemas.xmlsoap.org/claims/Group").Select(c => c.Value));
    List<string> roles = new List<string>();
    roles.AddRange(HttpContext.User.Identities.SelectMany(i => i.Claims).Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value));
    var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
      Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = Summaries[Random.Shared.Next(Summaries.Length)],

    });
    List<string> result = new List<string>();
    result.AddRange(groups);
    result.AddRange(roles);
    result.AddRange(weatherForecasts.Select(x => JsonSerializer.Serialize(x)));
    return result;
  }
}
