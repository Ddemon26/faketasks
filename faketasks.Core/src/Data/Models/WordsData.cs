namespace faketasks.Core.Data.Models;

/// <summary>
///     Word lists for generating realistic names and descriptions.
/// </summary>
public sealed record WordsData {
    /// <summary>
    ///     Adjectives for name generation (e.g., "async", "fast", "secure").
    /// </summary>
    public IReadOnlyList<string> Adjectives { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Nouns for name generation (e.g., "parser", "runtime", "framework").
    /// </summary>
    public IReadOnlyList<string> Nouns { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Verbs for action descriptions (e.g., "compile", "build", "deploy").
    /// </summary>
    public IReadOnlyList<string> Verbs { get; init; } = Array.Empty<string>();
}