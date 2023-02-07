namespace weatherApi.Options;

public class AzureAdGroupsOptions
{
  public const string SectionName = "AzureAdGroups";
  public string DataReadAADGroupObjectId { get; set; } = string.Empty;
  public string DataWriteAADGroupObjectId { get; set; } = string.Empty;
}