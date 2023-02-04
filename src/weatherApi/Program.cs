using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using weatherApi.Options;
using weatherApi.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection(AzureAdOptions.SectionName));
builder.Services.Configure<AzureAdRolesOptions>(builder.Configuration.GetSection(AzureAdRolesOptions.SectionName));
builder.Services.Configure<AzureAdGroupsOptions>(builder.Configuration.GetSection(AzureAdGroupsOptions.SectionName));
builder.Services.Configure<MicrosoftGraphOptions>(builder.Configuration.GetSection(MicrosoftGraphOptions.SectionName));

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection(AzureAdOptions.SectionName));

AzureAdOptions? azureAdOptions = builder.Configuration.GetSection(AzureAdOptions.SectionName).Get<AzureAdOptions>();
AzureAdRolesOptions? azureAdRolesOptions = builder.Configuration.GetSection(AzureAdRolesOptions.SectionName).Get<AzureAdRolesOptions>();
AzureAdGroupsOptions? azureAdGroupsOptions = builder.Configuration.GetSection(AzureAdGroupsOptions.SectionName).Get<AzureAdGroupsOptions>();
MicrosoftGraphOptions? microsoftGraphOptions = builder.Configuration.GetSection(MicrosoftGraphOptions.SectionName).Get<MicrosoftGraphOptions>();

if (azureAdOptions != null && azureAdRolesOptions != null && azureAdGroupsOptions != null && microsoftGraphOptions != null)
{
  var clientSecretCredential = new ClientSecretCredential(
        azureAdOptions.TenantId,
        azureAdOptions.ClientId,
        azureAdOptions.ClientSecret,
        new TokenCredentialOptions { AuthorityHost = new Uri(azureAdOptions.Instance) }
    );

  var graphClient = new GraphServiceClient(clientSecretCredential,
    new string[] { microsoftGraphOptions.GraphApiScope });
  builder.Services.AddSingleton(graphClient);

  builder.Services.AddAuthorization(options =>
  {
    options.AddPolicy(weatherApi.Constants.MemberOfDataReadAADGroupPolicyName,
      policy =>
      {
        policy.RequireClaim(weatherApi.Constants.GroupClaimType, azureAdGroupsOptions.DataReadAADGroupObjectId);
      });
    options.AddPolicy(weatherApi.Constants.MemberOfDataWriteAADGroupPolicyName,
      policy =>
      {
        policy.RequireClaim(weatherApi.Constants.GroupClaimType, azureAdGroupsOptions.DataWriteAADGroupObjectId);
      });
    options.AddPolicy(weatherApi.Constants.RequireDataReadRolePolicyName,
    policy =>
    {
      policy.RequireRole(azureAdRolesOptions.DataReadRoleName);
    });
    options.AddPolicy(weatherApi.Constants.RequireDataWriteRolePolicyName,
    policy =>
    {
      policy.RequireRole(azureAdRolesOptions.DataWriteRoleName);
    });
  }
  );
}
else
{
  throw new ArgumentNullException("Missing configuration settings");
}

builder.Services.AddTransient<IClaimsTransformation, MicrosoftGraphClaimsTransformationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
