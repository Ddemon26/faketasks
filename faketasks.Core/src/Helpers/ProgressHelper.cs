namespace faketasks.Core.Helpers;

/// <summary>
///     Utilities for rendering progress indicators, percentages, and data sizes.
/// </summary>
public static class ProgressHelper {
    static readonly string[] SpinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    static readonly string[] BarSpinnerFrames = { "▱▱▱", "▰▱▱", "▰▰▱", "▰▰▰", "▰▰▱", "▰▱▱" };

    /// <summary>
    ///     Renders a progress bar with the given percentage and width.
    /// </summary>
    /// <param name="percent">Progress percentage (0-100).</param>
    /// <param name="width">Total width of the progress bar in characters.</param>
    /// <returns>Formatted progress bar string (e.g., "[=====>    ]").</returns>
    public static string RenderProgressBar(int percent, int width = 20) {
        percent = Math.Clamp( percent, 0, 100 );
        int fillWidth = width - 2; // Account for brackets
        var filled = (int)Math.Round( fillWidth * (percent / 100.0) );
        int empty = fillWidth - filled;

        string arrow = filled > 0 && filled < fillWidth ? ">" : "";
        string filledPart = filled > 0 ? new string( '=', Math.Max( 0, filled - arrow.Length ) ) : "";
        var emptyPart = new string( ' ', empty );

        return $"[{filledPart}{arrow}{emptyPart}]";
    }

    /// <summary>
    ///     Renders a block-style progress bar using Unicode block characters.
    /// </summary>
    public static string RenderBlockProgressBar(int percent, int width = 20) {
        percent = Math.Clamp( percent, 0, 100 );
        var filled = (int)Math.Round( width * (percent / 100.0) );
        int empty = width - filled;

        return new string( '█', filled ) + new string( '░', empty );
    }

    /// <summary>
    ///     Formats a percentage from current and total values.
    /// </summary>
    public static string FormatPercentage(int current, int total) {
        if ( total == 0 ) return "0%";
        var percent = (int)Math.Round( 100.0 * current / total );
        return $"{percent}%";
    }

    /// <summary>
    ///     Formats a percentage value.
    /// </summary>
    public static string FormatPercentage(int percent)
        => $"{Math.Clamp( percent, 0, 100 )}%";

    /// <summary>
    ///     Gets a spinner frame based on iteration count.
    ///     Cycles through animation frames: ⠋⠙⠹⠸⠼⠴⠦⠧⠇⠏
    /// </summary>
    public static string GetSpinnerFrame(int iteration)
        => SpinnerFrames[iteration % SpinnerFrames.Length];

    /// <summary>
    ///     Gets a bar-style spinner frame based on iteration count.
    /// </summary>
    public static string GetBarSpinnerFrame(int iteration)
        => BarSpinnerFrames[iteration % BarSpinnerFrames.Length];

    /// <summary>
    ///     Formats a byte count as a human-readable size (B, KB, MB, GB, TB).
    /// </summary>
    public static string FormatBytes(long bytes) {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double value = bytes;
        var order = 0;

        while (value >= 1024 && order < sizes.Length - 1) {
            order++;
            value /= 1024;
        }

        return $"{value:0.##} {sizes[order]}";
    }

    /// <summary>
    ///     Formats a transfer rate in bytes per second.
    /// </summary>
    public static string FormatBytesPerSecond(long bytesPerSecond)
        => $"{FormatBytes( bytesPerSecond )}/s";

    /// <summary>
    ///     Renders a progress line with percentage, bar, and optional message.
    ///     Example: "Downloading: [=====>    ] 45% - package.tar.gz"
    /// </summary>
    public static string RenderProgressLine(
        string label,
        int percent,
        string? message = null,
        int barWidth = 20
    ) {
        string bar = RenderProgressBar( percent, barWidth );
        string percentText = FormatPercentage( percent );
        List<string> parts = new() { label, bar, percentText };

        if ( !string.IsNullOrEmpty( message ) ) {
            parts.Add( message );
        }

        return string.Join( " ", parts );
    }

    /// <summary>
    ///     Calculates estimated time remaining based on progress.
    /// </summary>
    public static string FormatTimeRemaining(TimeSpan elapsed, int percentComplete) {
        if ( percentComplete <= 0 ) return "--:--";

        double totalTime = elapsed.TotalSeconds * (100.0 / percentComplete);
        var remaining = TimeSpan.FromSeconds( totalTime - elapsed.TotalSeconds );

        if ( remaining.TotalHours >= 1 ) {
            return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
        }

        if ( remaining.TotalMinutes >= 1 ) {
            return $"{(int)remaining.TotalMinutes}m {remaining.Seconds}s";
        }

        return $"{(int)remaining.TotalSeconds}s";
    }

    /// <summary>
    ///     Formats a count with total (e.g., "3/10", "45/100").
    /// </summary>
    public static string FormatCount(int current, int total)
        => $"{current}/{total}";

    /// <summary>
    ///     Pads a number to match the width of the total for aligned output.
    /// </summary>
    public static string PadCount(int current, int total) {
        int width = total.ToString().Length;
        return current.ToString().PadLeft( width );
    }
}