namespace weatherApi;

public static class Constants
{
  public const string GroupClaimType = "http://schemas.xmlsoap.org/claims/Group";
  public const string RolesClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  public const string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
  public const string MemberOfDataReadAADGroupPolicyName = "MemberOfDataReadAADGroup";
  public const string MemberOfDataWriteAADGroupPolicyName = "MemberOfDataWriteAADGroup";
  public const string RequireDataReadRolePolicyName = "RequireDataReadRole"
  ;
  public const string RequireDataWriteRolePolicyName = "RequireDataWriteRole";
}