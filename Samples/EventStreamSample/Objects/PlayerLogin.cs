using System;

namespace EventStreamSample.Objects
{
    public record PlayerLogin(ulong CharacterId, DateTimeOffset Timestamp, World WorldId);
}
