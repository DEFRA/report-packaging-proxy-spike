using AwesomeAssertions;
using Defra.Spike.ReportPackagingProxy.ReverseProxy.Configuration;
using Microsoft.Extensions.Configuration;

namespace Defra.Spike.ReportPackagingProxy.ReverseProxy.Tests.Configuration;

public class ReverseProxyConfigurationValidatorTests
{
    [Fact]
    public void Validate_WhenDestinationAddressIsUnconfigured_ShouldThrow()
    {
        var configuration = CreateConfiguration("https://unconfigured.invalid/");

        var act = () => ReverseProxyConfigurationValidator.Validate(configuration.GetSection("ReverseProxy"));

        act.Should().Throw<InvalidOperationException>().WithMessage("*ManageRecyclingObligations:Primary*");
    }

    [Fact]
    public void Validate_WhenDestinationAddressIsConfigured_ShouldNotThrow()
    {
        var configuration = CreateConfiguration("https://manage-recycling-obligations.example/");

        var act = () => ReverseProxyConfigurationValidator.Validate(configuration.GetSection("ReverseProxy"));

        act.Should().NotThrow();
    }

    private static IConfiguration CreateConfiguration(string address) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ReverseProxy:Clusters:ManageRecyclingObligations:Destinations:Primary:Address"] = address,
                }
            )
            .Build();
}
