﻿using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using System;

namespace DbgCensus.EventStream.EventHandlers.Services;

/// <summary>
/// Represents a service for storing an <see cref="IPayloadContext"/> object.
/// </summary>
public class PayloadContextInjectionService
{
    private IPayloadContext? _context;

    /// <summary>
    /// Gets or sets the stored context.
    /// </summary>
    public IPayloadContext Context
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
