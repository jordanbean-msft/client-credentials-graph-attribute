using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using weatherApp.Options;

namespace weatherApp.Services;

public class WeatherApiService : IWeatherApiService
{
  private readonly HttpClient _httpClient;
  private readonly AzureAdOptions _azureAdOptions;
  private readonly WeatherApiOptions _weatherApiOptions;
  private readonly ILogger<WeatherApiService> _logger;
  private readonly ClientSecretCredential _clientSecretCredential;
  private const string WeatherApiUriPrefix = "api/WeatherForecast/";

  public WeatherApiService(HttpClient httpClient, IOptions<AzureAdOptions> azureAdOptions, IOptions<WeatherApiOptions> weatherApiOptions, ILogger<WeatherApiService> logger)
  {
    _httpClient = httpClient;
    _azureAdOptions = azureAdOptions.Value;
    _weatherApiOptions = weatherApiOptions.Value;
    _logger = logger;

    _httpClient.BaseAddress = new Uri(_weatherApiOptions.BaseAddress);
    _clientSecretCredential = new ClientSecretCredential(
        _azureAdOptions.TenantId,
        _azureAdOptions.ClientId,
        _azureAdOptions.ClientSecret,
        new TokenCredentialOptions { AuthorityHost = new Uri(_azureAdOptions.Instance) }
    );
  }

  public async Task<List<string>> GetAzureAdGroupsAsync()
  {
    List<string> returnValue = new List<string>();

    await AddAuthorizationToHttpClient();

    var result = await _httpClient.GetAsync($"{WeatherApiUriPrefix}GetAzureAdGroups");

    if (result.IsSuccessStatusCode)
    {
      returnValue = await result.Content.ReadFromJsonAsync<List<string>>();
    }

    return returnValue;
  }

  public async Task<List<string>> GetAzureAdRolesAsync()
  {
    List<string> returnValue = new List<string>();

    await AddAuthorizationToHttpClient();

    var result = await _httpClient.GetAsync($"{WeatherApiUriPrefix}GetAzureAdRoles");

    if (result.IsSuccessStatusCode)
    {
      returnValue = await result.Content.ReadFromJsonAsync<List<string>>();
    }

    return returnValue;
  }

  public async Task<List<WeatherForecast>> GetWeatherForecastsAsync()
  {
    List<WeatherForecast> returnValue = new List<WeatherForecast>();

    await AddAuthorizationToHttpClient();

    var result = await _httpClient.GetAsync($"{WeatherApiUriPrefix}GetWeatherForecasts");

    if (result.IsSuccessStatusCode)
    {
      returnValue = await result.Content.ReadFromJsonAsync<List<WeatherForecast>>();
    }

    return returnValue;
  }

  private async Task AddAuthorizationToHttpClient()
  {
    var accessToken = await _clientSecretCredential.GetTokenAsync(new TokenRequestContext(new[] { _weatherApiOptions.Scopes }));

    _httpClient.DefaultRequestHeaders.Clear();
    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken.Token}");
    _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
  }
}
