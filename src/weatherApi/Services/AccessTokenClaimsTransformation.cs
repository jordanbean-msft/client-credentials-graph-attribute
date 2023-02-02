using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Graph;

namespace weatherApi.Services;

public class AccessTokenClaimsTransformation : IClaimsTransformation
{
  private GraphServiceClient _graphServiceClient;

  public AccessTokenClaimsTransformation(GraphServiceClient graphServiceClient)
  {
    _graphServiceClient = graphServiceClient;
  }
  public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
  {
    if (!principal.HasClaim(claim => claim.Type == "groups"))
    {
      var groups = await _graphServiceClient.ServicePrincipals[((ClaimsIdentity)principal.Identity).Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value].MemberOf.Request().GetAsync();
      List<Claim> claims = new List<Claim>();
      foreach (var group in groups)
      {
        claims.Add(new Claim("groups", group.Id));
      };
      ClaimsIdentity appIdentity = new ClaimsIdentity(claims);
      principal.AddIdentity(appIdentity);
    }
    return principal;
  }
}