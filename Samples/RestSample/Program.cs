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
        IHost host = Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(o => o.ValidateScopes = true)
            .UseSerilog(GetLogger())
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<CensusQueryOptions>(hostContext.Configuration.GetSection(nameof(CensusQueryOptions)));

                // Configure a second query options to point towards Sanctuary.Census
                services.Configure<CensusQueryOptions>("sanctuary", hostContext.Configuration.GetSection(nameof(CensusQueryOptions)));
                services.Configure<CensusQueryOptions>("sanctuary", o => o.RootEndpoint = "https://census.lithafalcon.cc");

                services.AddCensusRestServices();

                services.AddHostedService<Worker>();
            })
            .Build();

        await host.RunAsync();
    }

    private static ILogger GetLogger()
        => new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System.Net.Http.HttpClient.ICensusRestClient.ClientHandler", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient.ICensusRestClient.LogicalHandler", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
}
