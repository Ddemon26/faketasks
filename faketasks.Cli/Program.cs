using faketasks.Core.Configuration;
using faketasks.Core.IO;
using faketasks.Core.Modules;
using faketasks.Core.Orchestration;

// Bootstrap the application
var cts = new CancellationTokenSource();

// Handle Ctrl+C gracefully
Console.CancelKeyPress += (sender, e) => {
    e.Cancel = true; // Prevent immediate termination
    cts.Cancel();
    Console.WriteLine( "\n\nSaving work to disk..." );
};

try {
    // Create configuration with a 3-module limit for demo
    var config = new GeneratorConfig {
        SpeedFactor = 1.0,
        InstantPrintLines = 0,
        ExitAfterModules = 3,
    };

    // Set up modules and scheduler
    List<IFakeModule> modules = new() { new DemoModule() };
    var outputWriter = new ConsoleOutputWriter();
    var scheduler = new ModuleScheduler( modules, config, outputWriter );

    Console.WriteLine( "faketasks - Phase 1a Infrastructure Demo" );
    Console.WriteLine( "=========================================\n" );
    Console.WriteLine( "Running 3 demo module iterations (Press Ctrl+C to exit early)\n" );

    // Run the scheduler
    await scheduler.RunAsync( cts.Token );

    Console.WriteLine( "\nDemo completed successfully!" );
}
catch (OperationCanceledException) {
    // Normal cancellation path
    Console.WriteLine( "Operation cancelled." );
}
catch (Exception ex) {
    Console.WriteLine( $"Error: {ex.Message}" );
    return 1;
}

return 0;

// Example module demonstrating the infrastructure
internal class DemoModule : IFakeModule {
    public string Name => "demo";
    public string Signature => "Simulates a simple counting task";

    public async Task RunAsync(IModuleContext context, CancellationToken cancellationToken) {
        int iterations = context.Random.Next( 5, 15 );

        context.WriteStyled( $"[{Name}] ", ConsoleColor.Cyan );
        context.WriteLine( $"Starting demo task with {iterations} steps..." );

        for (var i = 1; i <= iterations && !cancellationToken.IsCancellationRequested; i++) {
            context.Write( $"  Step {i}/{iterations}: " );

            // Simulate work with styled output
            var statusColor = i % 2 == 0 ? ConsoleColor.Green : ConsoleColor.Yellow;
            context.WriteStyled( "Processing", statusColor );
            context.WriteLine( "..." );

            // Delay between lines (will be scaled by speed factor)
            await context.DelayAsync( TimeSpan.FromMilliseconds( 200 ), cancellationToken );
        }

        context.WriteStyled( $"[{Name}] ", ConsoleColor.Cyan );
        context.WriteStyled( "Completed!\n", ConsoleColor.Green );

        // Pause before next module
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), cancellationToken );
    }
}