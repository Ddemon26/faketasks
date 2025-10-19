using Spectre.Console.Cli;
namespace faketasks.Cli.Infrastructure.Settings;

/// <summary>
///     Settings for the cargo command (Rust package manager simulation).
/// </summary>
public sealed class CargoSettings : ModuleSettingsBase {
    // Inherits all standard options: -t, -s, --instant, --count, --time
    // No cargo-specific options needed yet
}
