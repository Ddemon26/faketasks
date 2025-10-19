namespace faketasks.Core.IO;

/// <summary>
/// Abstraction for writing output to the terminal or other destinations.
/// Allows testing and alternative output targets.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Writes text without a newline.
    /// </summary>
    void Write(string text);

    /// <summary>
    /// Writes a line of text.
    /// </summary>
    void WriteLine(string text);

    /// <summary>
    /// Writes text with optional color styling.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="color">Optional console color. Implementation may ignore if unsupported.</param>
    void WriteStyled(string text, ConsoleColor? color = null);

    /// <summary>
    /// Gets the current terminal width in characters.
    /// </summary>
    int TerminalWidth { get; }
}
