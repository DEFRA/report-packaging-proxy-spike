namespace Defra.ReportPackagingProxySpike.ReverseProxy.IntegrationTests;

[Trait("Category", "IntegrationTests")]
public abstract class IntegrationTestBase
{
    protected static HttpClient CreateClient() => new() { BaseAddress = new Uri("http://localhost:8085") };
}
