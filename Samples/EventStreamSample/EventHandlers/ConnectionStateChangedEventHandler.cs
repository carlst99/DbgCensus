using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Commands;
using DbgCensus.EventStream.Objects.Push;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers
{
    public class ConnectionStateChangedEventHandler : ICensusEventHandler<ConnectionStateChanged>
    {
        private readonly ILogger<ConnectionStateChangedEventHandler> _logger;
        private readonly ICensusEventStreamClientFactory _clientFactory;

        public ConnectionStateChangedEventHandler(ILogger<ConnectionStateChangedEventHandler> logger, ICensusEventStreamClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task HandleAsync(ConnectionStateChanged censusEvent, CancellationToken ct = default)
        {
            _logger.LogWarning("Event stream connection state changed: we are now {state}!", censusEvent.Connected ? "connected" : "disconnected");

            if (!censusEvent.Connected)
                return;

            ICensusEventStreamClient client = _clientFactory.GetClient(censusEvent.DispatchingClientName);
            await client.SendCommandAsync
            (
                new SubscribeCommand
                (
                    new string[] { "all" },
                    new string[] { "PlayerLogin", "PlayerLogout" },
                    worlds: new string[] { "all" }
                ),
                ct
            ).ConfigureAwait(false);
        }
    }
}
