using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Graph;

namespace weatherApi.Services;

public class MicrosoftGraphClaimsTransformationService : IClaimsTransformation
{
  private GraphServiceClient _graphServiceClient;
  private ILogger<MicrosoftGraphClaimsTransformationService> _logger;

  public MicrosoftGraphClaimsTransformationService(GraphServiceClient graphServiceClient, ILogger<MicrosoftGraphClaimsTransformationService> logger)
  {
    _graphServiceClient = graphServiceClient;
    _logger = logger;
  }
  public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
  {
    ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;

    if (identity == null)
    {
      throw new ArgumentNullException("Identity cannot be null");
    }

    if (!principal.HasClaim(claim => claim.Type == "http://schemas.xmlsoap.org/claims/Group"))
    {
      ClaimsIdentity groupClaimsIdentity = new ClaimsIdentity();
      Claim? servicePrincipalObjectId = identity.Claims.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier");
      if (servicePrincipalObjectId != null)
      {
        _logger.LogInformation($"Retrieving group memebrship for {servicePrincipalObjectId.Value}");
        var groups = await _graphServiceClient.ServicePrincipals[servicePrincipalObjectId.Value].MemberOf.Request().GetAsync();

        foreach (var group in groups)
        {
          _logger.LogInformation("Adding groups claim to principal");
          groupClaimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/claims/Group", group.Id));
        };

        principal.AddIdentity(groupClaimsIdentity);
      }
    }
    return principal;
  }
}