namespace faketasks.Core.IO;

/// <summary>
///     Abstraction for writing output to the terminal or other destinations.
///     Allows testing and alternative output targets.
/// </summary>
public interface IOutputWriter {
    /// <summary>
    ///     Gets the current terminal width in characters.
    /// </summary>
    int TerminalWidth { get; }

    /// <summary>
    ///     Gets the current terminal height in characters.
    /// </summary>
    int TerminalHeight { get; }

    /// <summary>
    ///     Gets whether the output writer supports cursor control.
    /// </summary>
    bool SupportsCursorControl { get; }

    /// <summary>
    ///     Gets whether the output writer supports ANSI escape sequences.
    /// </summary>
    bool SupportsAnsi { get; }

    // Synchronous methods
    /// <summary>
    ///     Writes text without a newline.
    /// </summary>
    void Write(string text);

    /// <summary>
    ///     Writes a line of text.
    /// </summary>
    void WriteLine(string text);

    /// <summary>
    ///     Writes text with optional color styling.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="color">Optional console color. Implementation may ignore if unsupported.</param>
    void WriteStyled(string text, ConsoleColor? color = null);

    // Asynchronous methods
    /// <summary>
    ///     Asynchronously writes text without a newline.
    /// </summary>
    Task WriteAsync(string text);

    /// <summary>
    ///     Asynchronously writes a line of text.
    /// </summary>
    Task WriteLineAsync(string text);

    /// <summary>
    ///     Asynchronously writes text with optional color styling.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="color">Optional console color. Implementation may ignore if unsupported.</param>
    Task WriteStyledAsync(string text, ConsoleColor? color = null);

    // Cursor control methods
    /// <summary>
    ///     Moves cursor to specified position (1-based indexing).
    /// </summary>
    /// <param name="row">Row number (1-based). If 0, stays in current row.</param>
    /// <param name="col">Column number (1-based). If 0, stays in current column.</param>
    void MoveCursor(int row, int col);

    /// <summary>
    ///     Clears the current line and moves cursor to beginning.
    /// </summary>
    void ClearLine();

    /// <summary>
    ///     Clears from cursor position to end of line.
    /// </summary>
    void ClearToEndOfLine();

    /// <summary>
    ///     Clears from beginning of line to cursor position.
    /// </summary>
    void ClearFromBeginningOfLine();

    /// <summary>
    ///     Clears the entire screen and moves cursor to top-left.
    /// </summary>
    void ClearScreen();

    /// <summary>
    ///     Hides the cursor visibility.
    /// </summary>
    void HideCursor();

    /// <summary>
    ///     Shows the cursor visibility.
    /// </summary>
    void ShowCursor();

    /// <summary>
    ///     Saves current cursor position.
    /// </summary>
    void SaveCursor();

    /// <summary>
    ///     Restores saved cursor position.
    /// </summary>
    void RestoreCursor();

    // Combined positioning and writing methods
    /// <summary>
    ///     Writes text at specified cursor position.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="row">Row number (1-based).</param>
    /// <param name="col">Column number (1-based).</param>
    void WriteAt(string text, int row, int col);

    /// <summary>
    ///     Overwrites the current line with new text.
    /// </summary>
    /// <param name="text">Text to write.</param>
    void OverwriteLine(string text);

    // Asynchronous cursor control methods
    /// <summary>
    ///     Asynchronously moves cursor to specified position.
    /// </summary>
    Task MoveCursorAsync(int row, int col);

    /// <summary>
    ///     Asynchronously clears the current line.
    /// </summary>
    Task ClearLineAsync();

    /// <summary>
    ///     Asynchronously clears the entire screen.
    /// </summary>
    Task ClearScreenAsync();

    /// <summary>
    ///     Asynchronously writes text at specified position.
    /// </summary>
    /// <param name="text">Text to write.</param>
    /// <param name="row">Row number (1-based).</param>
    /// <param name="col">Column number (1-based).</param>
    Task WriteAtAsync(string text, int row, int col);

    /// <summary>
    ///     Asynchronously overwrites the current line with new text.
    /// </summary>
    /// <param name="text">Text to write.</param>
    Task OverwriteLineAsync(string text);
}