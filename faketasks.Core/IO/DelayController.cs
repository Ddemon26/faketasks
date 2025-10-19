using faketasks.Core.Configuration;
namespace faketasks.Core.IO;

/// <summary>
///     Controls delays between output lines, applying speed scaling and instant-print logic.
///     Tracks state to handle the initial burst of instant-print lines.
/// </summary>
public sealed class DelayController {
    readonly GeneratorConfig _config;
    int _linesPrintedInstantly;

    public DelayController(GeneratorConfig config) {
        _config = config ?? throw new ArgumentNullException( nameof(config) );
    }

    /// <summary>
    ///     Asynchronously delays for the specified base duration, applying speed factor scaling.
    ///     If instant-print lines remain, returns immediately without delay.
    /// </summary>
    /// <param name="baseDuration">Base delay before scaling.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    public async Task DelayAsync(TimeSpan baseDuration, CancellationToken cancellationToken) {
        // Check if we should print instantly
        if ( _linesPrintedInstantly < _config.InstantPrintLines ) {
            _linesPrintedInstantly++;
            return;
        }

        // Apply speed factor scaling
        var scaledDuration = ScaleDelay( baseDuration );

        if ( scaledDuration > TimeSpan.Zero ) {
            await Task.Delay( scaledDuration, cancellationToken ).ConfigureAwait( false );
        }
    }

    /// <summary>
    ///     Calculates the scaled delay based on the configured speed factor.
    ///     Speed factor > 1.0 reduces delay (faster), < 1.0 increases delay ( slower).
    /// </summary>
    TimeSpan ScaleDelay(TimeSpan baseDuration) {
        if ( _config.SpeedFactor <= 0 ) {
            return baseDuration; // Defensive: avoid division by zero
        }

        double scaledMilliseconds = baseDuration.TotalMilliseconds / _config.SpeedFactor;
        return TimeSpan.FromMilliseconds( Math.Max( 0, scaledMilliseconds ) );
    }

    /// <summary>
    ///     Resets the instant-print counter.
    ///     Useful if reusing the controller across multiple module runs.
    /// </summary>
    public void Reset() {
        _linesPrintedInstantly = 0;
    }
}