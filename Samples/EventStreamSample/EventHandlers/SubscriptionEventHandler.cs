using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Objects;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers
{
    public class SubscriptionEventHandler : ICensusEventHandler<Subscription>
    {
        private readonly ILogger<SubscriptionEventHandler> _logger;

        public SubscriptionEventHandler(ILogger<SubscriptionEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(Subscription censusEvent, CancellationToken ct = default)
        {
            _logger.LogInformation
            (
                "Current subscription changed! Subscribed to:" +
                "\n- {charCount} characters" +
                "\n- On worlds {worlds}" +
                "\n- To events {events}" +
                "\n- Logical AND characters with worlds: {logicalAnd}",
                censusEvent.CharacterCount,
                string.Join(", ", censusEvent.Worlds),
                string.Join(", ", censusEvent.EventNames),
                censusEvent.LogicalAndCharactersWithWorlds
            );

            return Task.CompletedTask;
        }
    }
}
