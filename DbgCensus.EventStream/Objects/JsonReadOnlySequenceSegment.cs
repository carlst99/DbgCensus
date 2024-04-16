using System;
using System.Buffers;

namespace DbgCensus.EventStream.Objects;

internal sealed class JsonReadOnlySequenceSegment : ReadOnlySequenceSegment<byte>, IDisposable
{
    private readonly IMemoryOwner<byte> _data;

    public JsonReadOnlySequenceSegment(IMemoryOwner<byte> data, int dataLength)
    {
        _data = data;
        base.Memory = data.Memory[..dataLength];
    }

    public JsonReadOnlySequenceSegment(JsonReadOnlySequenceSegment previous, IMemoryOwner<byte> data, int dataLength)
        : this(data, dataLength)
    {
        base.RunningIndex = previous.Memory.Length;
        previous.Next = this;
    }

    /// <summary>
    /// Disposes of this segment, and any linked segments.
    /// </summary>
    public void Dispose()
    {
        JsonReadOnlySequenceSegment? seg = this;
        do
        {
            // Start dispose logic
            seg._data.Dispose();
            // End dispose logic

            seg = (JsonReadOnlySequenceSegment?)seg.Next;
        } while (seg is not null);
    }
}
