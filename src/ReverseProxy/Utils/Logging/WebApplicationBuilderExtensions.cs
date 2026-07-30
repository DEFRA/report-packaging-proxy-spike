using System.Diagnostics.CodeAnalysis;
using Elastic.Serilog.Enrichers.Web;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace Defra.ReportPackagingProxySpike.ReverseProxy.Utils.Logging;

[ExcludeFromCodeCoverage]
public static class WebApplicationBuilderExtensions
{
    public static void ConfigureLoggingAndTracing(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder
            .Services.AddOptions<TraceHeader>()
            .Bind(builder.Configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<IConfigureOptions<HeaderPropagationOptions>>(sp =>
        {
            var traceHeader = sp.GetRequiredService<IOptions<TraceHeader>>().Value;
            return new ConfigureOptions<HeaderPropagationOptions>(options =>
            {
                if (!string.IsNullOrWhiteSpace(traceHeader.Name))
                    options.Headers.Add(traceHeader.Name);
            });
        });
        builder.Services.TryAddSingleton<HeaderPropagationValues>();
        builder.Host.UseSerilog(ConfigureLogging);
    }

    private static void ConfigureLogging(
        HostBuilderContext hostBuilderContext,
        IServiceProvider services,
        LoggerConfiguration config
    )
    {
        var httpAccessor = services.GetRequiredService<IHttpContextAccessor>();
        var traceHeader = services.GetRequiredService<IOptions<TraceHeader>>().Value;
        var serviceVersion = Environment.GetEnvironmentVariable("SERVICE_VERSION") ?? "";

        config
            .ReadFrom.Configuration(hostBuilderContext.Configuration)
            .Enrich.WithEcsHttpContext(httpAccessor)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service.version", serviceVersion)
            .Enrich.WithCorrelationId(traceHeader.Name)
            .Filter.ByExcluding(x =>
                x.Level == LogEventLevel.Information
                && x.Properties.TryGetValue("RequestPath", out var path)
                && path.ToString().Contains("/health")
                && !x.MessageTemplate.Text.StartsWith("Request finished")
            );
    }
}
