namespace faketasks.Core.Modules;

/// <summary>
///     Represents a single fake activity module (e.g., cargo build, bootlog, terraform).
///     Each module is responsible for generating themed output that simulates a specific task.
/// </summary>
public interface IFakeModule {
    /// <summary>
    ///     Unique identifier for this module (e.g., "cargo", "bootlog").
    ///     Used for module selection via CLI.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Brief description of what this module simulates.
    ///     Displayed when listing available modules.
    /// </summary>
    string Signature { get; }

    /// <summary>
    ///     Executes the module's fake activity loop.
    ///     Should produce output via the context and respect the cancellation token.
    /// </summary>
    /// <param name="context">Runtime context providing output, delays, and random helpers.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    Task RunAsync(IModuleContext context, CancellationToken cancellationToken);
}