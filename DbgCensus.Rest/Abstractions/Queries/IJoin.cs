namespace DbgCensus.Rest.Abstractions.Queries
{
    public interface IJoin
    {
        /// <summary>
        /// The ID field on the base collection to join on.
        /// </summary>
        /// <param name="fieldName">The name of the field to join on.</param>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>
        IJoin OnField(string fieldName);

        /// <summary>
        /// The ID field on the joined collection to join to.
        /// </summary>
        /// <param name="fieldName">The field to join to.</param>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>
        IJoin ToField(string fieldName);

        /// <summary>
        /// Sets a value indicating that the joined data is a list.
        /// </summary>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>
        IJoin IsList();

        /// <summary>
        /// Only includes the provided fields in the result. This method is incompatible with <see cref="HideFields(string[])"/>.
        /// </summary>
        /// <param name="fieldNames">The names of the fields that should be shown in the result.</param>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>
        IJoin ShowFields(params string[] fieldNames);

        /// <summary>
        /// Includes all but the provided fields in the result. This method is incompatible with <see cref="ShowFields(string[])"/>.
        /// </summary>
        /// <param name="fieldNames">The names of the fields that should be hidden from the result.</param>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>/
        IJoin HideFields(params string[] fieldNames);

        /// <summary>
        /// Creates a new field on the base collection, where the joined data should be injected to.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>
        IJoin InjectAt(string name);

        /// <summary>
        /// Performs a filter on the joined collection. Multiple filters can be performed.
        /// </summary>
        /// <param name="field">The collection field to filter on.</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <param name="modifier">The comparison operator.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        IJoin Where<T>(string field, T filterValue, SearchModifier modifier) where T : notnull;

        /// <summary>
        /// By default, all queries are treated as an 'outer' join. As in SQL, this means that results will be included for joins that do not match the criteria defined via terms. You can set a query to use 'inner' join behaviour, which allows filtering of a parent join via a term defined for its child if both are using 'inner' join behaviour.
        /// </summary>
        /// <returns>An <see cref="IJoin"/> instance so that calls may be chained.</returns>
        IJoin IsInnerJoin();

        /// <summary>
        /// Creates a nested join on this join.
        /// </summary>
        /// <param name="collection">The collection to join to.</param>
        /// <returns>The nested join object.</returns>
        IJoin WithNestedJoin(string collection);
    }
}
