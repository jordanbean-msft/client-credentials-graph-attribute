namespace weatherApi.Options;

public class MicrosoftGraphOptions
{
  public const string SectionName = "MicrosoftGraph";
  public string GraphApiScope { get; set; } = string.Empty;
}