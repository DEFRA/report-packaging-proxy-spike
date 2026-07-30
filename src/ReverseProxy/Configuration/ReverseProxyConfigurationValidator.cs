namespace Defra.ReportPackagingProxySpike.ReverseProxy.Configuration;

internal static class ReverseProxyConfigurationValidator
{
    private const string UnconfiguredHost = "unconfigured.invalid";

    public static void Validate(IConfigurationSection reverseProxyConfiguration)
    {
        var unconfiguredDestinations = reverseProxyConfiguration
            .GetSection("Clusters")
            .GetChildren()
            .SelectMany(cluster =>
                cluster
                    .GetSection("Destinations")
                    .GetChildren()
                    .Where(destination => HasUnconfiguredAddress(destination["Address"]))
                    .Select(destination => $"{cluster.Key}:{destination.Key}")
            )
            .ToArray();

        if (unconfiguredDestinations.Length > 0)
        {
            throw new InvalidOperationException(
                $"The following reverse-proxy destinations must be configured before startup: {string.Join(", ", unconfiguredDestinations)}."
            );
        }
    }

    private static bool HasUnconfiguredAddress(string? address) =>
        Uri.TryCreate(address, UriKind.Absolute, out var uri)
        && string.Equals(uri.Host, UnconfiguredHost, StringComparison.OrdinalIgnoreCase);
}
