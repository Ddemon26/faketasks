using System.Diagnostics;
using faketasks.Core.Configuration;
using faketasks.Core.IO;
using faketasks.Core.Modules;
namespace faketasks.Core.Orchestration;

/// <summary>
///     Orchestrates the execution of fake modules according to configuration.
///     Randomly selects enabled modules, runs them, and enforces exit conditions.
/// </summary>
public sealed class ModuleScheduler {
    readonly IReadOnlyList<IFakeModule> _allModules;
    readonly GeneratorConfig _config;
    readonly DelayController _delayController;
    readonly IOutputWriter _outputWriter;
    readonly Random _random;

    public ModuleScheduler(
        IEnumerable<IFakeModule> modules,
        GeneratorConfig config,
        IOutputWriter outputWriter
    ) {
        _allModules = modules?.ToList() ?? throw new ArgumentNullException( nameof(modules) );
        _config = config ?? throw new ArgumentNullException( nameof(config) );
        _outputWriter = outputWriter ?? throw new ArgumentNullException( nameof(outputWriter) );
        _delayController = new DelayController( config );
        _random = new Random();

        if ( _allModules.Count == 0 ) {
            throw new ArgumentException( "At least one module must be provided.", nameof(modules) );
        }
    }

    /// <summary>
    ///     Runs the module scheduler loop until cancellation or exit conditions are met.
    ///     Repeatedly selects random modules from the enabled set and executes them.
    /// </summary>
    public async Task RunAsync(CancellationToken cancellationToken) {
        IReadOnlyList<IFakeModule> enabledModules = FilterEnabledModules();
        if ( enabledModules.Count == 0 ) {
            throw new InvalidOperationException(
                "No modules are enabled. Check configuration or module names."
            );
        }

        var context = new ModuleContext( _outputWriter, _delayController, _random );
        var stopwatch = Stopwatch.StartNew();
        var modulesRun = 0;

        while (!cancellationToken.IsCancellationRequested) {
            // Check exit conditions
            if ( ShouldExit( stopwatch.Elapsed, modulesRun ) ) {
                break;
            }

            // Select and run a random module
            var module = SelectRandomModule( enabledModules );
            await module.RunAsync( context, cancellationToken ).ConfigureAwait( false );
            modulesRun++;
        }
    }

    /// <summary>
    ///     Filters modules based on the EnabledModules configuration.
    ///     If EnabledModules is null or empty, all modules are enabled.
    /// </summary>
    IReadOnlyList<IFakeModule> FilterEnabledModules() {
        if ( _config.EnabledModules == null || _config.EnabledModules.Count == 0 ) {
            return _allModules;
        }

        HashSet<string> enabledSet = new(_config.EnabledModules, StringComparer.OrdinalIgnoreCase);
        return _allModules.Where( m => enabledSet.Contains( m.Name ) ).ToList();
    }

    /// <summary>
    ///     Randomly selects a module from the enabled set.
    /// </summary>
    IFakeModule SelectRandomModule(IReadOnlyList<IFakeModule> modules) {
        int index = _random.Next( modules.Count );
        return modules[index];
    }

    /// <summary>
    ///     Determines if the scheduler should exit based on configured limits.
    /// </summary>
    bool ShouldExit(TimeSpan elapsed, int modulesRun) {
        if ( _config.ExitAfterTime.HasValue && elapsed >= _config.ExitAfterTime.Value ) {
            return true;
        }

        if ( _config.ExitAfterModules.HasValue && modulesRun >= _config.ExitAfterModules.Value ) {
            return true;
        }

        return false;
    }
}