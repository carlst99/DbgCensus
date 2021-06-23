using DbgCensus.EventStream.Abstractions.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.Abstractions
{
    public interface ICensusEventStreamClient : IDisposable
    {
        Task ConnectAsync(CensusEventStreamOptions options, CancellationToken ct = default);
        Task DisconnectAsync(CancellationToken ct = default);
        Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand;
    }
}
