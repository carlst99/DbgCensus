using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.Core.Objects;

/// <summary>
/// Represents a value that may be present, null, or missing.
/// Useful for JSON de/serialization on Census types with optional properties.
/// </summary>
/// <typeparam name="TValue">The type of value that can be stored in the optional.</typeparam>
public readonly struct Optional<TValue>
{
    private readonly TValue _value;

    /// <summary>
    /// Gets a value indicating whether or not this optional contains a value.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Gets the value of the optional.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the optional does not contain a value.</exception>
    public TValue Value => HasValue
        ? _value
        : throw new InvalidOperationException("This optional does not have a value");

    /// <summary>
    /// Initializes a new instance of the <see cref="Optional{TValue}"/> struct.
    /// </summary>
    /// <param name="value">The value of the optional.</param>
    public Optional(TValue value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>
    /// Determines whether the value of this optional is present and non-null.
    /// </summary>
    /// <returns>A value indicating whether the value of this optional is present and non-null.</returns>
    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsDefined()
        => HasValue && _value is not null;

    /// <summary>
    /// Determines whether the value of this optional is present and non-null.
    /// </summary>
    /// <param name="value">The defined value, if present.</param>
    /// <returns>A value indicating whether the value of this optional is present and non-null.</returns>
    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsDefined([NotNullWhen(true)] out TValue? value)
    {
        value = _value;

        return IsDefined();
    }

    /// <summary>
    /// Indicates whether this instance and a specified <see cref="Optional{TValue}"/> are equal.
    /// </summary>
    /// <param name="other">The <see cref="Optional{TValue}"/> to compare with the current instance.</param>
    /// <returns><c>true</c> if the instances are considered equal; otherwise, <c>false</c>.</returns>
    public bool Equals(Optional<TValue> other)
        => this.HasValue == other.HasValue
           && EqualityComparer<TValue>.Default.Equals(_value, other._value);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Optional<TValue> other
           && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(HasValue, _value);

    public override string ToString()
        => HasValue
            ? Value?.ToString() ?? "null"
            : "Missing";

    public static implicit operator Optional<TValue>(TValue value)
        => new(value);

    public static bool operator ==(Optional<TValue> left, Optional<TValue> right)
        => left.Equals(right);

    public static bool operator !=(Optional<TValue> left, Optional<TValue> right)
        => !left.Equals(right);
}
