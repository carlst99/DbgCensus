using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.Objects.Control;
using DbgCensus.EventStream.Abstractions.Objects.Events;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using DbgCensus.EventStream.Objects.Commands;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.ControlPayloads;

/// <summary>
/// <para>
/// Utilising something along the lines of this handler in your own project is NEAR MANDATORY.
/// You will need to resend your subscription every time the websocket drops your connection.
/// And unfortunately, that can happen fairly frequently.
/// </para>
/// <para>
/// It is also highly recommended that you refresh your subscription every 10m or so.
/// Otherwise, Census will start dropping off certain events entirely.
/// </para>
/// </summary>
public class ConnectionStateChangedPayloadHandler : IPayloadHandler<IConnectionStateChanged>
{
    private readonly ILogger<ConnectionStateChangedPayloadHandler> _logger;
    private readonly IPayloadContext _context;
    private readonly IEventStreamClientFactory _clientFactory;

    public ConnectionStateChangedPayloadHandler
    (
        ILogger<ConnectionStateChangedPayloadHandler> logger,
        IPayloadContext context,
        IEventStreamClientFactory clientFactory
    )
    {
        _logger = logger;
        _context = context;
        _clientFactory = clientFactory;
    }

    public async Task HandleAsync(IConnectionStateChanged payload, CancellationToken ct = default)
    {
        _logger.LogWarning
        (
            "Event stream connection state changed for the client {clientName}: we are now {state}!",
            _context.DispatchingClientName,
            payload.Connected ? "connected" : "disconnected"
        );

        if (!payload.Connected)
            return;

        IEventStreamClient client = _clientFactory.GetClient(_context.DispatchingClientName);
        await client.SendCommandAsync
        (
            new Subscribe
            (
                new string[] { "all" },
                new string[] { EventNames.FacilityControl, EventNames.PlayerLogin, EventNames.PlayerLogout },
                Worlds: new string[] { "all" }
            ),
            ct
        ).ConfigureAwait(false);
    }
}
