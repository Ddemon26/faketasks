namespace faketasks.Core.Data;

/// <summary>
///     Provides access to data resources (word lists, file names, etc.) used by modules.
///     Implementations can load from embedded resources, files, or other sources.
/// </summary>
public interface IDataProvider {
    /// <summary>
    ///     Asynchronously loads lines from a named data resource.
    /// </summary>
    /// <param name="resourceName">
    ///     Identifier for the resource (e.g., "package_names", "log_messages").
    ///     Resource naming convention is implementation-specific.
    /// </param>
    /// <returns>Read-only list of lines from the resource.</returns>
    /// <exception cref="ArgumentException">If the resource name is invalid.</exception>
    /// <exception cref="InvalidOperationException">If the resource cannot be loaded.</exception>
    Task<IReadOnlyList<string>> GetLinesAsync(string resourceName);
}