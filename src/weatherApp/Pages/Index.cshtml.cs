using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using weatherApp.Services;

namespace weatherApp.Pages;

public class IndexModel : PageModel
{
  private readonly ILogger<IndexModel> _logger;
  private readonly IWeatherApiService _weatherApiService;

  public IndexModel(ILogger<IndexModel> logger, IWeatherApiService weatherApiService)
  {
    _logger = logger;
    _weatherApiService = weatherApiService;
  }

  public async void OnGet()
  {
    var groups = await _weatherApiService.GetAzureAdGroupsAsync();
    var roles = await _weatherApiService.GetAzureAdRolesAsync();
    var forecasts = await _weatherApiService.GetWeatherForecastsAsync();

  }
}
