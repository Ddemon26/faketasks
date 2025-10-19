using faketasks.Core.IO;

namespace faketasks.Core.IO;

/// <summary>
///     Utilities for creating rich progress displays and widgets.
///     Provides multi-line status displays, real-time updates, and animated indicators.
/// </summary>
public static class ProgressWidgets {
    static readonly string[] SpinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
    static readonly string[] BarSpinnerFrames = { "▱▱▱", "▰▱▱", "▰▰▱", "▰▰▰", "▰▰▱", "▰▱▱" };
    static readonly string[] ProgressChars = { "█", "▓", "▒", "▐", "░" };

    /// <summary>
    ///     Creates a real-time updating progress display that overwrites itself.
    /// </summary>
    /// <param name="writer">Output writer to use.</param>
    /// <param name="message">Progress message.</param>
    /// <param name="current">Current progress value.</param>
    /// <param name="total">Total progress value.</param>
    /// <param name="width">Width of the progress bar.</param>
    /// <param name="showPercentage">Whether to show percentage.</param>
    /// <param name="showTime">Whether to show elapsed time.</param>
    /// <returns>Function to update the progress display.</returns>
    public static async Task<IProgressUpdater> CreateRealTimeProgress(
        IOutputWriter writer,
        string message,
        int current,
        int total,
        int width = 40,
        bool showPercentage = true,
        bool showTime = false
    ) {
        var startTime = DateTime.UtcNow;
        var lastUpdate = new DateTime(1, 1, 1);
        var capabilities = TerminalCapabilityDetector.GetCapabilities();
        var supportsAnsi = capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl);

        if (!supportsAnsi) {
            // Fallback to simple line-by-line progress
            return CreateSimpleProgress(writer, message, current, total, width, showPercentage);
        }

