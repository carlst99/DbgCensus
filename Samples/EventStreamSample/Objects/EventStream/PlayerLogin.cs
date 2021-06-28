using System;

namespace EventStreamSample.Objects.EventStream
{
    public record PlayerLogin(ulong CharacterId, DateTimeOffset Timestamp, World WorldId);
}
