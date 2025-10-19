using faketasks.Core.IO;
using faketasks.Core.Modules;
namespace faketasks.Core.Orchestration;

/// <summary>
///     Concrete implementation of IModuleContext that modules receive during execution.
///     Bridges modules to the output writer and delay controller.
/// </summary>
internal sealed class ModuleContext : IModuleContext {
    readonly DelayController _delayController;
    readonly IOutputWriter _outputWriter;

    public ModuleContext(
        IOutputWriter outputWriter,
        DelayController delayController,
        Random random
    ) {
        _outputWriter = outputWriter ?? throw new ArgumentNullException( nameof(outputWriter) );
        _delayController = delayController ?? throw new ArgumentNullException( nameof(delayController) );
        Random = random ?? throw new ArgumentNullException( nameof(random) );
    }

    public void Write(string text) => _outputWriter.Write( text );

    public void WriteLine(string text) => _outputWriter.WriteLine( text );

    public void WriteStyled(string text, ConsoleColor? color = null)
        => _outputWriter.WriteStyled( text, color );

    public Task DelayAsync(TimeSpan baseDuration, CancellationToken cancellationToken)
        => _delayController.DelayAsync( baseDuration, cancellationToken );

    public Random Random { get; }

    public int TerminalWidth => _outputWriter.TerminalWidth;
}