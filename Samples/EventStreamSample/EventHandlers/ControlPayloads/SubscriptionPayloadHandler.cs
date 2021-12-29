using DbgCensus.EventStream.Abstractions.Objects.Control;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.ControlPayloads;

public class SubscriptionPayloadHandler : IPayloadHandler<ISubscription>
{
    private readonly ILogger<SubscriptionPayloadHandler> _logger;

    public SubscriptionPayloadHandler(ILogger<SubscriptionPayloadHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ISubscription payload, CancellationToken ct = default)
    {
        _logger.LogInformation
        (
            "Current subscription changed! Subscribed to:" +
            "\n\t- {charCount} characters: {characters}" +
            "\n\t- on worlds {worlds}" +
            "\n\t- for events {events}" +
            "\n\t- logical AND characters with worlds: {logicalAnd}",
            payload.CharacterCount,
            string.Join(", ", payload.Characters ?? new[] { "none" }),
            string.Join(", ", payload.Worlds),
            string.Join(", ", payload.EventNames),
            payload.LogicalAndCharactersWithWorlds
        );

        return Task.CompletedTask;
    }
}
