using DbgCensus.EventStream;
using DbgCensus.EventStream.EventHandlers.Extensions;
using EventStreamSample.EventHandlers;
using EventStreamSample.EventHandlers.ControlPayloads;
using EventStreamSample.EventHandlers.PreDispatch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace EventStreamSample;

public static class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        SetupLogger(builder);

        builder.Services.Configure<EventStreamOptions>(builder.Configuration.GetSection(EventStreamOptions.CONFIG_KEY));

        builder.Services.AddCensusEventHandlingServices()
            // There should only ever be one instance of the duplicate prevention handler, hence the singleton scope
            .RegisterPreDispatchHandler<DuplicatePreventionPreDispatchHandler>(ServiceLifetime.Singleton)
            .AddPayloadHandler<ConnectionStateChangedPayloadHandler>()
            .AddPayloadHandler<HeartbeatPayloadHandler>()
            .AddPayloadHandler<ServiceStateChangedPayloadHandler>()
            .AddPayloadHandler<SubscriptionPayloadHandler>()
            .AddPayloadHandler<FacilityControlPayloadHandler>()
            .AddPayloadHandler<PlayerLogEventHandler>()
            .AddPayloadHandler<UnknownPayloadHandler>();

        builder.Services.AddHostedService<EventStreamWorker>();

        builder.Build().Run();
    }

    private static void SetupLogger(HostApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console
            (
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        builder.Services.AddSerilog();
    }
}
