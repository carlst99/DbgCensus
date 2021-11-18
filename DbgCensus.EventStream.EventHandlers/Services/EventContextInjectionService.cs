using DbgCensus.EventStream.EventHandlers.Abstractions;
using System;

namespace DbgCensus.EventStream.EventHandlers.Services;

/// <summary>
/// Represents a service for storing an <see cref="IEventContext"/> object.
/// </summary>
public class EventContextInjectionService
{
    private IEventContext? _context;

    /// <summary>
    /// Gets or sets the stored context.
    /// </summary>
    public IEventContext Context
    {
        get
        {
            if (_context is null)
                throw new InvalidOperationException("No context has been defined for this scope");

            return _context;
        }
        set
        {
            _context = value;
        }
    }
}
