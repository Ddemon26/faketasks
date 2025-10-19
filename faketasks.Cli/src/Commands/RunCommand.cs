using System.ComponentModel;
using faketasks.Cli.Infrastructure;
using faketasks.Cli.Infrastructure.Settings;
using faketasks.Core.Configuration;
using Spectre.Console.Cli;
namespace faketasks.Cli.Commands;

[Description( "Run fake activity modules" )]
public sealed class RunCommand : AsyncCommand<RunSettings> {
    public override async Task<int> ExecuteAsync(CommandContext context, RunSettings settings) {
        // Create configuration from settings
        var config = new GeneratorConfig {
            SpeedFactor = settings.SpeedFactor,
            InstantPrintLines = settings.InstantPrintLines,
            ExitAfterModules = settings.ModuleCount ?? (settings.TestMode ? 1 : null),
            ExitAfterTime = settings.TimeLimit.HasValue
                ? TimeSpan.FromSeconds( settings.TimeLimit.Value )
                : null,
        };

        // Get enabled modules from settings (null = all modules)
        IReadOnlyList<string>? enabledModules = settings.GetEnabledModules();

        // Create scheduler using factory
        (var scheduler, var cts) = SchedulerFactory.CreateScheduler( config, enabledModules );

        // Run scheduler with standard error handling
        return await SchedulerFactory.RunSchedulerAsync( scheduler, cts );
    }
}
