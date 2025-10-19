namespace faketasks.Core.IO;

/// <summary>
///     Standard console output writer using System.Console.
///     Supports ANSI color codes, terminal width detection, and cursor control.
/// </summary>
public sealed class ConsoleOutputWriter : IOutputWriter {
    const int DefaultTerminalWidth = 80;
    const int DefaultTerminalHeight = 25;
    private readonly TerminalCapabilityDetector.Capabilities _capabilities;
    private readonly bool _supportsAnsi;
    private readonly bool _supportsCursorControl;

    /// <summary>
    ///     Creates an appropriate output writer based on TTY detection.
    ///     Returns PlainTextOutputWriter if output is redirected, otherwise ConsoleOutputWriter.
    /// </summary>
    public static IOutputWriter Create() {
        // Check if output is redirected (e.g., piped to file or another command)
        bool isRedirected = Console.IsOutputRedirected || Console.IsErrorRedirected;
        return isRedirected ? new PlainTextOutputWriter() : new ConsoleOutputWriter();
    }

    public ConsoleOutputWriter() {
        _capabilities = TerminalCapabilityDetector.GetCapabilities();
        _supportsAnsi = _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.AnsiColors) ||
                       _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl);
        _supportsCursorControl = _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl) &&
                                _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorPositioning);
    }

    // Properties
    public int TerminalWidth {
        get {
            try {
                return Console.WindowWidth;
            }
            catch {
                // If we can't detect width (e.g., output redirected), use default
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
                // If we can't detect height, use default
                return DefaultTerminalHeight;
            }
        }
    }

    public bool SupportsCursorControl => _supportsCursorControl;

    public bool SupportsAnsi => _supportsAnsi;

    // Synchronous methods
    public void Write(string text) {
        Console.Write(text);
    }

    public void WriteLine(string text) {
        Console.WriteLine(text);
    }

    public void WriteStyled(string text, ConsoleColor? color = null) {
        if (color.HasValue) {
            var previousColor = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color.Value;
                Console.Write(text);
            }
            finally {
                Console.ForegroundColor = previousColor;
            }
        }
        else {
            Console.Write(text);
        }
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

    // Cursor control methods
    public void MoveCursor(int row, int col) {
        if (_supportsCursorControl) {
            var escapeSequence = CursorControl.SafeEscape(CursorControl.Position(row, col), true);
            Console.Write(escapeSequence);
        }
    }

    public void ClearLine() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.ClearLine());
        }
    }

    public void ClearToEndOfLine() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.ClearToEndOfLine());
        }
    }

    public void ClearFromBeginningOfLine() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.ClearFromBeginningOfLine());
        }
    }

    public void ClearScreen() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.ClearScreen());
        }
    }

    public void HideCursor() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.HideCursor());
        }
    }

    public void ShowCursor() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.ShowCursor());
        }
    }

    public void SaveCursor() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.SaveCursor());
        }
    }

    public void RestoreCursor() {
        if (_supportsCursorControl) {
            Console.Write(CursorControl.RestoreCursor());
        }
    }

    // Combined positioning and writing methods
    public void WriteAt(string text, int row, int col) {
        if (_supportsCursorControl) {
            MoveCursor(row, col);
            Write(text);
        }
        else {
            // Fallback: just write the text
            Write(text);
        }
    }

    public void OverwriteLine(string text) {
        if (_supportsCursorControl) {
            ClearLine();
            Write(text);
        }
        else {
            // Fallback: write on new line
            WriteLine(text);
        }
    }

    // Asynchronous cursor control methods
    public Task MoveCursorAsync(int row, int col) {
        MoveCursor(row, col);
        return Task.CompletedTask;
    }

    public Task ClearLineAsync() {
        ClearLine();
        return Task.CompletedTask;
    }

    public Task ClearScreenAsync() {
        ClearScreen();
        return Task.CompletedTask;
    }

    public Task WriteAtAsync(string text, int row, int col) {
        WriteAt(text, row, col);
        return Task.CompletedTask;
    }

    public Task OverwriteLineAsync(string text) {
        OverwriteLine(text);
        return Task.CompletedTask;
    }
}