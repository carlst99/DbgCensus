using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Stores data required to perform a resolve in the Census REST API.
    /// </summary>
    public struct QueryResolve
    {
        private readonly string[] _showFields;

        /// <summary>
        /// Gets the resolve to be made.
        /// </summary>
        public string ResolveTo { get; }

        /// <summary>
        /// Gets the fields to show from the resolved collection.
        /// </summary>
        public IReadOnlyList<string> ShowFields => _showFields;

        /// <summary>
        /// Stores data required to perform a resolve in the Census REST API.
        /// </summary>
        /// <param name="resolveTo">The resolve to make.</param>
        /// <param name="showFields">The fields to be shown from the resolved collection.</param>
        /// <exception cref="ArgumentNullException">Thrown if the 'resolveTo' parameter is null or empty.</exception>
        public QueryResolve(string resolveTo, params string[] showFields)
        {
            if (string.IsNullOrEmpty(resolveTo))
                throw new ArgumentNullException(nameof(resolveTo));

            ResolveTo = resolveTo;
            _showFields = showFields;
        }

        /// <summary>
        /// Constructs a well-formed resolve string, without the query operator (c:resolve=).
        /// </summary>
        /// <returns>A well-formed resolve string.</returns>
        public string GetResolveString() => $"{ResolveTo}({string.Join(',', _showFields)})";
    }
}
