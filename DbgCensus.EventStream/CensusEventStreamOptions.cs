namespace DbgCensus.EventStream
{
    public class CensusEventStreamOptions
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

        public CensusEventStreamOptions()
        {
            RootEndpoint = "wss://push.planetside2.com";
            ServiceId = "example";
            Environment = "ps2";
        }
    }
}
