using faketasks.Core.Helpers;
using faketasks.Core.IO;

namespace faketasks.Core.Modules;

/// <summary>
///     Extension methods for IModuleContext to simplify common operations.
///     Provides convenient access to styling, formatting, and progress display helpers.
/// </summary>
public static class ModuleContextExtensions {
    /// <summary>
    ///     Writes text with status coloring.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="text">Text to write.</param>
    /// <param name="status">Status level for coloring.</param>
    public static void WriteStatus(this IModuleContext context, string text, Status status) {
        var (coloredText, color) = ColorHelper.Colorize(text, status);
        context.WriteStyled(coloredText, color);
    }

    /// <summary>
    ///     Writes a line with status coloring.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="text">Text to write.</param>
    /// <param name="status">Status level for coloring.</param>
    public static void WriteLineStatus(this IModuleContext context, string text, Status status) {
        var (coloredText, color) = ColorHelper.Colorize(text, status);
        context.WriteStyled(coloredText, color);
        context.WriteLine(string.Empty);
    }

    /// <summary>
    ///     Writes a success message (green).
    /// </summary>
    public static void WriteSuccess(this IModuleContext context, string text) {
        WriteStatus(context, text, Status.Success);
    }

    /// <summary>
    ///     Writes a success message line (green).
    /// </summary>
    public static void WriteLineSuccess(this IModuleContext context, string text) {
        WriteLineStatus(context, text, Status.Success);
    }

    /// <summary>
    ///     Writes a warning message (yellow).
    /// </summary>
    public static void WriteWarning(this IModuleContext context, string text) {
        WriteStatus(context, text, Status.Warning);
    }

    /// <summary>
    ///     Writes a warning message line (yellow).
    /// </summary>
    public static void WriteLineWarning(this IModuleContext context, string text) {
        WriteLineStatus(context, text, Status.Warning);
    }

    /// <summary>
    ///     Writes an error message (red).
    /// </summary>
    public static void WriteError(this IModuleContext context, string text) {
        WriteStatus(context, text, Status.Error);
    }

    /// <summary>
    ///     Writes an error message line (red).
    /// </summary>
    public static void WriteLineError(this IModuleContext context, string text) {
        WriteLineStatus(context, text, Status.Error);
    }

    /// <summary>
    ///     Writes an info message (cyan).
    /// </summary>
    public static void WriteInfo(this IModuleContext context, string text) {
        WriteStatus(context, text, Status.Info);
    }

    /// <summary>
    ///     Writes an info message line (cyan).
    /// </summary>
    public static void WriteLineInfo(this IModuleContext context, string text) {
        WriteLineStatus(context, text, Status.Info);
    }

    /// <summary>
    ///     Writes a debug message (gray).
    /// </summary>
    public static void WriteDebug(this IModuleContext context, string text) {
        WriteStatus(context, text, Status.Debug);
    }

    /// <summary>
    ///     Writes a debug message line (gray).
    /// </summary>
    public static void WriteLineDebug(this IModuleContext context, string text) {
        WriteLineStatus(context, text, Status.Debug);
    }

    /// <summary>
    ///     Renders a progress bar as text.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="percent">Progress percentage (0-100).</param>
    /// <param name="width">Width of the progress bar.</param>
    /// <param name="style">Progress bar style.</param>
    /// <returns>Formatted progress bar string.</returns>
    public static string RenderProgressBar(this IModuleContext context, int percent, int width = 40, ProgressBarStyle style = ProgressBarStyle.Default) {
        return ProgressWidgets.RenderBlockProgressBar(percent, width, style);
    }

    /// <summary>
    ///     Renders detailed progress with percentage and time.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="current">Current value.</param>
    /// <param name="total">Total value.</param>
    /// <param name="startTime">Start time for elapsed time calculation.</param>
    /// <param name="width">Width of the progress bar.</param>
    /// <param name="showPercentage">Whether to show percentage.</param>
    /// <param name="showTime">Whether to show elapsed time.</param>
    /// <returns>Formatted progress string.</returns>
    public static string RenderDetailedProgress(
        this IModuleContext context,
        int current,
        int total,
        DateTime? startTime = null,
        int width = 40,
        bool showPercentage = true,
        bool showTime = false
    ) {
        return ProgressWidgets.RenderDetailedProgress(current, total, startTime, width, showPercentage, showTime);
    }

    /// <summary>
    ///     Formats data size with appropriate units.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns>Formatted size string.</returns>
    public static string FormatDataSize(this IModuleContext context, long bytes) {
        return ProgressWidgets.FormatDataSize(bytes);
    }

    /// <summary>
    ///     Formats data transfer rate with appropriate units.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="bytesPerSecond">Bytes per second.</param>
    /// <returns>Formatted rate string.</returns>
    public static string FormatDataRate(this IModuleContext context, double bytesPerSecond) {
        return ProgressWidgets.FormatDataRate(bytesPerSecond);
    }

    /// <summary>
    ///     Gets a random item from a collection.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    /// <param name="context">Module context.</param>
    /// <param name="items">Collection of items to choose from.</param>
    /// <returns>A random item from the collection.</returns>
    public static T RandomItem<T>(this IModuleContext context, IReadOnlyList<T> items) {
        if (items.Count == 0) {
            throw new ArgumentException("Collection cannot be empty.", nameof(items));
        }
        return items[context.Random.Next(items.Count)];
    }

    /// <summary>
    ///     Gets a random item from an array.
    /// </summary>
    /// <typeparam name="T">Type of items in the array.</typeparam>
    /// <param name="context">Module context.</param>
    /// <param name="items">Array of items to choose from.</param>
    /// <returns>A random item from the array.</returns>
    public static T RandomItem<T>(this IModuleContext context, params T[] items) {
        if (items.Length == 0) {
            throw new ArgumentException("Array cannot be empty.", nameof(items));
        }
        return items[context.Random.Next(items.Length)];
    }

    /// <summary>
    ///     Generates a random string of specified length.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="length">Length of the string to generate.</param>
    /// <param name="characterSet">Optional character set to use.</param>
    /// <returns>Random string.</returns>
    public static string RandomString(this IModuleContext context, int length, string? characterSet = null) {
        const string defaultChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var chars = characterSet ?? defaultChars;

        var result = new char[length];
        for (int i = 0; i < length; i++) {
            result[i] = chars[context.Random.Next(chars.Length)];
        }
        return new string(result);
    }

    /// <summary>
    ///     Checks if the current terminal supports advanced features.
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="capability">Specific capability to check.</param>
    /// <returns>True if the capability is supported.</returns>
    public static bool Supports(this IModuleContext context, TerminalCapabilityDetector.Capabilities capability) {
        var capabilities = TerminalCapabilityDetector.GetCapabilities();
        return capabilities.HasFlag(capability);
    }

    /// <summary>
    ///     Gets a random percentage between min and max (inclusive).
    /// </summary>
    /// <param name="context">Module context.</param>
    /// <param name="min">Minimum percentage (default: 0).</param>
    /// <param name="max">Maximum percentage (default: 100).</param>
    /// <returns>Random percentage.</returns>
    public static int RandomPercentage(this IModuleContext context, int min = 0, int max = 100) {
        return context.Random.Next(min, max + 1);
    }
}