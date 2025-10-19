using faketasks.Core.Configuration;
using faketasks.Core.Data;
using faketasks.Core.Data.Models;
using faketasks.Core.Helpers;
using faketasks.Core.IO;
using faketasks.Core.Modules;
using faketasks.Core.Orchestration;

// Bootstrap the application
var cts = new CancellationTokenSource();

// Handle Ctrl+C gracefully
Console.CancelKeyPress += (sender, e) => {
    e.Cancel = true; // Prevent immediate termination
    cts.Cancel();
    Console.WriteLine( "\n\nSaving work to disk..." );
};

try {
    // Create configuration
    var config = new GeneratorConfig {
        SpeedFactor = 1.5, // Faster for demo
        InstantPrintLines = 0,
        ExitAfterModules = 2, // Run two module iterations
    };

    // Initialize data provider
    var dataProvider = new EmbeddedDataProvider();

    // Set up modules and scheduler
    List<IFakeModule> modules = new() { new EnhancedDemoModule( dataProvider ) };
    var outputWriter = new ConsoleOutputWriter();
    var scheduler = new ModuleScheduler( modules, config, outputWriter );

    Console.WriteLine( "faketasks - Helper Utilities Showcase" );
    Console.WriteLine( "======================================\n" );

    // Run the scheduler
    await scheduler.RunAsync( cts.Token );

    Console.WriteLine( "\n\nDemo completed successfully!" );
}
catch (OperationCanceledException) {
    // Normal cancellation path
    Console.WriteLine( "Operation cancelled." );
}
catch (Exception ex) {
    Console.WriteLine( $"Error: {ex.Message}" );
    Console.WriteLine( ex.StackTrace );
    return 1;
}

return 0;

// Enhanced demo module showcasing all utilities
internal class EnhancedDemoModule : IFakeModule {
    readonly EmbeddedDataProvider _dataProvider;

    public EnhancedDemoModule(EmbeddedDataProvider dataProvider) {
        _dataProvider = dataProvider;
    }

    public string Name => "enhanced-demo";
    public string Signature => "Showcases all helper utilities and data loading";

    public async Task RunAsync(IModuleContext context, CancellationToken cancellationToken) {
        // Load data asynchronously
        var wordsData = await _dataProvider.GetWordsDataAsync();
        var packagesData = await _dataProvider.GetPackageDataAsync();
        var linuxData = await _dataProvider.GetLinuxDataAsync();

        context.WriteLine( "" );
        var header = ColorHelper.Info( $"=== {Name}: Starting Demo ===" );
        context.WriteStyled( header.text, header.color );
        context.WriteLine( "" );
        context.WriteLine( "" );

        // Demo 1: Random Generators
        await DemoRandomGenerators( context, cancellationToken );

        // Demo 2: Progress Bars
        await DemoProgressBars( context, cancellationToken );

        // Demo 3: Name Generation
        await DemoNameGeneration( context, wordsData, packagesData, linuxData, cancellationToken );

        // Demo 4: Timestamp Formatting
        await DemoTimestamps( context, cancellationToken );

        // Demo 5: Colored Output
        await DemoColoredOutput( context, cancellationToken );

        context.WriteLine( "" );
        var footer = ColorHelper.Success( $"=== {Name}: Completed ===" );
        context.WriteStyled( footer.text, footer.color );
        context.WriteLine( "\n" );

        await context.DelayAsync( TimeSpan.FromMilliseconds( 800 ), cancellationToken );
    }

