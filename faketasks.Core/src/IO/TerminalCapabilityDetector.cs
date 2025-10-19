using System.Runtime.InteropServices;

namespace faketasks.Core.IO;

/// <summary>
///     Detects terminal capabilities and features.
///     Helps determine what advanced features are available on the current terminal.
/// </summary>
public static class TerminalCapabilityDetector {
    /// <summary>
    ///     Terminal capability flags.
    /// </summary>
    [Flags]
    public enum Capabilities {
        None = 0,
        AnsiColors = 1 << 0,
        CursorControl = 1 << 1,
        Unicode = 1 << 2,
        TrueColor = 1 << 3,
        CursorPositioning = 1 << 4,
        TextFormatting = 1 << 5,
        ScreenClearing = 1 << 6,
        WindowTitle = 1 << 7,
        ProgressBars = 1 << 8,
        SpinningIndicators = 1 << 9
    }

    private static Capabilities? _cachedCapabilities;
    private static readonly object _cacheLock = new object();

    /// <summary>
    ///     Gets the detected terminal capabilities.
    /// </summary>
    public static Capabilities GetCapabilities() {
        if (_cachedCapabilities.HasValue) {
            return _cachedCapabilities.Value;
        }

        lock (_cacheLock) {
            if (_cachedCapabilities.HasValue) {
                return _cachedCapabilities.Value;
            }

            _cachedCapabilities = DetectCapabilities();
            return _cachedCapabilities.Value;
        }
    }

    /// <summary>
    ///     Checks if the terminal supports ANSI colors.
    /// </summary>
    public static bool SupportsAnsiColors => GetCapabilities().HasFlag(Capabilities.AnsiColors);

    /// <summary>
    ///     Checks if the terminal supports cursor control.
    /// </summary>
    public static bool SupportsCursorControl => GetCapabilities().HasFlag(Capabilities.CursorControl);

    /// <summary>
    ///     Checks if the terminal supports Unicode characters.
    /// </summary>
    public static bool SupportsUnicode => GetCapabilities().HasFlag(Capabilities.Unicode);

    /// <summary>
    ///     Checks if the terminal supports true color (24-bit).
    /// </summary>
    public static bool SupportsTrueColor => GetCapabilities().HasFlag(Capabilities.TrueColor);

    /// <summary>
    ///     Checks if the terminal supports cursor positioning.
    /// </summary>
    public static bool SupportsCursorPositioning => GetCapabilities().HasFlag(Capabilities.CursorPositioning);

    /// <summary>
    ///     Checks if the output is a TTY (interactive terminal).
    /// </summary>
    public static bool IsInteractive => !Console.IsOutputRedirected && !Console.IsErrorRedirected;

    /// <     Checks if running in a known terminal emulator.
    /// </summary>
    public static bool IsTerminalEmulator => GetTerminalType() != TerminalType.Unknown;

    /// <summary>
    ///     Gets the detected terminal type.
    /// </summary>
    public static TerminalType GetTerminalType() {
        string? term = Environment.GetEnvironmentVariable("TERM");
        if (string.IsNullOrEmpty(term)) {
            return TerminalType.Unknown;
        }

        return term.ToLowerInvariant() switch {
            "xterm" or "xterm-256color" or "xterm-color" => TerminalType.XTerm,
            "screen" or "screen-256color" => TerminalType.Screen,
            "tmux" or "tmux-256color" => TerminalType.Tmux,
            "linux" => TerminalType.LinuxConsole,
            "rxvt" or "rxvt-256color" => TerminalType.Rxvt,
            "alacritty" => TerminalType.Alacritty,
            "kitty" => TerminalType.Kitty,
            "wezterm" => TerminalType.WezTerm,
            "gnome" or "gnome-256color" => TerminalType.GNOMETerminal,
            "konsole" or "konsole-256color" => TerminalType.Konsole,
            "cygwin" => TerminalType.Cygwin,
            "mintty" => TerminalType.Mintty,
            "putty" => TerminalType.PuTTY,
            "windows-terminal" => TerminalType.WindowsTerminal,
            "powershell" => TerminalType.PowerShell,
            "cmd" => TerminalType.WindowsCommandPrompt,
            _ => TerminalType.Unknown
        };
    }

    /// <summary>
    ///     Detects terminal capabilities by examining environment and system information.
    /// </summary>
    private static Capabilities DetectCapabilities() {
        var capabilities = Capabilities.None;

        if (!IsInteractive) {
            // Non-interactive output - minimal capabilities
            return Capabilities.Unicode;
        }

        string? term = Environment.GetEnvironmentVariable("TERM");
        if (string.IsNullOrEmpty(term)) {
            // No TERM variable - assume basic capabilities
            return DetectBasicCapabilities();
        }

        // Terminal type detection
        var terminalType = GetTerminalType();

        // Base capabilities for all terminals
        capabilities |= Capabilities.Unicode | Capabilities.TextFormatting;

        // Color support detection
        capabilities |= DetectColorSupport(term, terminalType);

        // Cursor control detection
        capabilities |= DetectCursorSupport(term, terminalType);

        // Advanced features
        capabilities |= DetectAdvancedFeatures(term, terminalType);

        // Platform-specific capabilities
        capabilities |= DetectPlatformCapabilities();

        return capabilities;
    }

