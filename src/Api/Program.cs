var builder = WebApplication.CreateBuilder(args);

var port = builder.Configuration["PORT"];
if (int.TryParse(port, out var configuredPort))
{
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(configuredPort));
}

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { message = "success" })).WithOrder(-1);
app.MapReverseProxy();

app.Run();
