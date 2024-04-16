using BenchmarkDotNet.Attributes;
using Microsoft.IO;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Benchmarks;

[MemoryDiagnoser]
public class JsonDeserializationBenchmarks
{
    private const int WEBSOCKET_BUFFER_SIZE = 4096;

    private readonly RecyclableMemoryStreamManager _msManager = new();

    private bool _hasData;
    private List<Memory<byte>> _smallData = new();
    private List<Memory<byte>> _mediumData = new();
    private List<Memory<byte>> _largeData = new();

    public IEnumerable<object> GetData()
    {
        if (!_hasData)
        {
            _smallData = File.ReadAllBytes("Data\\SmallJsonData.json")
                .Chunk(WEBSOCKET_BUFFER_SIZE)
                .Select(x => x.AsMemory())
                .ToList();

            _mediumData = File.ReadAllBytes("Data\\MediumJsonData.json")
                .Chunk(WEBSOCKET_BUFFER_SIZE)
                .Select(x => x.AsMemory())
                .ToList();

            _largeData = File.ReadAllBytes("Data\\LargeJsonData.json")
                .Chunk(WEBSOCKET_BUFFER_SIZE)
                .Select(x => x.AsMemory())
                .ToList();

            _hasData = true;
        }

        yield return new JsonBenchData { Data = _smallData };
        yield return new JsonBenchData { Data = _mediumData };
        yield return new JsonBenchData { Data = _largeData };
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetData))]
    public JsonDocument? DeserializeStream(JsonBenchData benchData)
    {
        using RecyclableMemoryStream ms = _msManager.GetStream();
        foreach (Memory<byte> element in benchData.Data)
            ms.Write(element.Span);

        ms.Seek(0, SeekOrigin.Begin);
        return JsonSerializer.Deserialize<JsonDocument>(ms);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetData))]
    public JsonDocument DeserializeArraySegment(JsonBenchData benchData)
    {
        JsonReadOnlySequenceSegment? startSeg = null;
        JsonReadOnlySequenceSegment? endSeg = null;
        int endSegIndex = 0;

        foreach (Memory<byte> data in benchData.Data)
        {
            IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent(WEBSOCKET_BUFFER_SIZE);
            data.CopyTo(buffer.Memory);
            endSegIndex = data.Length;

            if (startSeg is null)
                startSeg = new JsonReadOnlySequenceSegment(buffer, data.Length);
            else if (endSeg is null)
                endSeg = new JsonReadOnlySequenceSegment(startSeg, buffer, data.Length);
            else
                endSeg = new JsonReadOnlySequenceSegment(endSeg, buffer, data.Length);
        }

        ReadOnlySequence<byte> sequence = endSeg is null
            ? new ReadOnlySequence<byte>(startSeg!.Memory)
            : new ReadOnlySequence<byte>(startSeg!, 0, endSeg, endSegIndex);

        JsonDocument doc = JsonDocument.Parse(sequence);
        startSeg!.Dispose();
        return doc;
    }

    public readonly struct JsonBenchData
    {
        public List<Memory<byte>> Data { get; init; }

        public override string ToString()
            => $"{Data.Count} segment";
    }

    private sealed class JsonReadOnlySequenceSegment : ReadOnlySequenceSegment<byte>, IDisposable
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
            base.RunningIndex = previous.RunningIndex + previous.Memory.Length;
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

}
