namespace weatherApi.Options;

public class AzureAdOptions
{
  public const string SectionName = "AzureAd";
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
  public string TenantId { get; set; } = string.Empty;
  public string Instance { get; set; } = string.Empty;
  public string Domain { get; set; } = string.Empty;
  public string CallbackPath { get; set; } = string.Empty;
  public bool AllowWebApiToBeAuthorizedByACL { get; set; } = false;
}