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
        /// Gets the field to filter on.
        /// </summary>
        public string Field { get; }

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
        /// <param name="field">The collection property to search on.</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <param name="modifier">The search modifier.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null or empty string is passed in as the 'property' and/or 'filterValue' parameter/s.</exception>
        public QueryFilter(string field, object filterValue, SearchModifier modifier)
        {
            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException(nameof(field));

            if (filterValue is null || string.IsNullOrEmpty(filterValue.ToString()) || filterValue.ToString() == filterValue.GetType().FullName)
                throw new ArgumentException($"{ nameof(filterValue) } must not be null and { nameof(filterValue.ToString) } must be fully implemented.", nameof(filterValue));

            Field = field;
            Value = filterValue;
            Modifier = modifier;
        }

        /// <summary>
        /// Constructs a value that can be used to perform a search within a Census query.
        /// </summary>
        /// <returns>A well-formed filter string.</returns>
        public override string ToString() => Field + "=" + Modifier.Value + Value;
    }
}
