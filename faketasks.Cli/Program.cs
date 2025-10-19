using Spectre.Console;
using Spectre.Console.Cli;
using faketasks.Cli.Commands;
using faketasks.Cli.Infrastructure;

// Preprocess arguments to handle module flags and combined flags
var (transformedArgs, detectedModule) = ArgumentPreprocessor.Preprocess( args );

// Special case: if no args provided, show help
if (args.Length == 0) {
    transformedArgs = new[] { "--help" };
}

var app = new CommandApp();

app.Configure( config => {
    config.SetApplicationName( "faketasks" );

    // Custom help text
    config.SetApplicationVersion( "1.0.0" );

    // Module commands
    config.AddCommand<BootlogCommand>( "bootlog" )
        .WithAlias( "boot" )
        .WithDescription( "Simulate Linux kernel boot messages" )
        .WithExample( new[] { "bootlog" } )
        .WithExample( new[] { "bootlog", "-t" } )
        .WithExample( new[] { "-b" } )
        .WithExample( new[] { "-bt" } )
        .WithExample( new[] { "--bootlog", "--speed", "2.0" } );

    // Utility commands
    config.AddCommand<ListModulesCommand>( "list-modules" )
        .WithAlias( "list" )
        .WithAlias( "ls" )
        .WithDescription( "List all available modules" );

    config.AddCommand<VersionCommand>( "version" )
        .WithAlias( "ver" )
        .WithDescription( "Display version information" );
} );

// If a module was detected via flag, show a hint in debug mode
if (detectedModule != null && Environment.GetEnvironmentVariable( "FAKETASKS_DEBUG" ) == "1") {
    AnsiConsole.MarkupLine( $"[dim]Detected module via flag: {detectedModule}[/]" );
    AnsiConsole.MarkupLine( $"[dim]Transformed args: {string.Join( " ", transformedArgs )}[/]" );
}

return await app.RunAsync( transformedArgs );
