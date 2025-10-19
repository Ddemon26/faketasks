using faketasks.Core.Data;
using faketasks.Core.Data.Models;
using faketasks.Core.Helpers;
namespace faketasks.Core.Modules;

/// <summary>
///     Simulates Rust cargo build operations with dependency resolution, compilation, and testing.
/// </summary>
public sealed class CargoModule : IFakeModule {
    readonly ITypedDataProvider _dataProvider;
    PackageData? _packageData;
    WordsData? _wordsData;

    public CargoModule(ITypedDataProvider dataProvider) {
        _dataProvider = dataProvider ?? throw new ArgumentNullException( nameof(dataProvider) );
    }

    public string Name => "cargo";
    public string Signature => "Simulates Rust cargo build operations";

    public async Task RunAsync(IModuleContext context, CancellationToken cancellationToken) {
        // Load data if not cached
        _packageData ??= await _dataProvider.GetPackageDataAsync().ConfigureAwait( false );
        _wordsData ??= await _dataProvider.GetWordsDataAsync().ConfigureAwait( false );

        // Generate cargo build sequence
        await EmitCargoUpdate( context, cancellationToken ).ConfigureAwait( false );
        await EmitDependencyResolution( context, cancellationToken ).ConfigureAwait( false );
        await EmitCompilation( context, cancellationToken ).ConfigureAwait( false );
        await EmitTesting( context, cancellationToken ).ConfigureAwait( false );
        await EmitBuildComplete( context, cancellationToken ).ConfigureAwait( false );

        // Pause before next module
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), cancellationToken ).ConfigureAwait( false );
    }

    /// <summary>
    ///     Emits cargo update message.
    /// </summary>
    async Task EmitCargoUpdate(IModuleContext context, CancellationToken ct) {
        context.WriteStyled( "Updating crates.io index\r", ConsoleColor.Cyan );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 1500 ), ct ).ConfigureAwait( false );
        context.WriteLine( "" ); // Clear the line
    }

    /// <summary>
    ///     Emits dependency downloading and resolution.
    /// </summary>
    async Task EmitDependencyResolution(IModuleContext context, CancellationToken ct) {
        var depsToDownload = context.Random.Next( 3, 8 );

        for (var i = 0; i < depsToDownload; i++) {
            var crateName = context.Random.NextElement( _packageData!.CrateNames );
            var version = $"{context.Random.Next( 1, 5 )}.{context.Random.Next( 0, 20 )}.{context.Random.Next( 0, 10 )}";
            var size = context.Random.Next( 50, 500 );

            context.WriteStyled( $"Downloading {crateName} v{version}\r", ConsoleColor.White );
            await context.DelayAsync( TimeSpan.FromMilliseconds( 800 ), ct ).ConfigureAwait( false );

            // Simulate download progress
            for (var progress = 0; progress <= 100; progress += context.Random.Next( 20, 40 )) {
                var progressBar = ProgressHelper.RenderProgressBar( progress, 20 );
                context.WriteStyled( $"  Downloading {crateName} v{version} ({size} KB) {progressBar}\r", ConsoleColor.White );
                await context.DelayAsync( TimeSpan.FromMilliseconds( 200 ), ct ).ConfigureAwait( false );
            }
            context.WriteLine( "" ); // Clear the line
        }

        context.WriteLine( "" );
    }

    /// <summary>
    ///     Emits compilation process with warnings and occasional errors.
    /// </summary>
    async Task EmitCompilation(IModuleContext context, CancellationToken ct) {
        context.WriteStyled( "Compiling myproject v0.1.0", ConsoleColor.Cyan );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct ).ConfigureAwait( false );
        context.WriteLine( "" );

        var cratesToCompile = context.Random.Next( 2, 5 );

        for (var i = 0; i < cratesToCompile; i++) {
            var crateName = i == 0 ? "myproject" : context.Random.NextElement( _packageData!.CrateNames );
            var warnings = context.Random.Next( 0, 3 );
            var hasError = context.Random.NextDouble() < 0.1; // 10% chance of error

            context.WriteStyled( $"     Compiling {crateName} v0.1.0", ConsoleColor.White );
            await context.DelayAsync( TimeSpan.FromMilliseconds( context.Random.Next( 800, 2000 ) ), ct ).ConfigureAwait( false );
            context.WriteLine( "" );

            // Emit warnings
            for (var w = 0; w < warnings; w++) {
                var warningType = context.Random.NextElement( new[] { "dead_code", "unused_imports", "unused_variables" } );
                var file = $"src/{context.Random.NextElement( _wordsData!.Nouns )}.rs";
                var line = context.Random.Next( 10, 200 );
                context.WriteStyled( $"warning: {warningType}", ConsoleColor.Yellow );
                context.WriteLine( $"  --> {file}:{line}" );
                await context.DelayAsync( TimeSpan.FromMilliseconds( 300 ), ct ).ConfigureAwait( false );
            }

            // Emit error (rare)
            if ( hasError ) {
                var errorType = context.Random.NextElement( new[] { "E0425", "E0308", "E0277" } );
                var file = $"src/{context.Random.NextElement( _wordsData!.Nouns )}.rs";
                var line = context.Random.Next( 10, 200 );
                context.WriteStyled( $"error[{errorType}]: cannot find value", ConsoleColor.Red );
                context.WriteLine( $"  --> {file}:{line}" );
                await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct ).ConfigureAwait( false );
                context.WriteStyled( "error: could not compile `myproject` due to previous error", ConsoleColor.Red );
                context.WriteLine( "" );
                return; // Stop on error
            }
        }

        context.WriteLine( "" );
    }

    /// <summary>
    ///     Emits test execution with progress and results.
    /// </summary>
    async Task EmitTesting(IModuleContext context, CancellationToken ct) {
        context.WriteStyled( "    Finished dev [unoptimized + debuginfo] target(s) in", ConsoleColor.Green );
        context.WriteLine( $" {context.Random.Next( 1, 5 )}.{context.Random.Next( 100, 999 )}s" );
        context.WriteLine( "" );

        // Running tests
        context.WriteStyled( "Running unittests src/lib.rs", ConsoleColor.Cyan );
        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct ).ConfigureAwait( false );
        context.WriteLine( "" );

        var testCount = context.Random.Next( 5, 15 );
        var passedTests = testCount - (context.Random.NextDouble() < 0.05 ? 1 : 0); // 5% chance of failure
        var failedTests = testCount - passedTests;

        for (var i = 0; i < testCount; i++) {
            var testName = $"test_{context.Random.NextElement( _wordsData!.Adjectives )}_{context.Random.NextElement( _wordsData!.Nouns )}";
            var passed = i < passedTests;

            context.Write( $"     Running `{testName}`" );

            // Test progress spinner
            for (var j = 0; j < context.Random.Next( 3, 8 ); j++) {
                var spinner = ProgressHelper.SpinnerFrames[j % ProgressHelper.SpinnerFrames.Length];
                context.WriteStyled( $"\r     Running `{testName}` {spinner}", ConsoleColor.White );
                await context.DelayAsync( TimeSpan.FromMilliseconds( 200 ), ct ).ConfigureAwait( false );
            }

            // Test result
            var status = passed ? "ok" : "FAILED";
            var color = passed ? ConsoleColor.Green : ConsoleColor.Red;
            var time = $"{context.Random.Next( 1, 100 )}.{context.Random.Next( 100, 999 )}ms";
            context.WriteStyled( $"\r     Running `{testName}` [{status}] [{time}]", color );
            context.WriteLine( "" );

            if ( !passed ) {
                await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct ).ConfigureAwait( false );
                context.WriteStyled( "error: test failed, aborting", ConsoleColor.Red );
                context.WriteLine( "" );
                return; // Stop on test failure
            }
        }

        context.WriteLine( "" );
    }

    /// <summary>
    ///     Emits final build completion with binary info.
    /// </summary>
    async Task EmitBuildComplete(IModuleContext context, CancellationToken ct) {
        context.WriteStyled( "     Running target/debug/deps/myproject", ConsoleColor.Green );
        context.WriteLine( "" );

        // Build summary
        context.WriteStyled( "    Finished test [unoptimized + debuginfo] target(s) in", ConsoleColor.Green );
        context.WriteLine( $" {context.Random.Next( 2, 8 )}.{context.Random.Next( 100, 999 )}s" );
        context.WriteLine( "" );

        // Binary info
        var binarySize = context.Random.Next( 1500, 3500 );
        context.WriteLine( $"   Compiling myproject v0.1.0" );
        context.WriteLine( $"    Finished release [optimized] target(s) in {context.Random.Next( 1, 5 )}.{context.Random.Next( 100, 999 )}s" );
        context.WriteLine( $"     Binary target/debug/myproject" );
        context.WriteLine( $"       Size: {binarySize} KB" );

        await context.DelayAsync( TimeSpan.FromMilliseconds( 500 ), ct ).ConfigureAwait( false );
    }
}
