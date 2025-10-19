namespace faketasks.Core.Helpers;

/// <summary>
/// Status levels for semantic coloring.
/// </summary>
public enum Status
{
    Success,
    Warning,
    Error,
    Info,
    Debug
}

/// <summary>
/// Utilities for ANSI color formatting and semantic coloring.
/// </summary>
public static class ColorHelper
{
    /// <summary>
    /// Maps status levels to console colors.
    /// </summary>
    public static ConsoleColor GetStatusColor(Status status)
    {
        return status switch
        {
            Status.Success => ConsoleColor.Green,
            Status.Warning => ConsoleColor.Yellow,
            Status.Error => ConsoleColor.Red,
            Status.Info => ConsoleColor.Cyan,
            Status.Debug => ConsoleColor.Gray,
            _ => ConsoleColor.White
        };
    }

    /// <summary>
    /// Formats text with semantic status coloring.
    /// Note: This returns a tuple; the caller must use the color with IOutputWriter.
    /// </summary>
    public static (string text, ConsoleColor color) Colorize(string text, Status status)
    {
        return (text, GetStatusColor(status));
    }

    /// <summary>
    /// Creates a success-colored text tuple.
    /// </summary>
    public static (string text, ConsoleColor color) Success(string text)
    {
        return (text, ConsoleColor.Green);
    }

    /// <summary>
    /// Creates a warning-colored text tuple.
    /// </summary>
    public static (string text, ConsoleColor color) Warning(string text)
    {
        return (text, ConsoleColor.Yellow);
    }

    /// <summary>
    /// Creates an error-colored text tuple.
    /// </summary>
    public static (string text, ConsoleColor color) Error(string text)
    {
        return (text, ConsoleColor.Red);
    }

    /// <summary>
    /// Creates an info-colored text tuple.
    /// </summary>
    public static (string text, ConsoleColor color) Info(string text)
    {
        return (text, ConsoleColor.Cyan);
    }

    /// <summary>
    /// Creates a debug-colored text tuple.
    /// </summary>
    public static (string text, ConsoleColor color) Debug(string text)
    {
        return (text, ConsoleColor.Gray);
    }

    /// <summary>
    /// Gets a color for Cargo-style build status.
    /// </summary>
    public static ConsoleColor GetCargoStatusColor(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "compiling" => ConsoleColor.Green,
            "finished" => ConsoleColor.Green,
            "running" => ConsoleColor.Green,
            "warning" => ConsoleColor.Yellow,
            "error" => ConsoleColor.Red,
            "failed" => ConsoleColor.Red,
            "downloading" => ConsoleColor.Cyan,
            "updating" => ConsoleColor.Cyan,
            "checking" => ConsoleColor.Cyan,
            _ => ConsoleColor.White
        };
    }

    /// <summary>
    /// Gets a color for Docker build status.
    /// </summary>
    public static ConsoleColor GetDockerStatusColor(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "step" => ConsoleColor.Cyan,
            "pull" => ConsoleColor.Blue,
            "build" => ConsoleColor.Green,
            "successfully built" => ConsoleColor.Green,
            "successfully tagged" => ConsoleColor.Green,
            "error" => ConsoleColor.Red,
            "warning" => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };
    }

    /// <summary>
    /// Gets a color for Terraform operation status.
    /// </summary>
    public static ConsoleColor GetTerraformStatusColor(string operation)
    {
        return operation.ToLowerInvariant() switch
        {
            "creating" => ConsoleColor.Green,
            "created" => ConsoleColor.Green,
            "modifying" => ConsoleColor.Yellow,
            "modified" => ConsoleColor.Yellow,
            "destroying" => ConsoleColor.Red,
            "destroyed" => ConsoleColor.Red,
            "refreshing" => ConsoleColor.Cyan,
            "planning" => ConsoleColor.Cyan,
            "applying" => ConsoleColor.Green,
            _ => ConsoleColor.White
        };
    }

    /// <summary>
    /// Gets a color for systemd service status.
    /// </summary>
    public static ConsoleColor GetSystemdStatusColor(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "active" => ConsoleColor.Green,
            "activating" => ConsoleColor.Yellow,
            "inactive" => ConsoleColor.Gray,
            "failed" => ConsoleColor.Red,
            "dead" => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };
    }
}
