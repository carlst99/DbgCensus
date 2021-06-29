using System;

namespace EventStreamSample.Objects
{
    public record PlayerLogout(ulong CharacterId, DateTimeOffset Timestamp, World WorldId);
}
