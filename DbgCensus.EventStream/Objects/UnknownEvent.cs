using DbgCensus.EventStream.Abstractions.Objects;

namespace DbgCensus.EventStream.Objects
{
    public record UnknownEvent : IEventStreamObject
    {
        /// <inheritdoc />
        public string DispatchingClientName { get; set; }
        public string RawData { get; init; }

        public UnknownEvent(string dispatchingClientName, string rawData)
        {
            DispatchingClientName = dispatchingClientName;
            RawData = rawData;
        }
    }
}
