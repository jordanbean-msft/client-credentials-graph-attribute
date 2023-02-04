namespace weatherApi.Options;

public class AzureAdRolesOptions
{
  public const string SectionName = "AzureAdRoles";
  public string DataReadRoleName { get; set; } = string.Empty;
  public string DataWriteRoleName { get; set; } = string.Empty;
}