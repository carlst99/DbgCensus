namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Provides a default implementation of <see cref="DbgCensus.EventStream.Commands.ICensusCommand"/>
    /// </summary>
    public record CensusCommandBase(string Action, string Service) : ICensusCommand;
}
