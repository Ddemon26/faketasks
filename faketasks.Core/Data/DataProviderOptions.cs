namespace faketasks.Core.Data;

/// <summary>
///     Configuration options for data providers.
///     Can be extended in the future to support custom data directories, caching, etc.
/// </summary>
public sealed record DataProviderOptions {
    /// <summary>
    ///     Optional custom directory path for loading data files.
    ///     If null, the provider uses its default source (e.g., embedded resources).
    /// </summary>
    public string? CustomDataDirectory { get; init; }

    /// <summary>
    ///     Whether to cache loaded data in memory.
    ///     Default is true for performance.
    /// </summary>
    public bool EnableCaching { get; init; } = true;

    /// <summary>
    ///     Creates default options with standard settings.
    /// </summary>
    public static DataProviderOptions Default => new();
}