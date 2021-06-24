using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream
{
    public sealed class EventHandlingEventStreamClient : CensusEventStreamClient
    {
        public EventHandlingEventStreamClient(ILogger<EventHandlingEventStreamClient> logger, ClientWebSocket webSocket)
            : base(logger, webSocket)
        {
        }

        public EventHandlingEventStreamClient(ILogger<EventHandlingEventStreamClient> logger, ClientWebSocket webSocket, JsonSerializerOptions jsonOptions)
            : base(logger, webSocket, jsonOptions)
        {
        }

        protected override async Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default)
        {
            using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

            // TODO: Discover type, build responder system
        }
    }
}
