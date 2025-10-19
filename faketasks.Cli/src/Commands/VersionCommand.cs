using System.ComponentModel;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
namespace faketasks.Cli.Commands;

[Description( "Display version information" )]
public sealed class VersionCommand : Command {
    public override int Execute(CommandContext context) {
        var assembly = Assembly.GetExecutingAssembly();
        string version = assembly.GetName().Version?.ToString() ?? "1.0.0";
        string informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? version;

        string? product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        string? description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        string? copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

        // Get repository URL from assembly metadata
        string? repositoryUrl = null;
        var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        foreach (var attr in metadataAttributes) {
            if ( attr.Key == "RepositoryUrl" ) {
                repositoryUrl = attr.Value;
                break;
            }
        }

        // Display banner
        var rule = new Rule( $"[cyan]{product ?? "faketasks"}[/]" );
        rule.LeftJustified();
        AnsiConsole.Write( rule );

        Console.WriteLine();
        AnsiConsole.MarkupLine( $"  Version: [yellow]{informationalVersion}[/]" );
        AnsiConsole.MarkupLine( $"  Runtime: [dim].NET {Environment.Version}[/]" );
        AnsiConsole.MarkupLine( $"  Platform: [dim]{Environment.OSVersion.Platform}[/]" );

        Console.WriteLine();
        if ( !string.IsNullOrEmpty( description ) ) {
            AnsiConsole.MarkupLine( $"[dim]{description}[/]" );
        }
        if ( !string.IsNullOrEmpty( repositoryUrl ) ) {
            AnsiConsole.MarkupLine( $"[dim]Repository: {repositoryUrl}[/]" );
        }
        if ( !string.IsNullOrEmpty( copyright ) ) {
            AnsiConsole.MarkupLine( $"[dim]{copyright}[/]" );
        }

        Console.WriteLine();

        return 0;
    }
}