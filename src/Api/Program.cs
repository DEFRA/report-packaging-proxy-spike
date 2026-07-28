using Defra.Spike.ReportPackagingProxy.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration["PORT"];
if (int.TryParse(port, out var configuredPort))
{
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(configuredPort));
}

var reverseProxyConfiguration = builder.Configuration.GetSection("ReverseProxy");
ReverseProxyConfigurationValidator.Validate(reverseProxyConfiguration);
builder.Services.AddReverseProxy().LoadFromConfig(reverseProxyConfiguration);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { message = "success" })).WithOrder(-1);
app.MapReverseProxy();

app.Run();
