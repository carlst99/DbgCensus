using System;

namespace EventStreamSample.Objects.EventStream
{
    public record PlayerLogout(ulong CharacterId, DateTimeOffset Timestamp, World WorldId);
}
