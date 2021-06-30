using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Objects.Push;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers
{
    public class ConnectionStateChangedEventHandler : ICensusEventHandler<ConnectionStateChanged>
    {
        private readonly ILogger<ConnectionStateChangedEventHandler> _logger;

        public ConnectionStateChangedEventHandler(ILogger<ConnectionStateChangedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(ConnectionStateChanged censusEvent, CancellationToken ct = default)
        {
            _logger.LogWarning("Event stream connection state changed: we are now {state}!", censusEvent.Connected ? "connected" : "disconnected");

            if (!censusEvent.Connected)
                return;

            //if (censusEvent.EventStreamClient is null)
            //    return;

            //await censusEvent.EventStreamClient.SendCommandAsync
            //(
            //    new SubscribeCommand
            //    (
            //        new string[] { "all" },
            //        new string[] { "PlayerLogin", "PlayerLogout" },
            //        true,
            //        new string[] { "all" }
            //    ),
            //    ct
            //).ConfigureAwait(false);
        }
    }
}