    /// <summary>
    ///     Detects basic capabilities when TERM is not set.
    /// </summary>
    private static Capabilities DetectBasicCapabilities() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            // Windows Console (legacy) - minimal capabilities
            return Capabilities.Unicode | Capabilities.TextFormatting;
        }

        // Unix-like systems usually support basic ANSI
        return Capabilities.Unicode | Capabilities.TextFormatting |
               Capabilities.AnsiColors | Capabilities.CursorControl;
    }

    /// <summary>
    ///     Detects color support based on TERM variable.
    /// </summary>
    private static Capabilities DetectColorSupport(string term, TerminalType terminalType) {
        var capabilities = Capabilities.None;

        // Check for 256 color support
        if (term.Contains("256color") || term.Contains("88color") || term.Contains("direct")) {
            capabilities |= Capabilities.AnsiColors | Capabilities.TrueColor;
        }

        // Terminal-specific color support
        capabilities |= terminalType switch {
            TerminalType.XTerm => Capabilities.AnsiColors,
            TerminalType.Alacritty => Capabilities.AnsiColors | Capabilities.TrueColor,
            TerminalType.Kitty => Capabilities.AnsiColors | Capabilities.TrueColor,
            TerminalType.WezTerm => Capabilities.AnsiColors | Capabilities.TrueColor,
            TerminalType.GNOMETerminal => Capabilities.AnsiColors | Capabilities.TrueColor,
            TerminalType.Konsole => Capabilities.AnsiColors | Capabilities.TrueColor,
            TerminalType.WindowsTerminal => Capabilities.AnsiColors | Capabilities.TrueColor,
            TerminalType.LinuxConsole => Capabilities.AnsiColors,
            TerminalType.Screen => Capabilities.AnsiColors,
            TerminalType.Tmux => Capabilities.AnsiColors,
            _ => Capabilities.AnsiColors
        };

        // Check for true color support
        if (Environment.GetEnvironmentVariable("COLORTERM")?.Contains("truecolor") == true ||
            Environment.GetEnvironmentVariable("FORCE_COLOR") == "1") {
            capabilities |= Capabilities.TrueColor;
        }

        return capabilities;
    }

    /// <summary>
    ///     Detects cursor control support.
    /// </summary>
    private static Capabilities DetectCursorSupport(string term, TerminalType terminalType) {
        var capabilities = Capabilities.None;

        // Most modern terminals support cursor control
        capabilities |= Capabilities.CursorControl | Capabilities.CursorPositioning;

        // Terminal-specific cursor capabilities
        capabilities |= terminalType switch {
            TerminalType.WindowsCommandPrompt => Capabilities.None, // Very limited
            TerminalType.PowerShell => Capabilities.CursorControl,
            TerminalType.PuTTY => Capabilities.CursorControl | Capabilities.CursorPositioning,
            _ => Capabilities.CursorControl | Capabilities.CursorPositioning | Capabilities.ScreenClearing
        };

        return capabilities;
    }

    /// <summary>
    ///     Detects advanced terminal features.
    /// </summary>
    private static Capabilities DetectAdvancedFeatures(string term, TerminalType terminalType) {
        var capabilities = Capabilities.None;

        // Screen clearing
        capabilities |= Capabilities.ScreenClearing;

        // Window title support
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            capabilities |= Capabilities.WindowTitle;
        }

        // Progress bars and spinners
        capabilities |= Capabilities.ProgressBars | Capabilities.SpinningIndicators;

        return capabilities;
    }

    /// <summary>
    ///     Detects platform-specific capabilities.
    /// </summary>
    private static Capabilities DetectPlatformCapabilities() {
        var capabilities = Capabilities.None;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            // Windows-specific checks
            var isWindowsTerminal = Environment.GetEnvironmentVariable("WT_SESSION") != null ||
                                      Environment.GetEnvironmentVariable("WT_PROFILE_ID") != null;

            if (isWindowsTerminal) {
                capabilities |= Capabilities.AnsiColors | Capabilities.TrueColor |
                           Capabilities.CursorControl | Capabilities.Unicode;
            }
        }

        return capabilities;
    }
}

/// <summary>
///     Types of terminal emulators.
/// </summary>
public enum TerminalType {
    Unknown,
    XTerm,
    Screen,
    Tmux,
    LinuxConsole,
    Rxvt,
    Alacritty,
    Kitty,
    WezTerm,
    GNOMETerminal,
    Konsole,
    Cygwin,
    Mintty,
    PuTTY,
    WindowsTerminal,
    PowerShell,
    WindowsCommandPrompt
}