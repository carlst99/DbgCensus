using DbgCensus.Core.Objects;
using System;

namespace EventStreamSample.Objects
{
    public record PlayerLogin(ulong CharacterId, DateTimeOffset Timestamp, WorldDefinition WorldId);
}
