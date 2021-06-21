namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Defines a command sent to the Census event stream.
    /// </summary>
    public interface ICensusCommand
    {
        /// <summary>
        /// The action that the command will perform.
        /// </summary>
        string Action { get; }

        /// <summary>
        /// The websocket service that the command is intended for.
        /// </summary>
        string Service { get; }
    }
}
