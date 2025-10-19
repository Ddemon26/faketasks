using System.Reflection;
using faketasks.Cli.Commands;
using faketasks.Cli.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

// Preprocess arguments to handle module flags and combined flags
(string[] transformedArgs, string? detectedModule) = ArgumentPreprocessor.Preprocess( args );

// Special case: if no args provided, show help
if ( args.Length == 0 ) {
    transformedArgs = new[] { "--help" };
}

var app = new CommandApp();

app.Configure( config => {
        config.SetApplicationName( "faketasks" );

        // Get version from assembly metadata
        var assembly = Assembly.GetExecutingAssembly();
        string version = assembly.GetName().Version?.ToString() ?? "1.0.0";
        config.SetApplicationVersion( version );

        // General run command (can run any modules)
        config.AddCommand<RunCommand>( "run" )
            .WithDescription( "Run fake activity modules" )
            .WithExample( "run" )
            .WithExample( "run", "--modules", "bootlog" )
            .WithExample( "run", "--modules", "bootlog,cargo", "--speed", "2.0" )
            .WithExample( "run", "-t" );

        // Specific module commands
        config.AddCommand<CargoCommand>( "cargo" )
            .WithAlias( "build" )
            .WithDescription( "Simulate Rust cargo build operations" )
            .WithExample( "cargo" )
            .WithExample( "cargo", "-t" )
            .WithExample( "-c" )
            .WithExample( "-ct" )
            .WithExample( "--cargo", "--speed", "2.0" );

        config.AddCommand<BootlogCommand>( "bootlog" )
            .WithAlias( "boot" )
            .WithDescription( "Simulate Linux kernel boot messages" )
            .WithExample( "bootlog" )
            .WithExample( "bootlog", "-t" )
            .WithExample( "-b" )
            .WithExample( "-bt" )
            .WithExample( "--bootlog", "--speed", "2.0" );

        // Utility commands
        config.AddCommand<ListModulesCommand>( "list-modules" )
            .WithAlias( "list" )
            .WithAlias( "ls" )
            .WithDescription( "List all available modules" );

        config.AddCommand<VersionCommand>( "version" )
            .WithAlias( "ver" )
            .WithDescription( "Display version information" );
    }
);

// If a module was detected via flag, show a hint in debug mode
if ( detectedModule != null && Environment.GetEnvironmentVariable( "FAKETASKS_DEBUG" ) == "1" ) {
    AnsiConsole.MarkupLine( $"[dim]Detected module via flag: {detectedModule}[/]" );
    AnsiConsole.MarkupLine( $"[dim]Transformed args: {string.Join( " ", transformedArgs )}[/]" );
}

return await app.RunAsync( transformedArgs );