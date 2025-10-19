namespace faketasks.Core.IO;

/// <summary>
///     Standard console output writer using System.Console.
///     Supports ANSI color codes and terminal width detection.
/// </summary>
public sealed class ConsoleOutputWriter : IOutputWriter {
    const int DefaultTerminalWidth = 80;

    /// <summary>
    ///     Creates an appropriate output writer based on TTY detection.
    ///     Returns PlainTextOutputWriter if output is redirected, otherwise ConsoleOutputWriter.
    /// </summary>
    public static IOutputWriter Create() {
        // Check if output is redirected (e.g., piped to file or another command)
        bool isRedirected = Console.IsOutputRedirected || Console.IsErrorRedirected;
        return isRedirected ? new PlainTextOutputWriter() : new ConsoleOutputWriter();
    }

    public void Write(string text) {
        Console.Write( text );
    }

    public void WriteLine(string text) {
        Console.WriteLine( text );
    }

    public void WriteStyled(string text, ConsoleColor? color = null) {
        if ( color.HasValue ) {
            var previousColor = Console.ForegroundColor;
            try {
                Console.ForegroundColor = color.Value;
                Console.Write( text );
            }
            finally {
                Console.ForegroundColor = previousColor;
            }
        }
        else {
            Console.Write( text );
        }
    }

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
}