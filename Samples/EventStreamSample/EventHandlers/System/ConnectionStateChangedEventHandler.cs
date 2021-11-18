using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.Commands;
using DbgCensus.EventStream.EventHandlers.Objects.Push;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using DbgCensus.EventStream.Abstractions.Objects.Events;

namespace EventStreamSample.EventHandlers.System;

/// <summary>
/// Utilising something along the lines of this handler in your own project is NEAR MANDATORY.
/// You will need to resend your subscription every time the websocket drops your connection.
/// And unfortunately, that happens fairly frequently.
/// </summary>
public class ConnectionStateChangedEventHandler : ICensusEventHandler<ConnectionStateChanged>
{
    private readonly ILogger<ConnectionStateChangedEventHandler> _logger;
    private readonly IEventStreamClientFactory _clientFactory;

    public ConnectionStateChangedEventHandler(ILogger<ConnectionStateChangedEventHandler> logger, IEventStreamClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task HandleAsync(ConnectionStateChanged censusEvent, CancellationToken ct = default)
    {
        _logger.LogWarning("Event stream connection state changed: we are now {state}!", censusEvent.Connected ? "connected" : "disconnected");

        if (!censusEvent.Connected)
            return;

        IEventStreamClient client = _clientFactory.GetClient(censusEvent.DispatchingClientName);
        await client.SendCommandAsync
        (
            new SubscribeCommand
            (
                new string[] { "all" },
                new string[] { EventNames.FacilityControl, EventNames.PlayerLogin, EventNames.PlayerLogout },
                worlds: new string[] { "all" }
            ),
            ct
        ).ConfigureAwait(false);
    }
}
