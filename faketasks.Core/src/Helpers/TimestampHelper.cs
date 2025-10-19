namespace faketasks.Core.Helpers;

/// <summary>
///     Utilities for formatting timestamps and durations in various styles.
/// </summary>
public static class TimestampHelper {
    /// <summary>
    ///     Formats a time as a Linux kernel boot timestamp.
    ///     Format: "[    0.123456] " with 4-digit seconds and 6-digit microseconds.
    /// </summary>
    public static string FormatBootTime(double seconds) {
        var wholePart = (int)seconds;
        var fractionalPart = (int)((seconds - wholePart) * 1_000_000);

        return $"[{wholePart,4}.{fractionalPart:D6}] ";
    }

    /// <summary>
    ///     Formats a TimeSpan as a kernel boot timestamp.
    /// </summary>
    public static string FormatBootTime(TimeSpan elapsed)
        => FormatBootTime( elapsed.TotalSeconds );

    /// <summary>
    ///     Formats a timestamp in ISO 8601 format with UTC timezone.
    ///     Example: "2025-01-15T14:30:45Z"
    /// </summary>
    public static string FormatISO8601(DateTime dateTime)
        => dateTime.ToUniversalTime().ToString( "yyyy-MM-ddTHH:mm:ssZ" );

    /// <summary>
    ///     Formats a timestamp in ISO 8601 format for the current time.
    /// </summary>
    public static string FormatISO8601() => FormatISO8601( DateTime.UtcNow );

    /// <summary>
    ///     Formats a relative time (e.g., "2s ago", "5m 30s ago").
    /// </summary>
    public static string FormatRelativeTime(TimeSpan elapsed) {
        if ( elapsed.TotalSeconds < 60 ) {
            return $"{(int)elapsed.TotalSeconds}s ago";
        }

        if ( elapsed.TotalMinutes < 60 ) {
            var minutes = (int)elapsed.TotalMinutes;
            int seconds = elapsed.Seconds;
            return seconds > 0 ? $"{minutes}m {seconds}s ago" : $"{minutes}m ago";
        }

        if ( elapsed.TotalHours < 24 ) {
            var hours = (int)elapsed.TotalHours;
            int minutes = elapsed.Minutes;
            return minutes > 0 ? $"{hours}h {minutes}m ago" : $"{hours}h ago";
        }

        var days = (int)elapsed.TotalDays;
        return $"{days}d ago";
    }

    /// <summary>
    ///     Formats a duration in a compact form (e.g., "1m 23.5s", "45.2s").
    /// </summary>
    public static string FormatDuration(TimeSpan duration) {
        if ( duration.TotalSeconds < 1 ) {
            return $"{duration.TotalMilliseconds:0}ms";
        }

        if ( duration.TotalSeconds < 60 ) {
            return $"{duration.TotalSeconds:0.0}s";
        }

        if ( duration.TotalMinutes < 60 ) {
            var minutes = (int)duration.TotalMinutes;
            double seconds = duration.Seconds + duration.Milliseconds / 1000.0;
            return $"{minutes}m {seconds:0.0}s";
        }

        var hours = (int)duration.TotalHours;
        int mins = duration.Minutes;
        return $"{hours}h {mins}m";
    }

    /// <summary>
    ///     Formats a duration in milliseconds.
    /// </summary>
    public static string FormatDurationMs(TimeSpan duration)
        => $"{duration.TotalMilliseconds:0.0}ms";

    /// <summary>
    ///     Formats a timestamp in syslog format.
    ///     Example: "Jan 15 14:30:45"
    /// </summary>
    public static string FormatSyslog(DateTime dateTime)
        => dateTime.ToString( "MMM dd HH:mm:ss" );

    /// <summary>
    ///     Formats current time in syslog format.
    /// </summary>
    public static string FormatSyslog() => FormatSyslog( DateTime.Now );

    /// <summary>
    ///     Formats a timestamp for Docker logs.
    ///     Example: "2025-01-15T14:30:45.123456789Z"
    /// </summary>
    public static string FormatDockerLog(DateTime dateTime)
        => dateTime.ToUniversalTime().ToString( "yyyy-MM-ddTHH:mm:ss.fffffff" ) + "Z";

    /// <summary>
    ///     Formats a precise duration with microseconds (e.g., "0.000123s").
    ///     Used for compilation times and performance metrics.
    /// </summary>
    public static string FormatPreciseDuration(TimeSpan duration) {
        if ( duration.TotalSeconds < 0.001 ) {
            return $"{duration.TotalMilliseconds * 1000:0.0}µs";
        }

        if ( duration.TotalSeconds < 1 ) {
            return $"{duration.TotalMilliseconds:0.00}ms";
        }

        return $"{duration.TotalSeconds:0.000000}s";
    }

    /// <summary>
    ///     Formats a build/compile time in Cargo style.
    ///     Examples: "Finished", "in 2.34s"
    /// </summary>
    public static string FormatCargoTime(TimeSpan duration) {
        if ( duration.TotalSeconds < 1 ) {
            return $"{duration.TotalMilliseconds:0.00}s";
        }

        if ( duration.TotalMinutes < 1 ) {
            return $"{duration.TotalSeconds:0.00}s";
        }

        return $"{duration.TotalMinutes:0.00}m";
    }

    /// <summary>
    ///     Formats an uptime duration (e.g., "2 days, 3 hours, 15 minutes").
    /// </summary>
    public static string FormatUptime(TimeSpan uptime) {
        List<string> parts = new();

        if ( uptime.Days > 0 ) {
            parts.Add( $"{uptime.Days} day{(uptime.Days == 1 ? "" : "s")}" );
        }

        if ( uptime.Hours > 0 ) {
            parts.Add( $"{uptime.Hours} hour{(uptime.Hours == 1 ? "" : "s")}" );
        }

        if ( uptime.Minutes > 0 || parts.Count == 0 ) {
            parts.Add( $"{uptime.Minutes} minute{(uptime.Minutes == 1 ? "" : "s")}" );
        }

        return string.Join( ", ", parts );
    }
}