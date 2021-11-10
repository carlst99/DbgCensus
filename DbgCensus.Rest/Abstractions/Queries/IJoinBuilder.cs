using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// Defines functions to build a join command for the Census REST API.
/// </summary>
public interface IJoinBuilder
{
    /// <summary>
    /// The collection to join to.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder ToCollection(string collectionName);

    /// <summary>
    /// The ID field on the base collection to join on.
    /// </summary>
    /// <param name="fieldName">The name of the field to join on.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder OnField(string fieldName);

    /// <summary>
    /// The ID field on the joined collection to join to.
    /// </summary>
    /// <param name="fieldName">The field to join to.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder ToField(string fieldName);

    /// <summary>
    /// Sets a value indicating that the joined data is a list.
    /// </summary>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder IsList();

    /// <summary>
    /// Only includes the provided fields in the result. This method is incompatible with <see cref="HideFields(string[])"/>.
    /// </summary>
    /// <param name="fieldNames">The names of the fields that should be shown in the result.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder ShowFields(params string[] fieldNames);

    /// <summary>
    /// Includes all but the provided fields in the result. This method is incompatible with <see cref="ShowFields(string[])"/>.
    /// </summary>
    /// <param name="fieldNames">The names of the fields that should be hidden from the result.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>/
    IJoinBuilder HideFields(params string[] fieldNames);

    /// <summary>
    /// Creates a new field on the base collection, where the joined data should be injected to.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder InjectAt(string name);

    /// <summary>
    /// Performs a filter for a value on the joined collection.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="field">The field in the joined collection to filter on.</param>
    /// <param name="modifier">The comparison operator.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder Where<T>(string field, SearchModifier modifier, T filterValue) where T : notnull;

    /// <summary>
    /// By default, all queries are treated as an 'outer' join. As in SQL, this means that results will be included for joins that do not match the criteria defined via terms. You can set a query to use 'inner' join behaviour, which allows filtering of a parent join via a term defined for its child if both are using 'inner' join behaviour.
    /// </summary>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder IsInnerJoin();

    /// <summary>
    /// Creates a nested join on this join.
    /// </summary>
    /// <param name="toCollection">The name of the collection to create a nested join to.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance to configure the nested join.</returns>
    IJoinBuilder AddNestedJoin(string toCollection);

    /// <summary>
    /// Creates a nested join on this join.
    /// </summary>
    /// <param name="toCollection">The name of the collection to create a nested join to.</param>
    /// <param name="configureJoin">A delegate to configure the join.</param>
    /// <returns>The <see cref="IJoinBuilder"/> instance so that calls may be chained.</returns>
    IJoinBuilder AddNestedJoin(string toCollection, Action<IJoinBuilder> configureJoin);
}
