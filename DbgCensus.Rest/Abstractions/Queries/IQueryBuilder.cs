﻿using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// Defines functions to build a query URI for the Census REST API.
/// </summary>
public interface IQueryBuilder
{
    /// <summary>
    /// Gets the collection that the query is being performed on.
    /// </summary>
    string? CollectionName { get; }

    /// <summary>
    /// Builds the query URI.
    /// </summary>
    Uri ConstructEndpoint();

    /// <summary>
    /// Sets the type of query to perform. Known as the 'Verb' by the Census REST API.
    /// </summary>
    /// <param name="type">The query type / verb.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder OfQueryType(QueryType type);

    /// <summary>
    /// Sets the collection to perform the query on.
    /// </summary>
    /// <param name="collection">The name of the collection.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder OnCollection(string collection);

    /// <summary>
    /// Limits the number of items returned by the query. If used in tandem with <see cref="WithDistinctFieldValues"/>,
    /// limits the number of distinct values returned.
    /// </summary>
    /// <param name="limit">The maximum number of items.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithLimit(int limit);

    /// <summary>
    /// Limits the number of items returned from each database.
    /// More predictable than <see cref="WithLimit(int)"/> for collections
    /// that are spread across multiple databases, such as ps2/character.
    /// </summary>
    /// <param name="limit">The number of items to return per database.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithLimitPerDatabase(int limit);

    /// <summary>
    /// Return items starting at the Nth index of the internal query.
    /// Use in tandem with <see cref="WithSortOrder(string, SortOrder)"/>
    /// </summary>
    /// <remarks>
    /// This will have inconsistent behaviour when querying collections
    /// that span multiple databases, such as ps2/character.
    /// </remarks>
    /// <param name="index">The index to return items from.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithStartIndex(int index);

    /// <summary>
    /// Applies a filter to the query.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="field">The collection field to filter on.</param>
    /// <param name="modifier">The comparison operator.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder Where<T>(string field, SearchModifier modifier, T filterValue) where T : notnull;

    /// <summary>
    /// Applies a multi-value filter to the query
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="field">The collection field to filter on.</param>
    /// <param name="modifier">The comparison operator.</param>
    /// <param name="filterValues">The value to filter by.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WhereAll<T>(string field, SearchModifier modifier, IEnumerable<T> filterValues) where T : notnull;

    /// <summary>
    /// Sorts items in the result. Sorting can be performed on multiple fields.
    /// </summary>
    /// <param name="fieldName">The name of the field to sort on.</param>
    /// <param name="order">The sorting order.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithSortOrder(string fieldName, SortOrder order = SortOrder.Ascending);

    /// <summary>
    /// When filtering using a regex, returns exact matches at the top of the response list.
    /// </summary>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithExactMatchesFirst();

    /// <summary>
    /// Joins data from another collection to this result.
    /// </summary>
    /// <param name="toCollection">The name of the collection to join to.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance to configure the join.</returns>
    IJoinBuilder AddJoin(string toCollection);

    /// <summary>
    /// Joins data from another collection to this result.
    /// </summary>
    /// <param name="toCollection">The name of the collection to join to.</param>
    /// <param name="configureJoin">A delegate to configure the join with.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder AddJoin(string toCollection, Action<IJoinBuilder> configureJoin);

    /// <summary>
    /// Re-formats the returned data by placing it into groups based on a given field.
    /// </summary>
    /// <param name="onField">Sets the field to group data by. Will be removed from the data source.</param>
    /// <returns>The <see cref="ITreeBuilder"/> instance to configure the tree.</returns>
    ITreeBuilder WithTree(string onField);

    /// <summary>
    /// Re-formats the returned data by placing it into groups based on a given field.
    /// </summary>
    /// <param name="onField">Sets the field to group data by. Will be removed from the data source.</param>
    /// <param name="configureTree">A delegate to configure the tree with.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithTree(string onField, Action<ITreeBuilder> configureTree);

    /// <summary>
    /// Performs a pre-determined resolve. Multiple resolves can be made in the same query.
    /// </summary>
    /// <remarks>
    /// Note that the resolve will only function if the initial query
    /// is showing the field that the resolve is keyed on.
    /// </remarks>
    /// <param name="resolveTo">The resolve to make.</param>
    /// <param name="showFields">The fields to be shown from the resolved collection.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder AddResolve(string resolveTo, params string[] showFields);

    /// <summary>
    /// Only includes the provided fields in the result.
    /// This method is incompatible with <see cref="HideFields(string[])"/>.
    /// </summary>
    /// <param name="fieldNames">The names of the fields that should be shown in the result.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder ShowFields(params string[] fieldNames);

    /// <summary>
    /// Includes all but the provided fields in the result.
    /// This method is incompatible with <see cref="ShowFields(string[])"/>.
    /// </summary>
    /// <param name="fieldNames">The names of the fields that should be hidden from the result.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder HideFields(params string[] fieldNames);

    /// <summary>
    /// Only returns items in which the specified field/s exist, regardless of the field values.
    /// </summary>
    /// <param name="fieldNames">Names of the fields that must exist.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder HasFields(params string[] fieldNames);

    /// <summary>
    /// Only returns the specified translation for internationalized fields.
    /// </summary>
    /// <param name="languageCode">The locale to return.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithLanguage(string languageCode);

    /// <summary>
    /// Indicates that filters/searches will be performed without using case-sensitive comparison.
    /// </summary>
    /// <remarks>
    /// Using this command might slow down your query. If a lower case version of a field is available,
    /// use that instead for a faster result.
    /// </remarks>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder IsCaseInsensitive();

    /// <summary>
    /// Includes fields that have a null value in the response.
    /// </summary>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithNullFields();

    /// <summary>
    /// Includes the times taken for server-side queries and resolves to be made, in the response.
    /// </summary>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithTimings();

    /// <summary>
    /// Prevents the query from being re-attempted after a failure.
    /// </summary>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithoutOneTimeRetry();

    /// <summary>
    /// Gets the distinct values of the provided field.
    /// </summary>
    /// <param name="fieldName">The field to get distinct values of.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithDistinctFieldValues(string fieldName);

    /// <summary>
    /// Sets the Census service ID to perform the query with.
    /// </summary>
    /// <param name="serviceId">A valid Census service id.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithServiceId(string serviceId);

    /// <summary>
    /// Sets the namespace to perform the query on.
    /// </summary>
    /// <param name="censusNamespace">The namespace.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder OnNamespace(string censusNamespace);

    /// <summary>
    /// Adds a custom parameter to the query.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The <see cref="IQueryBuilder"/> instance so that calls may be chained.</returns>
    IQueryBuilder WithCustomParameter(string parameter);
}
