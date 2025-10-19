namespace faketasks.Core.Data.Models;

/// <summary>
///     Linux system-related data for bootlog and system modules.
/// </summary>
public sealed record LinuxData {
    /// <summary>
    ///     Device names (e.g., "sda", "eth0", "wlan0").
    /// </summary>
    public IReadOnlyList<string> Devices { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Kernel boot messages for bootlog simulation.
    /// </summary>
    public IReadOnlyList<string> KernelMessages { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Systemd service names (e.g., "networking.service", "ssh.service").
    /// </summary>
    public IReadOnlyList<string> SystemdServices { get; init; } = Array.Empty<string>();
}