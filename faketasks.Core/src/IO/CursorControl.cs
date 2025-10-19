namespace faketasks.Core.IO;

/// <summary>
///     Utilities for terminal cursor control and ANSI escape sequences.
///     Provides safe cursor positioning and screen manipulation operations.
/// </summary>
public static class CursorControl {
    /// <summary>
    ///     ANSI escape sequence for moving cursor to specified position.
    /// </summary>
    public const string MoveCursor = "\x1b[";

    /// <summary>
    ///     ANSI escape sequence for clearing operations.
    /// </summary>
    public const string Clear = "\x1b[";

    /// <summary>
    ///     ANSI escape sequence for cursor visibility control.
    /// </summary>
    public const string CursorVisibility = "\x1b[?25";

    /// <summary>
    ///     Moves cursor to specified row and column (1-based indexing).
    /// </summary>
    /// <param name="row">Row number (1-based). If 0, stays in current row.</param>
    /// <param name="col">Column number (1-based). If 0, stays in current column.</param>
    /// <returns>ANSI escape sequence for cursor positioning.</returns>
    public static string Position(int row, int col) {
        if (row == 0 && col == 0) return "";
        if (row == 0) return $"{MoveCursor}{col}G";
        if (col == 0) return $"{MoveCursor}{row}H";
        return $"{MoveCursor}{row};{col}H";
    }

    /// <summary>
    ///     Moves cursor up by specified number of lines.
    /// </summary>
    /// <param name="lines">Number of lines to move up.</param>
    /// <returns>ANSI escape sequence for moving cursor up.</returns>
    public static string MoveUp(int lines) {
        return lines > 0 ? $"{MoveCursor}{lines}A" : "";
    }

    /// <summary>
    ///     Moves cursor down by specified number of lines.
    /// </summary>
    /// <param name="lines">Number of lines to move down.</param>
    /// <returns>ANSI escape sequence for moving cursor down.</returns>
    public static string MoveDown(int lines) {
        return lines > 0 ? $"{MoveCursor}{lines}B" : "";
    }

    /// <summary>
    ///     Moves cursor right by specified number of columns.
    /// </summary>
    /// <param name="columns">Number of columns to move right.</param>
    /// <returns>ANSI escape sequence for moving cursor right.</returns>
    public static string MoveRight(int columns) {
        return columns > 0 ? $"{MoveCursor}{columns}C" : "";
    }

    /// <summary>
    ///     Moves cursor left by specified number of columns.
    /// </summary>
    /// <param name="columns">Number of columns to move left.</param>
    /// <returns>ANSI escape sequence for moving cursor left.</returns>
    public static string MoveLeft(int columns) {
        return columns > 0 ? $"{MoveCursor}{columns}D" : "";
    }

    /// <summary>
    ///     Clears from cursor to end of line.
    /// </summary>
    /// <returns>ANSI escape sequence for clearing to end of line.</returns>
    public static string ClearToEndOfLine() => $"{Clear}K";

    /// <summary>
    ///     Clears from beginning of line to cursor.
    /// </summary>
    /// <returns>ANSI escape sequence for clearing from beginning of line.</returns>
    public static string ClearFromBeginningOfLine() => $"{Clear}1K";

    /// <summary>
    ///     Clears entire current line.
    /// </summary>
    /// <returns>ANSI escape sequence for clearing entire line.</returns>
    public static string ClearLine() => $"{Clear}2K";

    /// <summary>
    ///     Clears screen and moves cursor to top-left corner.
    /// </summary>
    /// <returns>ANSI escape sequence for clearing screen.</returns>
    public static string ClearScreen() => $"{Clear}2J{Position(1, 1)}";

    /// <summary>
    ///     Clears screen from cursor down.
    /// </summary>
    /// <returns>ANSI escape sequence for clearing screen down.</returns>
    public static string ClearScreenDown() => $"{Clear}J";

    /// <summary>
    ///     Clears screen from cursor up.
    /// </summary>
    /// <returns>ANSI escape sequence for clearing screen up.</returns>
    public static string ClearScreenUp() => $"{Clear}1J";

    /// <summary>
    ///     Hides cursor visibility.
    /// </summary>
    /// <returns>ANSI escape sequence for hiding cursor.</returns>
    public static string HideCursor() => $"{CursorVisibility}l";

    /// <summary>
    ///     Shows cursor visibility.
    /// </summary>
    /// <returns>ANSI escape sequence for showing cursor.</returns>
    public static string ShowCursor() => $"{CursorVisibility}h";

    /// <summary>
    ///     Saves cursor position.
    /// </summary>
    /// <returns>ANSI escape sequence for saving cursor position.</returns>
    public static string SaveCursor() => $"{MoveCursor}s";

    /// <summary>
    ///     Restores cursor position.
    /// </summary>
    /// <returns>ANSI escape sequence for restoring cursor position.</returns>
    public static string RestoreCursor() => $"{MoveCursor}u";

    /// <summary>
    ///     Gets terminal size query escape sequence.
    /// </summary>
    /// <returns>ANSI escape sequence for querying terminal size.</returns>
    public static string QueryTerminalSize() => $"{MoveCursor}9999;9999H{MoveCursor}6n";

    /// <summary>
    ///     Creates a safe escape sequence that only includes ANSI codes when supported.
    /// </summary>
    /// <param name="escapeSequence">The ANSI escape sequence.</param>
    /// <param name="supportsAnsi">Whether ANSI escape sequences are supported.</param>
    /// <returns>Escape sequence if supported, empty string otherwise.</returns>
    public static string SafeEscape(string escapeSequence, bool supportsAnsi) {
        return supportsAnsi ? escapeSequence : "";
    }

    /// <summary>
    ///     Pads text to fit within specified width, adding ellipsis if needed.
    /// </summary>
    /// <param name="text">Text to pad.</param>
    /// <param name="width">Target width.</param>
    /// <param name="truncateFromLeft">If true, truncates from left; otherwise from right.</param>
    /// <returns>Padded text.</returns>
    public static string PadText(string text, int width, bool truncateFromLeft = false) {
        if (string.IsNullOrEmpty(text)) return new string(' ', width);

        if (text.Length <= width) {
            return text.PadRight(width);
        }

        // Truncate with ellipsis
        string ellipsis = "...";
        if (truncateFromLeft) {
            return ellipsis + text.Substring(Math.Max(0, text.Length - width + ellipsis.Length));
        } else {
            return text.Substring(0, width - ellipsis.Length) + ellipsis;
        }
    }

    /// <summary>
    ///     Centers text within specified width.
    /// </summary>
    /// <param name="text">Text to center.</param>
    /// <param name="width">Target width.</param>
    /// <returns>Centered text.</returns>
    public static string CenterText(string text, int width) {
        if (string.IsNullOrEmpty(text)) return new string(' ', width);

        if (text.Length >= width) {
            return PadText(text, width);
        }

        int padding = (width - text.Length) / 2;
        return text.PadLeft(padding + text.Length).PadRight(width);
    }
}