        return new RealTimeProgressUpdater(writer, message, current, total, width, showPercentage, showTime, startTime, capabilities);
    }

    /// <summary>
    ///     Creates a multi-line status display that updates in place.
    /// </summary>
    /// <param name="writer">Output writer to use.</param>
    /// <param name="lines">Initial status lines.</param>
    /// <returns>Function to update the status display.</returns>
    public static async Task<IStatusDisplay> CreateStatusDisplay(
        IOutputWriter writer,
        params string[] lines
    ) {
        var capabilities = TerminalCapabilityDetector.GetCapabilities();
        var supportsAnsi = capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl);

        if (!supportsAnsi) {
            // Fallback to simple line-by-line status
            return new SimpleStatusDisplay(writer, lines.ToList());
        }

        return new AnsiStatusDisplay(writer, lines.ToList(), capabilities);
    }

    /// <summary>
    ///     Creates a multi-line progress display for concurrent operations.
    /// </summary>
    /// <param name="writer">Output writer to use.</param>
    /// <param name="operations">List of operations to track.</param>
    /// <returns>Function to update the multi-progress display.</returns>
    public static async Task<IMultiProgressDisplay> CreateMultiProgressDisplay(
        IOutputWriter writer,
        params string[] operations
    ) {
        var capabilities = TerminalCapabilityDetector.GetCapabilities();
        var supportsAnsi = capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl);

        if (!supportsAnsi) {
            // Fallback to simple sequential progress
            return new SimpleMultiProgressDisplay(writer, operations.ToList());
        }

        return new AnsiMultiProgressDisplay(writer, operations.ToList(), capabilities);
    }

    /// <summary>
    ///     Renders a spinning indicator with optional text.
    /// </summary>
    /// <param name="text">Optional text to display with the spinner.</param>
    /// <param name="frame">Current frame number.</param>
    /// <param name="supportsAnsi">Whether ANSI escape codes are supported.</param>
    /// <returns>Formatted spinner text.</returns>
    public static string RenderSpinner(string? text = null, int frame = 0, bool supportsAnsi = true) {
        if (!supportsAnsi) {
            return text != null ? $"{text}..." : "...";
        }

        var spinner = SpinnerFrames[frame % SpinnerFrames.Length];
        return text != null ? $"{text} {spinner}" : spinner;
    }

    /// <summary>
    ///     Renders a progress bar with block characters.
    /// </summary>
    /// <param name="percent">Progress percentage (0-100).</param>
    /// <param name="width">Total width of the progress bar.</param>
    /// <param name="style">Progress bar style.</param>
    /// <returns>Formatted progress bar.</returns>
    public static string RenderBlockProgressBar(int percent, int width, ProgressBarStyle style = ProgressBarStyle.Default) {
        percent = Math.Clamp(percent, 0, 100);
        width = Math.Max(width, 3); // Minimum width

        var filledBlocks = (int)Math.Round(width * (percent / 100.0));
        var emptyBlocks = width - filledBlocks;

        char filledChar, emptyChar;

        switch (style) {
            case ProgressBarStyle.Blocks:
                filledChar = '█';
                emptyChar = '░';
                break;
            case ProgressBarStyle.Classic:
                filledChar = '=';
                emptyChar = ' ';
                break;
            case ProgressBarStyle.ClassicWithArrow:
                filledChar = '=';
                emptyChar = ' ';
                var hasArrow = filledBlocks > 0 && filledBlocks < width;
                if (hasArrow) filledBlocks--;
                emptyBlocks = width - filledBlocks;
                break;
            default:
                filledChar = '=';
                emptyChar = ' ';
                break;
        }

        var bar = new string(filledChar, filledBlocks) + new string(emptyChar, emptyBlocks);
        return $"[{bar}]";
    }

    /// <summary>
    ///     Renders a progress bar with percentage and optional time.
    /// </summary>
    /// <param name="current">Current value.</param>
    /// <param name="total">Total value.</param>
    /// <param name="startTime">Start time for elapsed time calculation.</param>
    /// <param name="width">Width of the progress bar.</param>
    /// <param name="showPercentage">Whether to show percentage.</param>
    /// <param name="showTime">Whether to show elapsed time.</param>
    /// <param name="showETA">Whether to show estimated time remaining.</param>
    /// <returns>Formatted progress line.</returns>
    public static string RenderDetailedProgress(
        int current,
        int total,
        DateTime? startTime,
        int width = 40,
        bool showPercentage = true,
        bool showTime = false,
        bool showETA = false
    ) {
        var percent = total > 0 ? (int)Math.Round((double)current / total * 100) : 0;
        var bar = RenderBlockProgressBar(percent, width);
        var parts = new List<string> { bar };

        if (showPercentage) {
            parts.Add($"{percent,3}%");
        }

        if (showTime && startTime.HasValue) {
            var elapsed = DateTime.UtcNow - startTime.Value;
            parts.Add($"({elapsed:mm\\:ss})");
        }

        if (showETA && startTime.HasValue && current > 0 && current < total) {
            var elapsed = DateTime.UtcNow - startTime.Value;
            var estimatedTotal = TimeSpan.FromTicks((long)(elapsed.Ticks * total / (double)current));
            var remaining = estimatedTotal - elapsed;
            parts.Add($"(ETA: {remaining:mm\\:ss})");
        }

        return string.Join(" ", parts);
    }

    /// <summary>
    ///     Formats data size with appropriate units.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns>Formatted size string.</returns>
    public static string FormatDataSize(long bytes) {
        string[] units = { "B", "KB", "MB", "GB", "TB", "PB" };
        double size = bytes;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1) {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F1} {units[unitIndex]}";
    }

    /// <summary>
    ///     Formats data transfer rate with appropriate units.
    /// </summary>
    /// <param name="bytesPerSecond">Bytes per second.</param>
    /// <returns>Formatted rate string.</returns>
    public static string FormatDataRate(double bytesPerSecond) {
        string[] units = { "B/s", "KB/s", "MB/s", "GB/s", "TB/s", "PB/s" };
        double rate = bytesPerSecond;
        int unitIndex = 0;

        while (rate >= 1024 && unitIndex < units.Length - 1) {
            rate /= 1024;
            unitIndex++;
        }

        return $"{rate:F1} {units[unitIndex]}";
    }

    private static IProgressUpdater CreateSimpleProgress(
        IOutputWriter writer,
        string message,
        int current,
        int total,
        int width,
        bool showPercentage
    ) {
        return new SimpleProgressUpdater(writer, message, current, total, width, showPercentage);
    }
}

/// <summary>
///     Progress bar styles.
/// </summary>
public enum ProgressBarStyle {
    Default,
    Blocks,
    Classic,
    ClassicWithArrow
}

/// <summary>
///     Interface for updating progress displays.
/// </summary>
public interface IProgressUpdater : IAsyncDisposable {
    /// <summary>
    ///     Updates the progress value.
    /// </summary>
    /// <param name="current">New current value.</param>
    /// <param name="message">Optional new message.</param>
    Task UpdateAsync(int current, string? message = null);

