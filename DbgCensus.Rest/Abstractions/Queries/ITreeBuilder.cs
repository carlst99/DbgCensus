namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// Defines functions to build a tree command for the Census REST API.
/// </summary>
public interface ITreeBuilder
{
    /// <summary>
    /// Sets the field to group data by. Will be removed from the data source.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>An <see cref="ITreeBuilder"/> instance so that calls may be chained.</returns>
    ITreeBuilder OnField(string fieldName);

    /// <summary>
    /// Indicates that the field being grouped on is a list,
    /// allowing multiple results to be included in each group.
    /// </summary>
    /// <returns>An <see cref="ITreeBuilder"/> instance so that calls may be chained.</returns>
    ITreeBuilder IsList();

    /// <summary>
    /// A value to prepend to each group name in the tree.
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns>An <see cref="ITreeBuilder"/> instance so that calls may be chained.</returns>
    ITreeBuilder WithPrefix(string prefix);

    /// <summary>
    /// Sets the field within the result (including joins and resolves) at which
    /// to start the tree, rather than performing the operation on the original query.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>An <see cref="ITreeBuilder"/> instance so that calls may be chained.</returns>
    ITreeBuilder StartOn(string fieldName);
}
