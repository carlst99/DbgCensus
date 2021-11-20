using DbgCensus.EventStream;
using DbgCensus.EventStream.EventHandlers.Extensions;
using EventStreamSample.EventHandlers;
using EventStreamSample.EventHandlers.ControlPayloads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace EventStreamSample;

public static class Program
{
    public static void Main(string[] args)
        => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(o => o.ValidateScopes = true)
            .UseSerilog(GetLogger())
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<EventStreamOptions>(hostContext.Configuration.GetSection(nameof(EventStreamOptions)));

                services.AddCensusEventHandlingServices()
                        .AddPayloadHandler<ConnectionStateChangedPayloadHandler>()
                        .AddPayloadHandler<HeartbeatPayloadHandler>()
                        .AddPayloadHandler<ServiceStateChangedPayloadHandler>()
                        .AddPayloadHandler<SubscriptionPayloadHandler>()
                        .AddPayloadHandler<FacilityControlPayloadHandler>()
                        .AddPayloadHandler<PlayerLogEventHandler>()
                        .AddPayloadHandler<PlayerLogEventHandler>()
                        .AddPayloadHandler<UnknownPayloadHandler>();

                services.AddHostedService<EventStreamWorker>();
            });

    private static ILogger GetLogger()
        => new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
}
