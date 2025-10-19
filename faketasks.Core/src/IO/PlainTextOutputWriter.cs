namespace faketasks.Core.IO;

/// <summary>
///     Plain text output writer without ANSI color codes.
///     Used when output is redirected to a file or pipe (non-TTY).
/// </summary>
public sealed class PlainTextOutputWriter : IOutputWriter {
    const int DefaultTerminalWidth = 80;

    public void Write(string text) {
        Console.Write( text );
    }

    public void WriteLine(string text) {
        Console.WriteLine( text );
    }

    public void WriteStyled(string text, ConsoleColor? color = null) {
        // Ignore color styling in plain text mode
        Console.Write( text );
    }

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
}
