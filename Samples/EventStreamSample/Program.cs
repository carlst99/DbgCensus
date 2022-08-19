using DbgCensus.EventStream;
using DbgCensus.EventStream.EventHandlers.Extensions;
using EventStreamSample.EventHandlers;
using EventStreamSample.EventHandlers.ControlPayloads;
using EventStreamSample.EventHandlers.PreDispatch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace EventStreamSample;

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
                services.Configure<EventStreamOptions>(hostContext.Configuration.GetSection(nameof(EventStreamOptions)));

                services.AddCensusEventHandlingServices()
                        .RegisterPreDispatchHandler<DuplicatePreventionPreDispatchHandler>()
                        .AddPayloadHandler<ConnectionStateChangedPayloadHandler>()
                        .AddPayloadHandler<HeartbeatPayloadHandler>()
                        .AddPayloadHandler<ServiceStateChangedPayloadHandler>()
                        .AddPayloadHandler<SubscriptionPayloadHandler>()
                        .AddPayloadHandler<FacilityControlPayloadHandler>()
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
