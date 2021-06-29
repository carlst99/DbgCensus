using System.Text.Json.Serialization;

namespace DbgCensus.EventStream.Objects.Event
{
    /// <summary>
    /// A heartbeat object sent by the event stream, to keep the connection alive.
    /// </summary>
    public record Heartbeat : EventStreamObjectBase
    {
        public record OnlineModel()
        {
            [JsonPropertyName("EventServerEndpoint_Connery_1")]
            public bool Connery { get; init; }

            [JsonPropertyName("EventServerEndpoint_Miller_10")]
            public bool Miller { get; init; }

            [JsonPropertyName("EventServerEndpoint_Cobalt_13")]
            public bool Cobalt { get; init; }

            [JsonPropertyName("EventServerEndpoint_Emerald_17")]
            public bool Emerald { get; init; }

            [JsonPropertyName("EventServerEndpoint_Jaeger_19")]
            public bool Jaeger { get; init; }

            [JsonPropertyName("EventServerEndpoint_Briggs_25")]
            public bool Briggs { get; init; }

            [JsonPropertyName("EventServerEndpoint_Soltech_40")]
            public bool Soltech { get; init; }

            public override string ToString()
                => $"{ nameof(Connery) }: { Connery } | " +
                   $"{ nameof(Miller) }: { Miller } | " +
                   $"{ nameof(Cobalt) }: { Cobalt } | " +
                   $"{ nameof(Emerald) }: { Emerald } | " +
                   $"{ nameof(Jaeger) }: { Jaeger } | " +
                   $"{ nameof(Briggs) }: { Briggs } | " +
                   $"{ nameof(Soltech) }: { Soltech }";
        }

        public OnlineModel Online { get; init; }

        public Heartbeat()
        {
            Online = new OnlineModel();
        }

        public override string ToString()
            => $"Endpoint status: { Online }";
    }
}
