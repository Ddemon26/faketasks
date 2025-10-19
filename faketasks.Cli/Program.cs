using Spectre.Console.Cli;
using faketasks.Cli.Commands;

var app = new CommandApp();

app.Configure( config => {
    config.SetApplicationName( "faketasks" );

    config.AddExample( new[] { "bootlog", "--test" } );
    config.AddExample( new[] { "bootlog", "--speed", "2.0" } );
    config.AddExample( new[] { "list-modules" } );

    // Module commands
    config.AddCommand<BootlogCommand>( "bootlog" )
        .WithAlias( "boot" )
        .WithDescription( "Simulate Linux kernel boot messages" )
        .WithExample( new[] { "bootlog" } )
        .WithExample( new[] { "bootlog", "-t" } )
        .WithExample( new[] { "bootlog", "--speed", "0.5" } )
        .WithExample( new[] { "bootlog", "--count", "5" } );

    // Utility commands
    config.AddCommand<ListModulesCommand>( "list-modules" )
        .WithAlias( "list" )
        .WithAlias( "ls" )
        .WithDescription( "List all available modules" );

    config.AddCommand<VersionCommand>( "version" )
        .WithAlias( "ver" )
        .WithDescription( "Display version information" );
} );

return await app.RunAsync( args );
