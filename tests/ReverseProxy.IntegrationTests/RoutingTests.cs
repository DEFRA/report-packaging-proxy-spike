using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;

namespace Defra.ReportPackagingProxySpike.ReverseProxy.IntegrationTests;

public class RoutingTests : IntegrationTestBase
{
    private const string TraceId = "4d2b9f4e-24de-467a-951f-342579445b2a";

    [Fact]
    public async Task ManageRecyclingObligations_ShouldRemovePublicPrefixAndForwardIt()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "/manage-recycling-obligations/returns?year=2026",
            new { reference = "example" },
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var downstreamRequest = await response.Content.ReadFromJsonAsync<DownstreamRequest>(
            TestContext.Current.CancellationToken
        );

        downstreamRequest.Should().NotBeNull();
        downstreamRequest.Method.Should().Be(HttpMethod.Post.Method);
        downstreamRequest.Path.Should().Be("/returns");
        downstreamRequest.Query.Should().Be("?year=2026");
    }

    [Fact]
    public async Task ManageRecyclingObligations_WhenTraceHeaderReceived_ShouldForwardTraceHeader()
    {
        using var client = CreateClient();
        client.DefaultRequestHeaders.Add(TraceHeaderName, TraceId);

        var response = await client.PostAsJsonAsync(
            "/manage-recycling-obligations/trace-returns?year=2026",
            new { reference = "example" },
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var downstreamRequest = await response.Content.ReadFromJsonAsync<DownstreamRequest>(
            TestContext.Current.CancellationToken
        );

        downstreamRequest.Should().NotBeNull();
        downstreamRequest.CorrelationId.Should().Be(TraceId);
    }

    [Fact]
    public async Task UnpermittedPath_ShouldReturnNotFound()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/not-permitted", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed record DownstreamRequest(string Method, string? Path, string? Query, string? CorrelationId);
}
