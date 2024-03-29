﻿using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects.Events;

/// <summary>
/// Represents a marker interface for an event.
/// </summary>
public interface IEvent : IPayload
{
    /// <summary>
    /// Gets the name of this event.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// Gets the time at which this event occured.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the world on which this event occured.
    /// </summary>
    WorldDefinition WorldID { get; }
}
