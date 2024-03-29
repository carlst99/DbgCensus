﻿using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries.Internal;

/// <summary>
/// Stores the data required to perform a search on a collection in the Census REST API.
/// </summary>
internal sealed class QueryFilter
{
    /// <summary>
    /// Gets the field to filter on.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Gets the filter value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the search modifier.
    /// </summary>
    public SearchModifier Modifier { get; }

    /// <summary>
    /// Stores the data required to perform a search on a collection in the Census REST API.
    /// </summary>
    /// <param name="field">The collection property to search on.</param>
    /// <param name="modifier">The search modifier.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <exception cref="ArgumentNullException">Thrown when a null or empty string is passed in as the 'property' and/or one of the 'filterValues' parameter/s.</exception>
    public QueryFilter(string field, SearchModifier modifier, string filterValue)
    {
        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException(nameof(field));

        if (string.IsNullOrEmpty(filterValue))
            throw new ArgumentNullException(nameof(filterValue));

        Field = field;
        Modifier = modifier;
        Value = filterValue;
    }

    /// <summary>
    /// Stores the data required to perform a search on a collection in the Census REST API.
    /// </summary>
    /// <param name="field">The collection property to search on.</param>
    /// <param name="modifier">The search modifier.</param>
    /// <param name="filterValues">The values to filter by.</param>
    /// <exception cref="ArgumentNullException">Thrown when a null or empty string is passed in as the 'property' and/or one of the 'filterValues' parameter/s.</exception>
    public QueryFilter(string field, SearchModifier modifier, IEnumerable<string> filterValues)
    {
        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException(nameof(field));

        Field = field;
        Modifier = modifier;
        Value = StringUtils.JoinWithoutNullOrEmptyValues(',', filterValues);
    }

    /// <summary>
    /// Constructs a value that can be used to perform a search within a Census query.
    /// </summary>
    /// <returns>A well-formed filter string.</returns>
    public override string ToString()
        => $"{Field}{GetEqualityModifier()}{Value}";

    private string GetEqualityModifier()
        => Modifier is SearchModifier.Equals
            ? "="
            : $"={(char)Modifier}";
}
