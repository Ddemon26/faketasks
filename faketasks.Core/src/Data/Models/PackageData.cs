namespace faketasks.Core.Data.Models;

/// <summary>
/// Package and dependency data for cargo, npm, and other package managers.
/// </summary>
public sealed record PackageData
{
    /// <summary>
    /// Rust crate names (e.g., "tokio", "serde", "actix-web").
    /// </summary>
    public IReadOnlyList<string> CrateNames { get; init; } = Array.Empty<string>();

    /// <summary>
    /// NPM package names (e.g., "react", "lodash", "express").
    /// </summary>
    public IReadOnlyList<string> NpmPackages { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Cargo features (e.g., "async", "std", "derive").
    /// </summary>
    public IReadOnlyList<string> Features { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Build targets (e.g., "x86_64-unknown-linux-gnu", "wasm32-unknown-unknown").
    /// </summary>
    public IReadOnlyList<string> Targets { get; init; } = Array.Empty<string>();
}
