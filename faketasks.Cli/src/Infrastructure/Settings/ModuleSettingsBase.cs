using System.ComponentModel;
using Spectre.Console.Cli;
namespace faketasks.Cli.Infrastructure.Settings;

/// <summary>
///     Base settings for all module commands.
///     Provides common options like test mode, speed factor, etc.
/// </summary>
public class ModuleSettingsBase : CommandSettings {
    [CommandOption( "-t|--test" )]
    [Description( "Run once and exit (test mode)" )]
    [DefaultValue( false )]
    public bool TestMode { get; init; }

    [CommandOption( "-s|--speed <FACTOR>" )]
    [Description( "Speed multiplier (default: 1.0)" )]
    [DefaultValue( 1.0 )]
    public double SpeedFactor { get; init; } = 1.0;

    [CommandOption( "--instant <LINES>" )]
    [Description( "Number of lines to print instantly without delay" )]
    [DefaultValue( 0 )]
    public int InstantPrintLines { get; init; }

    [CommandOption( "--count <COUNT>" )]
    [Description( "Number of times to run the module (overrides test mode)" )]
    public int? ModuleCount { get; init; }

    [CommandOption( "--time <SECONDS>" )]
    [Description( "Exit after specified seconds" )]
    public int? TimeLimit { get; init; }
}