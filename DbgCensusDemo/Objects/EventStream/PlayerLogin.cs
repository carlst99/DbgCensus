using System;

namespace DbgCensusDemo.Objects.EventStream
{
    public record PlayerLogin(ulong CharacterId, DateTimeOffset Timestamp, World WorldId);
}
