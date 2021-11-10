using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.System;

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
            "\n- {charCount} characters: {characters}" +
            "\n- on worlds {worlds}" +
            "\n- for events {events}" +
            "\n- logical AND characters with worlds: {logicalAnd}",
            censusEvent.CharacterCount,
            string.Join(", ", censusEvent.Characters),
            string.Join(", ", censusEvent.Worlds),
            string.Join(", ", censusEvent.EventNames),
            censusEvent.LogicalAndCharactersWithWorlds
        );

        return Task.CompletedTask;
    }
}
