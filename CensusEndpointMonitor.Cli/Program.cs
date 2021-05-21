using DbgCensus.Rest;
using DbgCensus.Rest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CensusEndpointMonitor.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(new LoggerConfiguration()
                    .WriteTo.Console()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                    .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Error)
                    .CreateLogger())
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddCensusRestServices();
                    services.Configure<CensusQueryOptions>((o) =>
                    {
                        o.LanguageCode = CensusLanguage.ENGLISH;
                        o.Namespace = CensusNamespace.PS2;
                        o.ServiceId = "example";
                        o.Limit = 10;
                    });
                });
    }
}
