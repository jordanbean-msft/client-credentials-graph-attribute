using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using weatherApi.Options;
using weatherApi.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection(AzureAdOptions.AzureAd));
// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

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
}
else
{
  throw new ArgumentNullException("AzureAd in appsettings.json cannot be null.");
}

builder.Services.AddTransient<IClaimsTransformation, AccessTokenClaimsTransformation>();

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
