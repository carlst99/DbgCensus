namespace DbgCensus.Rest.Abstractions.Queries
{
    /// <summary>
    /// Contains modifiers used to enhance searches in the Census REST API.
    /// </summary>
    public struct SearchModifier
    {
        /// <summary>
        /// Search for values that exactly equal to the filter.
        /// </summary>
        public static readonly SearchModifier EQUALS = new(string.Empty);

        /// <summary>
        /// Search for values that are less than the filter.
        /// </summary>
        public static readonly SearchModifier LESS_THAN = new("<");

        /// <summary>
        /// Search for values that are less than or equal to the filter.
        /// </summary>
        public static readonly SearchModifier LESS_THAN_OR_EQUAL = new("[");

        /// <summary>
        /// Search for values that are greater than the filter.
        /// </summary>
        public static readonly SearchModifier GREATER_THAN = new(">");

        /// <summary>
        /// Search for values that are greater than or equal to the filter.
        /// </summary>
        public static readonly SearchModifier GREATER_THAN_OR_EQUAL = new("]");

        /// <summary>
        /// Search for values that start with the filter string.
        /// </summary>
        public static readonly SearchModifier STARTS_WITH = new("^");

        /// <summary>
        /// Search for values that contain the filter string.
        /// </summary>
        public static readonly SearchModifier CONTAINS = new("*");

        /// <summary>
        /// Search for values that are not equal to the filter string.
        /// </summary>
        public static readonly SearchModifier NOT_EQUALS = new("!");

        /// <summary>
        /// Gets the value required to use the modifier in the Census REST API.
        /// </summary>
        public string Value { get; }

        private SearchModifier(string modifier)
        {
            Value = modifier;
        }

        public static implicit operator string(SearchModifier m) => m.ToString();

        public override string ToString() => Value;
    }
}
