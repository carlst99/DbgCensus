using System;

namespace DbgCensusDemo.Objects.EventStream
{
    public record PlayerLogout(ulong CharacterId, DateTimeOffset Timestamp, World WorldId);
}
