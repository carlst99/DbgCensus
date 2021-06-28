using DbgCensus.EventStream;
using DbgCensus.EventStream.Extensions;
using DbgCensusDemo.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DbgCensusDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<CensusEventStreamOptions>(hostContext.Configuration.GetSection(nameof(CensusEventStreamOptions)));

                    services.AddCensusEventStreamServices()
                            .AddEventHandler<HeartbeatEventHandler>();

                    services.AddHostedService<Worker>();
                });
    }
}