    /// <summary>
    ///     Marks the progress as complete.
    /// </summary>
    Task CompleteAsync();
}

/// <summary>
///     Interface for status displays.
/// </summary>
public interface IStatusDisplay : IAsyncDisposable {
    /// <summary>
    ///     Updates a specific line.
    /// </summary>
    /// <param name="lineIndex">Zero-based line index.</param>
    /// <param name="text">New line content.</param>
    Task UpdateLineAsync(int lineIndex, string text);

    /// <summary>
    ///     Updates multiple lines.
    /// </summary>
    /// <param name="updates">Dictionary of line index to text.</param>
    Task UpdateLinesAsync(IDictionary<int, string> updates);

    /// <summary>
    ///     Adds a new line to the display.
    /// </summary>
    /// <param name="text">Line content.</param>
    Task AddLineAsync(string text);

    /// <summary>
    ///     Removes a line from the display.
    /// </summary>
    /// <param name="lineIndex">Line index to remove.</param>
    Task RemoveLineAsync(int lineIndex);
}

/// <summary>
///     Interface for multi-progress displays.
/// </summary>
public interface IMultiProgressDisplay : IAsyncDisposable {
    /// <summary>
    ///     Updates progress for a specific operation.
    /// </summary>
    /// <param name="operationIndex">Operation index.</param>
    /// <param name="current">Current progress value.</param>
    /// <param name="total">Total value.</param>
    /// <param name="status">Optional status message.</param>
    Task UpdateProgressAsync(int operationIndex, int current, int total, string? status = null);

    /// <summary>
    ///     Marks an operation as complete.
    /// </summary>
    /// <param name="operationIndex">Operation index.</param>
    Task CompleteOperationAsync(int operationIndex);

    /// <summary>
    ///     Marks all operations as complete.
    /// </summary>
    Task CompleteAllAsync();
}

// Internal implementation classes
internal sealed class RealTimeProgressUpdater : IProgressUpdater {
    private readonly IOutputWriter _writer;
    private readonly string _originalMessage;
    private readonly int _total;
    private readonly int _width;
    private readonly bool _showPercentage;
    private readonly bool _showTime;
    private readonly DateTime _startTime;
    private readonly TerminalCapabilityDetector.Capabilities _capabilities;
    private readonly int _startRow;
    private bool _disposed;

    public RealTimeProgressUpdater(
        IOutputWriter writer,
        string message,
        int current,
        int total,
        int width,
        bool showPercentage,
        bool showTime,
        DateTime startTime,
        TerminalCapabilityDetector.Capabilities capabilities
    ) {
        _writer = writer;
        _originalMessage = message;
        _total = total;
        _width = width;
        _showPercentage = showPercentage;
        _showTime = showTime;
        _startTime = startTime;
        _capabilities = capabilities;
        _startRow = GetCursorPosition();
    }

    public async Task UpdateAsync(int current, string? message = null) {
        if (_disposed) return;

        var displayMessage = message ?? _originalMessage;
        var progressLine = ProgressWidgets.RenderDetailedProgress(
            current, _total, _startTime, _width, _showPercentage, _showTime, true
        );

        var fullLine = $"{displayMessage} {progressLine}";
        var safeSequence = CursorControl.SafeEscape(CursorControl.Position(_startRow, 1),
            _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl));

        _writer.Write(safeSequence);
        _writer.Write(fullLine);

        if (_capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl)) {
            _writer.Write(CursorControl.ClearToEndOfLine());
        }
        _writer.WriteLine(string.Empty);

        await Task.Delay(1).ConfigureAwait(false);
    }

    public async Task CompleteAsync() {
        if (_disposed) return;

        await UpdateAsync(_total, "Complete!");
        _disposed = true;
    }

    public async ValueTask DisposeAsync() {
        await CompleteAsync();
    }

    private int GetCursorPosition() {
        // This is a simplified implementation
        // In a real implementation, you would query the cursor position
        return Console.CursorTop;
    }
}

internal class SimpleProgressUpdater : IProgressUpdater {
    private readonly IOutputWriter _writer;
    private readonly string _originalMessage;
    private readonly int _total;
    private readonly int _width;
    private readonly bool _showPercentage;

    public SimpleProgressUpdater(
        IOutputWriter writer,
        string message,
        int current,
        int total,
        int width,
        bool showPercentage
    ) {
        _writer = writer;
        _originalMessage = message;
        _total = total;
        _width = width;
        _showPercentage = showPercentage;
    }

