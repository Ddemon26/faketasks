namespace faketasks.Core.Modules;

/// <summary>
/// Runtime context provided to modules during execution.
/// Encapsulates output, delay control, randomization, and terminal properties.
/// </summary>
public interface IModuleContext
{
    /// <summary>
    /// Writes text to output without a newline.
    /// </summary>
    void Write(string text);

    /// <summary>
    /// Writes a line of text to output.
    /// </summary>
    void WriteLine(string text);

    /// <summary>
    /// Writes styled text with optional ANSI color formatting.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="color">Optional console color. If null, uses default color.</param>
    void WriteStyled(string text, ConsoleColor? color = null);

    /// <summary>
    /// Asynchronously delays execution for the specified base duration,
    /// scaled by the configured speed factor and instant-print settings.
    /// </summary>
    /// <param name="baseDuration">The base delay duration before scaling.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    Task DelayAsync(TimeSpan baseDuration, CancellationToken cancellationToken);

    /// <summary>
    /// Random number generator for module use.
    /// Shared instance to allow reproducible sequences if seeded.
    /// </summary>
    Random Random { get; }

    /// <summary>
    /// Current terminal width in characters.
    /// Useful for formatting output to fit the screen.
    /// </summary>
    int TerminalWidth { get; }
}
