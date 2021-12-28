using System;

namespace DbgCensus.EventStream.EventHandlers;

/// <summary>
/// Represents a set of options consumed by an <see cref="EventHandlingEventStreamClient"/>.
/// </summary>
public class EventHandlingClientOptions : EventStreamOptions
{
    /// <summary>
    /// Gets or sets the interval in milliseconds at which the current subscription
    /// of an <see cref="EventHandlingEventStreamClient"/> will be refreshed.
    /// </summary>
    public TimeSpan SubscriptionRefreshIntervalMilliseconds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlingClientOptions"/> class.
    /// </summary>
    public EventHandlingClientOptions()
    {
        SubscriptionRefreshIntervalMilliseconds = TimeSpan.FromMinutes(15);
    }
}
