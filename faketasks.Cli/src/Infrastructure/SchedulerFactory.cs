using faketasks.Core.Configuration;
using faketasks.Core.Data;
using faketasks.Core.IO;
using faketasks.Core.Modules;
using faketasks.Core.Orchestration;
namespace faketasks.Cli.Infrastructure;

/// <summary>
///     Factory for creating configured ModuleScheduler instances.
///     Centralizes bootstrapping logic to eliminate duplication across commands.
/// </summary>
public static class SchedulerFactory {
    /// <summary>
    ///     Creates a fully configured scheduler ready to run.
    /// </summary>
    /// <param name="config">Generator configuration with speed, exit conditions, etc.</param>
    /// <param name="enabledModules">Optional list of module names to filter. If null/empty, all modules are enabled.</param>
    /// <returns>Configured scheduler and cancellation token source.</returns>
    public static (ModuleScheduler scheduler, CancellationTokenSource cts) CreateScheduler(
        GeneratorConfig config,
        IReadOnlyList<string>? enabledModules = null
    ) {
        // Create configuration with module filtering
        var effectiveConfig = new GeneratorConfig {
            EnabledModules = enabledModules,
            SpeedFactor = config.SpeedFactor,
            InstantPrintLines = config.InstantPrintLines,
            ExitAfterTime = config.ExitAfterTime,
            ExitAfterModules = config.ExitAfterModules,
        };

        // Initialize dependencies
        var dataProvider = new EmbeddedDataProvider();
        List<IFakeModule> modules = ModuleRegistry.GetAllModules( dataProvider );
        IOutputWriter outputWriter = ConsoleOutputWriter.Create(); // TTY detection

        // Create scheduler
        var scheduler = new ModuleScheduler( modules, effectiveConfig, outputWriter );

        // Setup cancellation token with Ctrl+C handling
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine( "\n\nInterrupted." );
        };

        return (scheduler, cts);
    }

    /// <summary>
    ///     Runs a scheduler and handles standard error cases.
    /// </summary>
    /// <param name="scheduler">The scheduler to run.</param>
    /// <param name="cts">Cancellation token source.</param>
    /// <returns>Exit code: 0 for success, 130 for Ctrl+C, 1 for error.</returns>
    public static async Task<int> RunSchedulerAsync(ModuleScheduler scheduler, CancellationTokenSource cts) {
        try {
            await scheduler.RunAsync( cts.Token );
            return 0;
        }
        catch (OperationCanceledException) {
            return 130; // Standard exit code for Ctrl+C
        }
        catch (Exception ex) {
            Console.WriteLine( $"Error: {ex.Message}" );
            return 1;
        }
    }
}
