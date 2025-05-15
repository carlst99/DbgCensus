using DbgCensus.Rest;
using DbgCensus.Rest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace RestSample;

public static class Program
{
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        SetupLogger(builder);

        builder.Services.Configure<CensusQueryOptions>(builder.Configuration.GetSection(CensusQueryOptions.CONFIG_KEY));

        // Configure a second query options to point towards Sanctuary.Census
        builder.Services.Configure<CensusQueryOptions>("sanctuary", builder.Configuration.GetSection(CensusQueryOptions.CONFIG_KEY));
        builder.Services.Configure<CensusQueryOptions>("sanctuary", o => o.RootEndpoint = "https://census.lithafalcon.cc");

        builder.Services.AddCensusRestServices();

        builder.Services.AddHostedService<Worker>();

        await builder.Build().RunAsync();
    }

    private static void SetupLogger(HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System.Net.Http.HttpClient.ICensusRestClient.ClientHandler", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient.ICensusRestClient.LogicalHandler", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console
            (
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        builder.Services.AddSerilog();
    }
}
