using System.ComponentModel;
using faketasks.Cli.Infrastructure.Settings;
using faketasks.Core.Configuration;
using faketasks.Core.Data;
using faketasks.Core.IO;
using faketasks.Core.Modules;
using faketasks.Core.Orchestration;
using Spectre.Console.Cli;
namespace faketasks.Cli.Commands;

[Description( "Simulate Linux kernel boot messages" )]
public sealed class BootlogCommand : AsyncCommand<BootlogSettings> {
    public override async Task<int> ExecuteAsync(CommandContext context, BootlogSettings settings) {
        // Create configuration from settings
        var config = new GeneratorConfig {
            SpeedFactor = settings.SpeedFactor,
            InstantPrintLines = settings.InstantPrintLines,
            ExitAfterModules = settings.ModuleCount ?? (settings.TestMode ? 1 : null),
            ExitAfterTime = settings.TimeLimit.HasValue
                ? TimeSpan.FromSeconds( settings.TimeLimit.Value )
                : null,
        };

        // Initialize module and scheduler
        var dataProvider = new EmbeddedDataProvider();
        List<IFakeModule> modules = ModuleRegistry.GetAllModules( dataProvider );
        var outputWriter = new ConsoleOutputWriter();
        var scheduler = new ModuleScheduler( modules, config, outputWriter );

        // Handle Ctrl+C gracefully
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) => {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine( "\n\nInterrupted." );
        };

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