using System.ComponentModel;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace faketasks.Cli.Commands;

[Description( "Display version information" )]
public sealed class VersionCommand : Command {
    public override int Execute(CommandContext context) {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? version;

        // Display banner
        var rule = new Rule( "[cyan]faketasks[/]" );
        rule.LeftJustified();
        AnsiConsole.Write( rule );

        Console.WriteLine();
        AnsiConsole.MarkupLine( $"  Version: [yellow]{informationalVersion}[/]" );
        AnsiConsole.MarkupLine( $"  Runtime: [dim].NET {Environment.Version}[/]" );
        AnsiConsole.MarkupLine( $"  Platform: [dim]{Environment.OSVersion.Platform}[/]" );

        Console.WriteLine();
        AnsiConsole.MarkupLine( "[dim]A nonsense activity generator inspired by genact[/]" );
        AnsiConsole.MarkupLine( "[dim]Repository: https://github.com/yourusername/faketasks[/]" );

        Console.WriteLine();

        return 0;
    }
}
