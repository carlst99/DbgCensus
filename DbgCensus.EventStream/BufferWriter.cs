using System;
using System.Buffers;

namespace DbgCensus.EventStream;

/// <summary>
/// Implements an <see cref="IBufferWriter{T}"/> that uses
/// an array as a backing store.
/// </summary>
/// <remarks>
/// This implementation differs to that of the
/// <see cref="ArrayBufferWriter{T}"/> in that
/// it more restrictive and doesn't clear the
/// underlying buffer, rather only resetting the index.
/// </remarks>
/// <typeparam name="T">The type of buffer to write to.</typeparam>
public class BufferWriter<T> : IBufferWriter<T>
{
    private const int DefaultInitialBufferSize = 256;

    private T[] _buffer;

    /// <summary>
    /// Gets the position into the underlying buffer
    /// at which retrieved memory blocks will begin.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Gets the remaining space in the underlying buffer
    /// that may be written before it needs to be expanded.
    /// </summary>
    public int FreeCapacity => _buffer.Length - Index;

    /// <summary>
    /// Retrieves a span instance representing the data written to the underlying buffer.
    /// </summary>
    public Span<T> WrittenSpan => _buffer.AsSpan(0, Index);

    /// <summary>
    /// Retrieves a memory instance representing the data written to the underlying buffer.
    /// </summary>
    public Memory<T> WrittenMemory => _buffer.AsMemory(0, Index);

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferWriter{T}"/> class.
    /// </summary>
    public BufferWriter()
        : this(DefaultInitialBufferSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferWriter{T}"/> class.
    /// </summary>
    /// <param name="initialSize">The initial length of the underlying buffer.</param>
    public BufferWriter(int initialSize)
    {
        _buffer = new T[initialSize];
    }

    /// <summary>
    /// Retrieves a span of the underlying buffer that
    /// is, at minimum, the length of the <paramref name="sizeHint"/>.
    /// The retrieved span is not guaranteed to be clean.
    /// </summary>
    /// <param name="sizeHint">The minimum size of the span to retrieve.</param>
    /// <returns>A span instance representing a slice of the underlying buffer.</returns>
    public Span<T> GetSpan(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);

        return _buffer.AsSpan(Index);
    }

    /// <summary>
    /// Retrieves a span of the underlying buffer that
    /// is, at minimum, the length of the <paramref name="sizeHint"/>.
    /// The retrieved span is not guaranteed to be clean.
    /// </summary>
    /// <param name="sizeHint">The minimum size of the span to retrieve.</param>
    /// <returns>A memory instance representing a slice of the underlying buffer.</returns>
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);

        return _buffer.AsMemory(Index);
    }

    /// <summary>
    /// Advances the <see cref="Index"/> by the given amount.
    /// </summary>
    /// <param name="amount">The amount to advance.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="amount"/> is invalid.
    /// </exception>
    public void Advance(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (amount + Index > _buffer.Length)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(amount),
                "Cannot advance past the end of the buffer."
            );
        }

        Index += amount;
    }

    /// <summary>
    /// Resets the <see cref="Index"/>.
    /// </summary>
    public void Reset()
        => Index = 0;

    private void CheckAndResizeBuffer(int sizeHint)
    {
        if (sizeHint < 0)
            throw new ArgumentOutOfRangeException(nameof(sizeHint));

        if (sizeHint == 0)
            sizeHint = 1;

        if (sizeHint < FreeCapacity)
            return;

        int currentLength = _buffer.Length;

        // Attempt to grow by the larger of the sizeHint and double the current size.
        int growBy = Math.Max(sizeHint, currentLength);

        if (currentLength == 0)
            growBy = Math.Max(growBy, DefaultInitialBufferSize);

        int newSize = currentLength + growBy;
        Array.Resize(ref _buffer, newSize);
    }
}
