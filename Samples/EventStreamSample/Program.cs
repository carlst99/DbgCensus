using DbgCensus.EventStream;
using DbgCensus.EventStream.EventHandlers.Extensions;
using DbgCensus.EventStream.Extensions;
using EventStreamSample.EventHandlers;
using EventStreamSample.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace EventStreamSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider(o => o.ValidateScopes = true)
                .UseSerilog(GetLogger())
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<CensusEventStreamOptions>(hostContext.Configuration.GetSection(nameof(CensusEventStreamOptions)));

                    services.AddCensusEventStreamServices()
                            .AddEventHandler<ConnectionStateChangedEventHandler>()
                            .AddEventHandler<HeartbeatEventHandler>()
                            .AddEventHandler<ServiceStateChangedEventHandler>()
                            .AddEventHandler<SubscriptionEventHandler>()
                            .AddEventHandler<PlayerLogEventHandler, PlayerLogin>(EventNames.PLAYER_LOGIN)
                            .AddEventHandler<PlayerLogEventHandler, PlayerLogout>(EventNames.PLAYER_LOGOUT)
                            .AddEventHandler<UnknownEventHandler>();

                    services.AddHostedService<Worker>();
                });

        private static ILogger GetLogger()
            => new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
    }
}
