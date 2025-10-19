using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
namespace faketasks.Cli.Commands;

[Description( "List all available modules" )]
public sealed class ListModulesCommand : Command {
    public override int Execute(CommandContext context) {
        var table = new Table();
        table.Border( TableBorder.Rounded );
        table.AddColumn( new TableColumn( "[cyan]Module[/]" ).Centered() );
        table.AddColumn( new TableColumn( "[cyan]Aliases[/]" ) );
        table.AddColumn( new TableColumn( "[cyan]Description[/]" ) );

        // Add rows for each module
        table.AddRow(
            "[green]bootlog[/]",
            "boot",
            "Simulates Linux kernel boot messages with hardware detection and service startup"
        );

        // Cargo module
        table.AddRow(
            "[green]cargo[/]",
            "build",
            "Simulates Rust cargo build with dependency resolution, compilation, and testing"
        );

        table.AddRow(
            "[dim]docker[/]",
            "[dim]container[/]",
            "[dim]Simulates Docker image build with layers (coming soon)[/]"
        );

        table.AddRow(
            "[dim]terraform[/]",
            "[dim]tf[/]",
            "[dim]Simulates Terraform infrastructure provisioning (coming soon)[/]"
        );

        AnsiConsole.Write( table );
        Console.WriteLine();

        AnsiConsole.MarkupLine( "[dim]Run [cyan]faketasks <module> --help[/] for module-specific options[/]" );

        return 0;
    }
}