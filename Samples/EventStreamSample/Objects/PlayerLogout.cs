using DbgCensus.Core.Objects;
using System;

namespace EventStreamSample.Objects;

public record PlayerLogout(ulong CharacterId, DateTimeOffset Timestamp, WorldDefinition WorldId);
