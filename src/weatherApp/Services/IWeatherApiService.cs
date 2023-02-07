namespace weatherApp.Services
{
  public interface IWeatherApiService
  {
    public Task<List<string>> GetAzureAdGroupsAsync();

    public Task<List<string>> GetAzureAdRolesAsync();

    public Task<List<WeatherForecast>> GetWeatherForecastsAsync();
  }
}