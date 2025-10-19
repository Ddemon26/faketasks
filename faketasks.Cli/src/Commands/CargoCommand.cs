using System.ComponentModel;
using faketasks.Cli.Infrastructure;
using faketasks.Cli.Infrastructure.Settings;
using faketasks.Core.Configuration;
using Spectre.Console.Cli;
namespace faketasks.Cli.Commands;

[Description( "Simulates Rust cargo build operations" )]
public sealed class CargoCommand : AsyncCommand<CargoSettings> {
    public override async Task<int> ExecuteAsync(CommandContext context, CargoSettings settings) {
        // Create configuration from settings
        var config = new GeneratorConfig {
            SpeedFactor = settings.SpeedFactor,
            InstantPrintLines = settings.InstantPrintLines,
            ExitAfterModules = settings.ModuleCount ?? (settings.TestMode ? 1 : null),
            ExitAfterTime = settings.TimeLimit.HasValue
                ? TimeSpan.FromSeconds( settings.TimeLimit.Value )
                : null,
        };

        // Create scheduler for cargo module only
        (var scheduler, var cts) = SchedulerFactory.CreateScheduler( config, new[] { "cargo" } );

        // Run scheduler with standard error handling
        return await SchedulerFactory.RunSchedulerAsync( scheduler, cts );
    }
}
