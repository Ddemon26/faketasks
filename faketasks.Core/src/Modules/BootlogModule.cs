using faketasks.Core.Data;
using faketasks.Core.Data.Models;
using faketasks.Core.Helpers;
namespace faketasks.Core.Modules;

/// <summary>
///     Simulates Linux kernel boot messages with realistic timestamps and hardware detection.
/// </summary>
public sealed class BootlogModule : IFakeModule {
    readonly ITypedDataProvider _dataProvider;
    LinuxData? _linuxData;
    WordsData? _wordsData;

    public BootlogModule(ITypedDataProvider dataProvider) {
        _dataProvider = dataProvider ?? throw new ArgumentNullException( nameof(dataProvider) );
    }

    public string Name => "bootlog";
    public string Signature => "Simulates Linux kernel boot messages";

    public async Task RunAsync(IModuleContext context, CancellationToken cancellationToken) {
        // Load data if not cached
        _linuxData ??= await _dataProvider.GetLinuxDataAsync().ConfigureAwait( false );
        _wordsData ??= await _dataProvider.GetWordsDataAsync().ConfigureAwait( false );

        var bootTime = 0.0;

        // Generate boot sequence
        bootTime = await EmitKernelBanner( context, bootTime, cancellationToken ).ConfigureAwait( false );
        bootTime = await EmitHardwareDetection( context, bootTime, cancellationToken ).ConfigureAwait( false );
        bootTime = await EmitFilesystemMessages( context, bootTime, cancellationToken ).ConfigureAwait( false );
        bootTime = await EmitServiceStartup( context, bootTime, cancellationToken ).ConfigureAwait( false );
        bootTime = await EmitBootComplete( context, bootTime, cancellationToken ).ConfigureAwait( false );

        // Pause before next module
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), cancellationToken ).ConfigureAwait( false );
    }

    /// <summary>
    ///     Emits kernel version banner and early initialization messages.
    /// </summary>
    async Task<double> EmitKernelBanner(IModuleContext context, double bootTime, CancellationToken ct) {
        // Kernel version
        var version = $"{context.Random.Next( 5, 7 )}.{context.Random.Next( 1, 20 )}.{context.Random.Next( 0, 50 )}";
        bootTime = await EmitMessage( context, bootTime, $"Linux version {version}-generic", ConsoleColor.Cyan, 10, 30, ct );

        // Early kernel messages
        bootTime = await EmitMessage( context, bootTime, "Command line: BOOT_IMAGE=/boot/vmlinuz root=UUID=...", null, 5, 20, ct );
        bootTime = await EmitMessage( context, bootTime, "x86/fpu: Supporting XSAVE feature", null, 5, 20, ct );
        bootTime = await EmitMessage( context, bootTime, "signal: max sigframe size: 1040", null, 5, 20, ct );

        return bootTime;
    }

    /// <summary>
    ///     Emits hardware detection messages (CPU, memory, PCI, devices).
    /// </summary>
    async Task<double> EmitHardwareDetection(IModuleContext context, double bootTime, CancellationToken ct) {
        // CPU detection
        int cpuCount = context.Random.Next( 2, 16 );
        bootTime = await EmitMessage( context, bootTime, $"smpboot: CPU{cpuCount} CPUs detected", ConsoleColor.Green, 20, 60, ct );

        // Memory
        int memoryMb = context.Random.Next( 8192, 65536 );
        bootTime = await EmitMessage( context, bootTime, $"Memory: {memoryMb}M/{memoryMb + 512}M available", ConsoleColor.Green, 15, 40, ct );

        // PCI setup
        bootTime = await EmitMessage( context, bootTime, "PCI: Using ACPI for IRQ routing", null, 10, 30, ct );
        bootTime = await EmitMessage( context, bootTime, "PCI: pci_cache_line_size set to 64 bytes", null, 10, 25, ct );

        // Random hardware devices
        int deviceCount = context.Random.Next( 5, 12 );
        for (var i = 0; i < deviceCount; i++) {
            string device = context.Random.NextElement( _linuxData!.Devices );
            string deviceMsg = GenerateDeviceMessage( context, device );
            bootTime = await EmitMessage( context, bootTime, deviceMsg, ConsoleColor.Gray, 30, 80, ct );

            if ( ct.IsCancellationRequested ) break;
        }

        // Storage devices
        string storageDevice = context.Random.NextElement( new[] { "sda", "sdb", "nvme0n1" } );
        int sizeGb = context.Random.Next( 256, 2048 );
        bootTime = await EmitMessage( context, bootTime, $"{storageDevice}: {sizeGb} GB, {context.Random.Next( 4000, 8000 )} MB/s", ConsoleColor.Green, 40, 100, ct );

        return bootTime;
    }

    /// <summary>
    ///     Emits filesystem mount and swap activation messages.
    /// </summary>
    async Task<double> EmitFilesystemMessages(IModuleContext context, double bootTime, CancellationToken ct) {
        bootTime = await EmitMessage( context, bootTime, "EXT4-fs: mounted filesystem with ordered data mode", ConsoleColor.Green, 80, 150, ct );
        bootTime = await EmitMessage( context, bootTime, "VFS: Mounted root (ext4 filesystem) readonly on device", null, 30, 70, ct );

        // Swap
        int swapMb = context.Random.Next( 2048, 8192 );
        bootTime = await EmitMessage( context, bootTime, $"Adding {swapMb}k swap on /dev/sda2", null, 40, 90, ct );
        bootTime = await EmitMessage( context, bootTime, $"Swap: total {swapMb}k, used 0k, free {swapMb}k", null, 20, 50, ct );

        // Filesystem checks
        bootTime = await EmitMessage( context, bootTime, "systemd[1]: systemd running in system mode", ConsoleColor.Cyan, 60, 120, ct );
        bootTime = await EmitMessage( context, bootTime, "systemd[1]: Detected architecture x86-64", null, 15, 40, ct );

        return bootTime;
    }

    /// <summary>
    ///     Emits systemd service startup messages.
    /// </summary>
    async Task<double> EmitServiceStartup(IModuleContext context, double bootTime, CancellationToken ct) {
        // Select random services to start
        int serviceCount = context.Random.Next( 8, 15 );
        IReadOnlyList<string> servicesToStart = context.Random.NextSelection( _linuxData!.SystemdServices, Math.Min( serviceCount, _linuxData.SystemdServices.Count ) );

        foreach (string service in servicesToStart) {
            string status = context.Random.NextBool( 0.95 ) ? "Started" : "Starting";
            var color = status == "Started" ? ConsoleColor.Green : ConsoleColor.Yellow;

            bootTime = await EmitMessage( context, bootTime, $"systemd[1]: {status} {service}", color, 100, 300, ct );

            if ( ct.IsCancellationRequested ) break;
        }

        // Network configuration
        bootTime = await EmitMessage( context, bootTime, "NetworkManager: <info> NetworkManager is now in the 'connected' state", ConsoleColor.Green, 150, 250, ct );

        return bootTime;
    }

    /// <summary>
    ///     Emits boot completion message.
    /// </summary>
    async Task<double> EmitBootComplete(IModuleContext context, double bootTime, CancellationToken ct) {
        bootTime = await EmitMessage( context, bootTime, "systemd[1]: Startup finished", ConsoleColor.Green, 80, 150, ct );

        string totalTime = TimestampHelper.FormatDuration( TimeSpan.FromSeconds( bootTime ) );
        bootTime = await EmitMessage( context, bootTime, $"Boot completed in {totalTime}", ConsoleColor.Green, 50, 100, ct );

        context.WriteLine( "" );

        return bootTime;
    }

    /// <summary>
    ///     Emits a single boot message with timestamp and optional color.
    ///     Returns the updated boot time.
    /// </summary>
    async Task<double> EmitMessage(
        IModuleContext context,
        double bootTime,
        string message,
        ConsoleColor? color,
        int minDelayMs,
        int maxDelayMs,
        CancellationToken ct
    ) {
        // Increment boot time slightly for realism
        bootTime += context.Random.NextDouble() * 0.001 + 0.0001;

        // Format and emit message
        string timestamp = TimestampHelper.FormatBootTime( bootTime );
        context.Write( timestamp );

        if ( color.HasValue ) {
            context.WriteStyled( message, color.Value );
        }
        else {
            context.Write( message );
        }

        context.WriteLine( "" );

        // Delay before next message
        int delayMs = context.Random.Next( minDelayMs, maxDelayMs );
        await context.DelayAsync( TimeSpan.FromMilliseconds( delayMs ), ct ).ConfigureAwait( false );

        return bootTime;
    }

    /// <summary>
    ///     Generates a realistic device detection message.
    /// </summary>
    string GenerateDeviceMessage(IModuleContext context, string device) {
        // Generate different message types based on device
        if ( device.StartsWith( "sd" ) || device.StartsWith( "nvme" ) ) {
            return $"{device}: detected at scsi0, channel 0, id {context.Random.Next( 0, 8 )}";
        }

        if ( device.StartsWith( "eth" ) || device.StartsWith( "wlan" ) ) {
            string mac = string.Join( ":", Enumerable.Range( 0, 6 ).Select( _ => context.Random.Next( 0, 256 ).ToString( "x2" ) ) );
            return $"{device}: link up, {context.Random.Next( 100, 10000 )} Mbps, MAC {mac}";
        }

        if ( device.StartsWith( "tty" ) ) {
            return $"{device}: initialized";
        }

        string message = context.Random.NextElement( _linuxData!.KernelMessages );
        return $"{device}: {message}";
    }
}