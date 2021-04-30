using DbgCensus.Rest.Abstractions.Queries;
using System;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Stores the data required to perform a search on a collection in the Census REST API.
    /// </summary>
    internal struct QueryFilter
    {
        /// <summary>
        /// Gets the property to filter on.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Gets the filter value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the search modifier.
        /// </summary>
        public SearchModifier Modifier { get; }

        /// <summary>
        /// Stores the data required to perform a search on a collection in the Census REST API.
        /// </summary>
        /// <param name="property">The collection property to search on.</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <param name="modifier">The search modifier.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null or empty string is passed in as the 'property' and/or 'filterValue' parameter/s.</exception>
        public QueryFilter(string property, object filterValue, SearchModifier modifier)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException(nameof(property));

            if (filterValue is null || string.IsNullOrEmpty(filterValue.ToString()) || filterValue.ToString() == filterValue.GetType().FullName)
                throw new ArgumentException(nameof(filterValue), "filterValue must not be null and filterValue.ToString() must be fully implemented.");

            Property = property;
            Value = filterValue;
            Modifier = modifier;
        }

        /// <summary>
        /// Constructs a value that can be used to perform a search within a Census query.
        /// </summary>
        /// <returns>A well-formed filter string.</returns>
        public string GetFilterString() => Property + "=" + Modifier.Value + Value;
    }
}
