namespace faketasks.Cli.Infrastructure.Settings;

/// <summary>
/// Settings specific to the bootlog command.
/// Currently inherits all settings from base, but can be extended with bootlog-specific options.
/// </summary>
public sealed class BootlogSettings : ModuleSettingsBase {
    // Bootlog-specific settings can be added here in the future
    // For example: --kernel-version, --services-count, etc.
}
