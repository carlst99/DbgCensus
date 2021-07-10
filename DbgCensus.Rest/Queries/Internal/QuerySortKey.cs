using DbgCensus.Rest.Abstractions.Queries;
using System;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Stores data required to perform a sort in the Census REST API.
    /// </summary>
    internal class QuerySortKey
    {
        /// <summary>
        /// The field to sort on.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// The sort order.
        /// </summary>
        public SortOrder Order { get; }

        /// <summary>
        /// Stores data required to perform a sort in the Census REST API.
        /// </summary>
        /// <param name="fieldName">The field to sort on.</param>
        /// <param name="order">The sort order.</param>
        public QuerySortKey(string fieldName, SortOrder order)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(nameof(fieldName));

            FieldName = fieldName;
            Order = order;
        }

        public static implicit operator string(QuerySortKey qsk) => qsk.ToString();

        /// <summary>
        /// Constructs a well-formed sort string, without the sort command (c:sort=).
        /// </summary>
        /// <returns>A well-formed sort string.</returns>
        public override string ToString() => $"{FieldName}:{(int)Order}";
    }
}
