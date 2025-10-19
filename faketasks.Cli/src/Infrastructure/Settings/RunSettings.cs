using System.ComponentModel;
using Spectre.Console.Cli;
namespace faketasks.Cli.Infrastructure.Settings;

/// <summary>
///     Settings for the run command which can execute any module(s).
/// </summary>
public sealed class RunSettings : ModuleSettingsBase {
    [CommandOption( "-m|--modules <MODULES>" )]
    [Description( "Comma-separated list of modules to run (e.g., 'bootlog,cargo'). If not specified, runs all modules." )]
    public string? Modules { get; init; }

    /// <summary>
    ///     Parses the Modules string into a list of module names.
    ///     Returns null if Modules is not specified (meaning: run all modules).
    /// </summary>
    public IReadOnlyList<string>? GetEnabledModules() {
        if ( string.IsNullOrWhiteSpace( Modules ) ) {
            return null;
        }

        return Modules
            .Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries )
            .ToList()
            .AsReadOnly();
    }
}