    public async Task UpdateAsync(int current, string? message = null) {
        var displayMessage = message ?? _originalMessage;
        var percent = _total > 0 ? (int)Math.Round((double)current / _total * 100) : 0;
        var bar = ProgressWidgets.RenderBlockProgressBar(percent, _width);

        var output = displayMessage;
        if (_showPercentage) {
            output += $" ({percent}%)";
        }
        output += $" {bar}";

        _writer.WriteLine(output);
        await Task.Delay(1).ConfigureAwait(false);
    }

    public async Task CompleteAsync() {
        await UpdateAsync(_total, "Complete!");
    }

    public async ValueTask DisposeAsync() {
        await CompleteAsync();
    }
}

internal class AnsiStatusDisplay : IStatusDisplay {
    private readonly IOutputWriter _writer;
    private readonly List<string> _lines;
    private readonly TerminalCapabilityDetector.Capabilities _capabilities;
    private readonly int _startRow;
    private bool _disposed;

    public AnsiStatusDisplay(
        IOutputWriter writer,
        List<string> lines,
        TerminalCapabilityDetector.Capabilities capabilities
    ) {
        _writer = writer;
        _lines = lines;
        _capabilities = capabilities;
        _startRow = GetCursorPosition();
        DisplayLines();
    }

    public async Task UpdateLineAsync(int lineIndex, string text) {
        if (_disposed || lineIndex < 0 || lineIndex >= _lines.Count) return;

        _lines[lineIndex] = text;
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async Task UpdateLinesAsync(IDictionary<int, string> updates) {
        if (_disposed) return;

        foreach (var update in updates) {
            if (update.Key >= 0 && update.Key < _lines.Count) {
                _lines[update.Key] = update.Value;
                }
        }
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async Task AddLineAsync(string text) {
        if (_disposed) return;

        _lines.Add(text);
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async Task RemoveLineAsync(int lineIndex) {
        if (_disposed || lineIndex < 0 || lineIndex >= _lines.Count) return;

        _lines.RemoveAt(lineIndex);
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;
    }

    private async Task DisplayLinesAsync() {
        var safeSequence = CursorControl.SafeEscape(CursorControl.Position(_startRow, 1),
            _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl));

        _writer.Write(safeSequence);

        foreach (var line in _lines) {
            _writer.WriteLine(line);
            if (_capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl)) {
                _writer.Write(CursorControl.ClearToEndOfLine());
            }
        }
    }

    private void DisplayLines() {
        DisplayLinesAsync().GetAwaiter().GetResult();
    }

    private int GetCursorPosition() {
        // Simplified implementation
        return Console.CursorTop;
    }
}

internal class SimpleStatusDisplay : IStatusDisplay {
    private readonly IOutputWriter _writer;
    private readonly List<string> _lines;
    private bool _disposed;

    public SimpleStatusDisplay(IOutputWriter writer, List<string> lines) {
        _writer = writer;
        _lines = lines;
        DisplayLines();
    }

