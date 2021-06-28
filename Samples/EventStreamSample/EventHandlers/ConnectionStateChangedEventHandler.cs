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
        private readonly ICensusEventStreamClient _client;

        public ConnectionStateChangedEventHandler(ILogger<ConnectionStateChangedEventHandler> logger, ICensusEventStreamClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task HandleAsync(ConnectionStateChanged censusEvent, CancellationToken ct = default)
        {
            if (!censusEvent.Connected)
            {
                _logger.LogWarning("Event stream connection state changed: we are now disconnected!");
                return;
            }

            await _client.SendCommandAsync
            (
                new SubscribeCommand
                (
                    new string[] { "all" },
                    new string[] { "PlayerLogin", "PlayerLogout" },
                    true,
                    new string[] { "all" }
                ),
                ct
            ).ConfigureAwait(false);
        }
    }
}
