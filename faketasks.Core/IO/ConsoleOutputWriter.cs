namespace faketasks.Core.IO;

/// <summary>
/// Standard console output writer using System.Console.
/// Supports ANSI color codes and terminal width detection.
/// </summary>
public sealed class ConsoleOutputWriter : IOutputWriter
{
    private const int DefaultTerminalWidth = 80;

    public void Write(string text)
    {
        Console.Write(text);
    }

    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }

    public void WriteStyled(string text, ConsoleColor? color = null)
    {
        if (color.HasValue)
        {
            var previousColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color.Value;
                Console.Write(text);
            }
            finally
            {
                Console.ForegroundColor = previousColor;
            }
        }
        else
        {
            Console.Write(text);
        }
    }

    public int TerminalWidth
    {
        get
        {
            try
            {
                return Console.WindowWidth;
            }
            catch
            {
                // If we can't detect width (e.g., output redirected), use default
                return DefaultTerminalWidth;
            }
        }
    }
}
