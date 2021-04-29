using System;

namespace DbgCensus.Rest.Abstractions.Queries
{
    /// <summary>
    /// Provides functions to build a query string.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Builds the query URI.
        /// </summary>
        Uri ConstructEndpoint();

        /// <summary>
        /// The type of query to perform. Known as the 'Verb' by the Census REST API.
        /// </summary>
        /// <param name="type">The query type / verb.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery OfQueryType(QueryType type);

        /// <summary>
        /// The collection to perform the query on.
        /// </summary>
        /// <param name="collection">The name of the collection.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery On(string collection);

        /// <summary>
        /// Limits the number of items returned by the query.
        /// </summary>
        /// <param name="limit">The maximum number of items.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery WithLimit(uint limit);

        /// <summary>
        /// Limits the number of items returned from each database. More predictable than <see cref="WithLimit(uint)"/> for collections that are spread across multiple databases, such as ps2/character.
        /// </summary>
        /// <param name="limit">The number of items to return per database.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery WithLimitPerDatabase(uint limit);

        /// <summary>
        /// Performs a search on the collection.
        /// </summary>
        /// <param name="property">The collection property to filter on.</param>
        /// <returns>An <see cref="IQueryFilter"/> instance to set the filter term.</returns>
        IQuery Where(string property, string filterValue, SearchModifier modifier);

        /// <summary>
        /// When filtering using a regex, returns exact matches at the top of the response list.
        /// </summary>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery WithExactMatchesFirst();

        /// <summary>
        /// Performs a pre-determined resolve. Multiple resolves can be made in the same query.
        /// </summary>
        /// <remarks>Note that the resolve will only function if the initial query is showing the field that the resolve is keyed on.</remarks>
        /// <param name="resolveTo">The resolve to make.</param>
        /// <param name="showFields">The fields to be shown from the resolved collection.</param>
        IQuery WithResolve(string resolveTo, params string[] showFields);

        /// <summary>
        /// Only includes the provided fields in the result. This method is incompatible with <see cref="HideFields(string[])"/>.
        /// </summary>
        /// <param name="fieldNames">The names of the fields that should be shown in the result.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery ShowFields(params string[] fieldNames);

        /// <summary>
        /// Includes all but the provided fields in the result. This method is incompatible with <see cref="ShowFields(string[])"/>.
        /// </summary>
        /// <param name="fieldNames">The names of the fields that should be hidden from the result.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery HideFields(params string[] fieldNames);

        /// <summary>
        /// Only returns the specified translation for internationalized fields.
        /// </summary>
        /// <param name="languageCode">The locale to return.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery SetLanguage(string languageCode);

        /// <summary>
        /// Only returns items in which the specified field/s exist, regardless of their value.
        /// </summary>
        /// <param name="fieldNames">Names of the fields that must exist.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery HasFields(params string[] fieldNames);

        /// <summary>
        /// Indicates that filters/searches will be performed without using case-sensitive comparison.
        /// </summary>
        /// <remarks>Using this command might slow down your query. If a lower case version of a field is available, use that instead for a faster result.</remarks>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery IsCaseInsensitive();

        /// <summary>
        /// Includes fields that have a null value in the response.
        /// </summary>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery WithNullFields();

        /// <summary>
        /// Includes the times taken for server-side queries and resolves to be made, in the response.
        /// </summary>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery WithTimings();

        /// <summary>
        /// Prevents the query from being re-attempted after a failure. Useful for quick failure.
        /// </summary>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery WithoutOneTimeRetry();

        /// <summary>
        /// Gets the distinct values of the provided field.
        /// </summary>
        /// <param name="fieldName">The field to get distinct values of.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IQuery GetDistinctFieldValues(string fieldName);
    }
}
