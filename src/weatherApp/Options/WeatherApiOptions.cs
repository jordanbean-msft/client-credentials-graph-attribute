namespace weatherApp.Options;

public class WeatherApiOptions
{
  public const string SectionName = "WeatherApi";
  public string BaseAddress { get; set; } = string.Empty;
  public string Scopes { get; set; } = string.Empty;
}