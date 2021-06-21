namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Tells the Census event stream to echo back the payload.
    /// </summary>
    /// <typeparam name="T">The type of payload.</typeparam>
    /// <param name="Payload">The payload to echo.</param>
    public record EchoCommand<T>(T Payload)
        : CensusCommandBase("echo", "event");
}
