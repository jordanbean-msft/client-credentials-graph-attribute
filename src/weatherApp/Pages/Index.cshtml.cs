using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using weatherApp.Services;

namespace weatherApp.Pages;

public class IndexModel : PageModel
{
  [BindProperty(SupportsGet = true)]
  public List<string> AzureAdGroups { get; set; } = new List<string>();
  [BindProperty(SupportsGet = true)]
  public List<string> AzureAdRoles { get; set; } = new List<string>();
  [BindProperty(SupportsGet = true)]
  public List<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
  private readonly ILogger<IndexModel> _logger;
  private readonly IWeatherApiService _weatherApiService;

  public IndexModel(ILogger<IndexModel> logger, IWeatherApiService weatherApiService)
  {
    _logger = logger;
    _weatherApiService = weatherApiService;
  }

  public async Task OnGetAsync()
  {
    AzureAdGroups = await _weatherApiService.GetAzureAdGroupsAsync();
    AzureAdRoles = await _weatherApiService.GetAzureAdRolesAsync();
    WeatherForecasts = await _weatherApiService.GetWeatherForecastsAsync();
  }
}
