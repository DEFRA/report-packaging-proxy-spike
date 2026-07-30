namespace Defra.ReportPackagingProxySpike.ReverseProxy.IntegrationTests;

[Trait("Category", "IntegrationTests")]
public abstract class IntegrationTestBase
{
    protected static string TraceHeaderName { get; } = GetTraceHeaderName();

    protected static HttpClient CreateClient() => new() { BaseAddress = new Uri("http://localhost:8085") };

    private static string GetTraceHeaderName()
    {
        using var appSettings = JsonDocument.Parse(File.ReadAllText("appsettings.json"));
        var traceHeaderName = appSettings.RootElement.GetProperty("TraceHeader").GetString();
        if (string.IsNullOrWhiteSpace(traceHeaderName))
            throw new InvalidOperationException("The TraceHeader setting must have a value.");

        return traceHeaderName;
    }
}
