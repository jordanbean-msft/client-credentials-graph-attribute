using Microsoft.AspNetCore.Authorization;

namespace weatherApi.Requirements
{
  public class MemberOfAADGroupRequirement : IAuthorizationRequirement
  {
    public MemberOfAADGroupRequirement(string groupObjectId)
    {
      GroupObjectId = groupObjectId;
    }

    public string GroupObjectId { get; }
  }
}