    async Task DemoRandomGenerators(IModuleContext context, CancellationToken ct) {
        var section = ColorHelper.Info( "1. Random Generators" );
        context.WriteStyled( section.text, section.color );
        context.WriteLine( "" );

        context.WriteLine( $"  IPv4: {context.Random.NextIPv4()}" );
        context.WriteLine( $"  IPv6: {context.Random.NextIPv6()}" );
        context.WriteLine( $"  SHA256: {context.Random.NextSHA256Hash()}" );
        context.WriteLine( $"  Version: {context.Random.NextSemanticVersion()}" );
        context.WriteLine( $"  File Size: {ProgressHelper.FormatBytes( context.Random.NextFileSize() )}" );
        context.WriteLine( $"  Port: {context.Random.NextPort()}" );
        context.WriteLine( "" );

        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct );
    }

    async Task DemoProgressBars(IModuleContext context, CancellationToken ct) {
        var section = ColorHelper.Info( "2. Progress Indicators" );
        context.WriteStyled( section.text, section.color );
        context.WriteLine( "" );

        for (var i = 0; i <= 100; i += 20) {
            var bar = ProgressHelper.RenderProgressBar( i, 25 );
            var pct = ProgressHelper.FormatPercentage( i );
            context.WriteLine( $"  Standard: {bar} {pct}" );
            await context.DelayAsync( TimeSpan.FromMilliseconds( 150 ), ct );
        }

        context.WriteLine( "" );
        for (var i = 0; i <= 100; i += 25) {
            var bar = ProgressHelper.RenderBlockProgressBar( i, 30 );
            context.WriteLine( $"  Block:    {bar} {i}%" );
            await context.DelayAsync( TimeSpan.FromMilliseconds( 150 ), ct );
        }

        context.WriteLine( "" );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 300 ), ct );
    }

    async Task DemoNameGeneration(
        IModuleContext context,
        WordsData words,
        PackageData packages,
        LinuxData linux,
        CancellationToken ct
    ) {
        var section = ColorHelper.Info( "3. Name Generation" );
        context.WriteStyled( section.text, section.color );
        context.WriteLine( "" );

        var crateName = NameGenerator.GenerateCrateName( context.Random, words.Adjectives, words.Nouns );
        context.WriteLine( $"  Crate: {crateName}" );

        var dockerImage = NameGenerator.GenerateDockerImage( context.Random, packages.NpmPackages );
        context.WriteLine( $"  Docker: {dockerImage}" );

        var device = NameGenerator.GenerateDeviceName( context.Random, linux.Devices );
        context.WriteLine( $"  Device: /dev/{device}" );

        var hostname = NameGenerator.GenerateHostname( context.Random, words.Adjectives, words.Nouns );
        context.WriteLine( $"  Hostname: {hostname}" );

        var commitSha = NameGenerator.GenerateCommitSha( context.Random );
        context.WriteLine( $"  Commit: {commitSha}" );

        context.WriteLine( "" );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct );
    }

    async Task DemoTimestamps(IModuleContext context, CancellationToken ct) {
        var section = ColorHelper.Info( "4. Timestamp Formatting" );
        context.WriteStyled( section.text, section.color );
        context.WriteLine( "" );

        var now = DateTime.UtcNow;
        var duration = TimeSpan.FromSeconds( 123.456 );

        context.WriteLine( $"  Boot Time: {TimestampHelper.FormatBootTime( 0.123456 )}" );
        context.WriteLine( $"  ISO 8601: {TimestampHelper.FormatISO8601( now )}" );
        context.WriteLine( $"  Syslog: {TimestampHelper.FormatSyslog( now )}" );
        context.WriteLine( $"  Duration: {TimestampHelper.FormatDuration( duration )}" );
        context.WriteLine( $"  Relative: {TimestampHelper.FormatRelativeTime( TimeSpan.FromSeconds( 90 ) )}" );
        context.WriteLine( $"  Cargo Time: {TimestampHelper.FormatCargoTime( TimeSpan.FromSeconds( 2.34 ) )}" );

        context.WriteLine( "" );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct );
    }

    async Task DemoColoredOutput(IModuleContext context, CancellationToken ct) {
        var section = ColorHelper.Info( "5. Semantic Coloring" );
        context.WriteStyled( section.text, section.color );
        context.WriteLine( "" );

        var success = ColorHelper.Success( "  ✓ Success: Operation completed" );
        context.WriteStyled( success.text, success.color );
        context.WriteLine( "" );

        var warning = ColorHelper.Warning( "  ⚠ Warning: Deprecated API used" );
        context.WriteStyled( warning.text, warning.color );
        context.WriteLine( "" );

        var error = ColorHelper.Error( "  ✗ Error: Connection failed" );
        context.WriteStyled( error.text, error.color );
        context.WriteLine( "" );

        var info = ColorHelper.Info( "  ℹ Info: Loading resources..." );
        context.WriteStyled( info.text, info.color );
        context.WriteLine( "" );

        context.WriteLine( "" );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct );
    }
}
