namespace DbgCensus.EventStream;

// Note to developer: If you rename this class, you will break the code of everyone who uses nameof() to retrieve this from their appsettings.
public class EventStreamOptions
{
    /// <summary>
    /// A default key under which configuration for this object can be written.
    /// </summary>
    public const string CONFIG_KEY = "EventStreamOptions";

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
    /// Gets or sets the amount of time to wait before attempting to
    /// reconnect when the event stream session is closed erroneously.
    /// </summary>
    public int ReconnectionDelayMilliseconds { get; set; }

    public EventStreamOptions()
    {
        RootEndpoint = "wss://push.planetside2.com";
        ServiceId = "example";
        Environment = "ps2";
        ReconnectionDelayMilliseconds = 5000;
    }
}