    public async Task UpdateLineAsync(int lineIndex, string text) {
        if (_disposed || lineIndex < 0 || lineIndex >= _lines.Count) return;

        _lines[lineIndex] = text;
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async Task UpdateLinesAsync(IDictionary<int, string> updates) {
        if (_disposed) return;

        foreach (var update in updates) {
            if (update.Key >= 0 && update.Key < _lines.Count) {
                _lines[update.Key] = update.Value;
            }
        }
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async Task AddLineAsync(string text) {
        if (_disposed) return;

        _lines.Add(text);
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async Task RemoveLineAsync(int lineIndex) {
        if (_disposed || lineIndex < 0 || lineIndex >= _lines.Count) return;

        _lines.RemoveAt(lineIndex);
        await DisplayLinesAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;
    }

    private async Task DisplayLinesAsync() {
        foreach (var line in _lines) {
            _writer.WriteLine(line);
        }
        await Task.Delay(1).ConfigureAwait(false);
    }

    private void DisplayLines() {
        DisplayLinesAsync().GetAwaiter().GetResult();
    }
}

internal class AnsiMultiProgressDisplay : IMultiProgressDisplay {
    private readonly IOutputWriter _writer;
    private readonly List<string> _operations;
    private readonly TerminalCapabilityDetector.Capabilities _capabilities;
    private readonly int _startRow;
    private readonly List<(int current, int total, string? status)> _progress;
    private bool _disposed;

    public AnsiMultiProgressDisplay(
        IOutputWriter writer,
        List<string> operations,
        TerminalCapabilityDetector.Capabilities capabilities
    ) {
        _writer = writer;
        _operations = operations;
        _capabilities = capabilities;
        _startRow = GetCursorPosition();
        _progress = operations.Select<string, (int, int, string?)>(_ => (0, 1, null)).ToList();
        DisplayProgress();
    }

    public async Task UpdateProgressAsync(int operationIndex, int current, int total, string? status = null) {
        if (_disposed || operationIndex < 0 || operationIndex >= _operations.Count) return;

        _progress[operationIndex] = (current, total, status);
        await DisplayProgressAsync().ConfigureAwait(false);
    }

    public async Task CompleteOperationAsync(int operationIndex) {
        if (_disposed || operationIndex < 0 || operationIndex >= _operations.Count) return;

        var (_, total, _) = _progress[operationIndex];
        _progress[operationIndex] = (total, total, "Complete");
        await DisplayProgressAsync().ConfigureAwait(false);
    }

    public async Task CompleteAllAsync() {
        if (_disposed) return;

        for (int i = 0; i < _progress.Count; i++) {
            await CompleteOperationAsync(i).ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;
    }

    private async Task DisplayProgressAsync() {
        var safeSequence = CursorControl.SafeEscape(CursorControl.Position(_startRow, 1),
            _capabilities.HasFlag(TerminalCapabilityDetector.Capabilities.CursorControl));

        _writer.Write(safeSequence);

        for (int i = 0; i < _operations.Count; i++) {
            var (current, total, status) = _progress[i];
            var operation = _operations[i];
            var percent = total > 0 ? (int)Math.Round((double)current / total * 100) : 0;
            var bar = ProgressWidgets.RenderBlockProgressBar(percent, 30);

            var line = $"{operation} {bar}";
            if (!string.IsNullOrEmpty(status)) {
                line += $" {status}";
            }
            if (percent < 100) {
                line += $" ({percent}%)";
            }

            _writer.WriteLine(line);
        }

        await Task.Delay(1).ConfigureAwait(false);
    }

    private void DisplayProgress() {
        DisplayProgressAsync().GetAwaiter().GetResult();
    }

    private int GetCursorPosition() {
        // Simplified implementation
        return Console.CursorTop;
    }
}

internal class SimpleMultiProgressDisplay : IMultiProgressDisplay {
    private readonly IOutputWriter _writer;
    private readonly List<string> _operations;
    private readonly List<(int current, int total, string? status)> _progress;
    private bool _disposed;

    public SimpleMultiProgressDisplay(IOutputWriter writer, List<string> operations) {
        _writer = writer;
        _operations = operations;
        _progress = operations.Select<string, (int, int, string?)>(_ => (0, 1, null)).ToList();
        DisplayProgress();
    }

    public async Task UpdateProgressAsync(int operationIndex, int current, int total, string? status = null) {
        if (_disposed || operationIndex < 0 || operationIndex >= _operations.Count) return;

        _progress[operationIndex] = (current, total, status);
        await DisplayProgressAsync().ConfigureAwait(false);
    }

    public async Task CompleteOperationAsync(int operationIndex) {
        if (_disposed || operationIndex < 0 || operationIndex >= _operations.Count) return;

        var (_, total, _) = _progress[operationIndex];
        _progress[operationIndex] = (total, total, "Complete");
        await DisplayProgressAsync().ConfigureAwait(false);
    }

    public async Task CompleteAllAsync() {
        if (_disposed) return;

        for (int i = 0; i < _progress.Count; i++) {
            await CompleteOperationAsync(i).ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync() {
        _disposed = true;
    }

    private async Task DisplayProgressAsync() {
        for (int i = 0; i < _operations.Count; i++) {
            var (current, total, status) = _progress[i];
            var operation = _operations[i];
            var percent = total > 0 ? (int)Math.Round((double)current / total * 100) : 0;
            var bar = ProgressWidgets.RenderBlockProgressBar(percent, 30);

            var line = $"{operation} {bar}";
            if (!string.IsNullOrEmpty(status)) {
                line += $" {status}";
            }
            if (percent < 100) {
                line += $" ({percent}%)";
            }

            _writer.WriteLine(line);
        }

        await Task.Delay(1).ConfigureAwait(false);
    }

    private void DisplayProgress() {
        DisplayProgressAsync().GetAwaiter().GetResult();
    }
}