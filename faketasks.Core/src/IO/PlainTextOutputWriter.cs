namespace faketasks.Core.IO;

/// <summary>
///     Plain text output writer without ANSI color codes or cursor control.
///     Used when output is redirected to a file or pipe (non-TTY).
///     All cursor operations are no-ops for compatibility.
/// </summary>
public sealed class PlainTextOutputWriter : IOutputWriter {
    const int DefaultTerminalWidth = 80;
    const int DefaultTerminalHeight = 25;

    // Properties
    public int TerminalWidth {
        get {
            try {
                // Even when redirected, try to get width if available
                return Console.WindowWidth;
            }
            catch {
                return DefaultTerminalWidth;
            }
        }
    }

    public int TerminalHeight {
        get {
            try {
                return Console.WindowHeight;
            }
            catch {
                return DefaultTerminalHeight;
            }
        }
    }

    public bool SupportsCursorControl => false;

    public bool SupportsAnsi => false;

    // Synchronous methods
    public void Write(string text) {
        Console.Write(text);
    }

    public void WriteLine(string text) {
        Console.WriteLine(text);
    }

    public void WriteStyled(string text, ConsoleColor? color = null) {
        // Ignore color styling in plain text mode
        Console.Write(text);
    }

    // Asynchronous methods
    public Task WriteAsync(string text) {
        Console.Write(text);
        return Task.CompletedTask;
    }

    public Task WriteLineAsync(string text) {
        Console.WriteLine(text);
        return Task.CompletedTask;
    }

    public Task WriteStyledAsync(string text, ConsoleColor? color = null) {
        WriteStyled(text, color);
        return Task.CompletedTask;
    }

    // Cursor control methods (no-ops for plain text mode)
    public void MoveCursor(int row, int col) {
        // No-op in plain text mode
    }

    public void ClearLine() {
        // No-op in plain text mode
    }

    public void ClearToEndOfLine() {
        // No-op in plain text mode
    }

    public void ClearFromBeginningOfLine() {
        // No-op in plain text mode
    }

    public void ClearScreen() {
        // No-op in plain text mode
    }

    public void HideCursor() {
        // No-op in plain text mode
    }

    public void ShowCursor() {
        // No-op in plain text mode
    }

    public void SaveCursor() {
        // No-op in plain text mode
    }

    public void RestoreCursor() {
        // No-op in plain text mode
    }

    // Combined positioning and writing methods (fallbacks)
    public void WriteAt(string text, int row, int col) {
        // Fallback: just write the text
        Write(text);
    }

    public void OverwriteLine(string text) {
        // Fallback: write on new line
        WriteLine(text);
    }

    // Asynchronous cursor control methods (no-ops)
    public Task MoveCursorAsync(int row, int col) {
        // No-op in plain text mode
        return Task.CompletedTask;
    }

    public Task ClearLineAsync() {
        // No-op in plain text mode
        return Task.CompletedTask;
    }

    public Task ClearScreenAsync() {
        // No-op in plain text mode
        return Task.CompletedTask;
    }

    public Task WriteAtAsync(string text, int row, int col) {
        // Fallback: just write the text
        Write(text);
        return Task.CompletedTask;
    }

    public Task OverwriteLineAsync(string text) {
        // Fallback: write on new line
        WriteLine(text);
        return Task.CompletedTask;
    }
}
