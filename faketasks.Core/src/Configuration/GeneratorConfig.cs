namespace faketasks.Core.Configuration;

/// <summary>
///     Configuration options for the fake task generator.
///     Mirrors genact's CLI flags for runtime behavior control.
/// </summary>
public sealed class GeneratorConfig {
    /// <summary>
    ///     List of module names to enable. If null or empty, all modules are enabled.
    /// </summary>
    public IReadOnlyList<string>? EnabledModules { get; init; }

    /// <summary>
    ///     Speed multiplier for delays between output lines.
    ///     Values &gt; 1.0 speed up, values &lt; 1.0 slow down. Default is 1.0.
    /// </summary>
    public double SpeedFactor { get; init; } = 1.0;

    /// <summary>
    ///     Number of lines to print instantly without any delay.
    ///     Useful for quickly filling the screen. Default is 0.
    /// </summary>
    public int InstantPrintLines { get; init; } = 0;

    /// <summary>
    ///     Optional time limit after which the generator should exit.
    ///     If null, runs indefinitely until interrupted.
    /// </summary>
    public TimeSpan? ExitAfterTime { get; init; }

    /// <summary>
    ///     Optional module count limit. Generator exits after running this many modules.
    ///     If null, runs indefinitely until interrupted.
    /// </summary>
    public int? ExitAfterModules { get; init; }

    /// <summary>
    ///     Creates a default configuration with standard settings.
    /// </summary>
    public static GeneratorConfig Default => new();
}