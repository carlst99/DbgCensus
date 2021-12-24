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
        => await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(o => o.ValidateScopes = true)
            .UseSerilog(GetLogger())
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<CensusQueryOptions>(hostContext.Configuration.GetSection(nameof(CensusQueryOptions)));

                services.AddCensusRestServices();

                services.AddHostedService<Worker>();
            });

    private static ILogger GetLogger()
        => new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System.Net.Http.HttpClient.ICensusRestClient.ClientHandler", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient.ICensusRestClient.LogicalHandler", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
}
