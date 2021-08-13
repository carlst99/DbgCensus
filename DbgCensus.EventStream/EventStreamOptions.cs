using System.Text.Json;

namespace DbgCensus.EventStream
{
    public class EventStreamOptions
    {
        /// <summary>
        /// The root endpoint of the Census event stream endpoint.
        /// </summary>
        public string RootEndpoint { get; set; }

        /// <summary>
        /// The service ID used to authenticate with the Census API.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// The Census environment to retrieve data from.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The JSON options to use when deserializing events.
        /// </summary>
        public JsonSerializerOptions DeserializationOptions { get; set; }

        /// <summary>
        /// The JSON options to use when serializing commands.
        /// </summary>
        public JsonSerializerOptions SerializationOptions { get; set; }

        public EventStreamOptions()
        {
            RootEndpoint = "wss://push.planetside2.com";
            ServiceId = "example";
            Environment = "ps2";
            DeserializationOptions = new JsonSerializerOptions();
            SerializationOptions = new JsonSerializerOptions();
        }
    }
}
