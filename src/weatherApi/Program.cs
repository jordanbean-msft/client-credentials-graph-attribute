using System.IdentityModel.Tokens.Jwt;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using weatherApi.Options;
using weatherApi.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection(AzureAdOptions.AzureAd));
// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
  options.TokenValidationParameters.RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
});

AzureAdOptions? azureAdOptions = builder.Configuration.GetSection(AzureAdOptions.AzureAd).Get<AzureAdOptions>();
if (azureAdOptions != null)
{
  var clientSecretCredential = new ClientSecretCredential(
        azureAdOptions.TenantId,
        azureAdOptions.ClientId,
        azureAdOptions.ClientSecret,
        new TokenCredentialOptions { AuthorityHost = new Uri(azureAdOptions.Instance) }
    );

  var graphClient = new GraphServiceClient(clientSecretCredential,
    new string[] { azureAdOptions.GraphApiScope });
  builder.Services.AddSingleton(graphClient);
  builder.Services.AddAuthorization(options =>
  {
    options.AddPolicy("MemberOfDataReadAADGroup",
      policy =>
      {
        policy.RequireClaim("http://schemas.xmlsoap.org/claims/Group", azureAdOptions.DataReadAADGroupObjectId);
      });
    options.AddPolicy("MemberOfDataWriteAADGroup",
      policy =>
      {
        policy.RequireClaim("http://schemas.xmlsoap.org/claims/Group", azureAdOptions.DataWriteAADGroupObjectId);
      });
    options.AddPolicy("RequireDataReadRole",
    policy =>
    {
      policy.RequireRole("Data.Read");
    });
    options.AddPolicy("RequireDataWriteRole",
    policy =>
    {
      policy.RequireRole("Data.Write");
    });
  }
  );
}
else
{
  throw new ArgumentNullException("AzureAd in appsettings.json cannot be null.");
}

builder.Services.AddTransient<IClaimsTransformation, MicrosoftGraphClaimsTransformationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
