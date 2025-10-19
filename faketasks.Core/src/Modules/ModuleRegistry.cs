using faketasks.Core.Data;
namespace faketasks.Core.Modules;

/// <summary>
///     Centralized registry for all available fake modules.
///     Provides module discovery and instantiation with dependency injection.
/// </summary>
public static class ModuleRegistry {
    /// <summary>
    ///     Gets all available modules with their dependencies injected.
    /// </summary>
    /// <param name="dataProvider">Data provider to inject into modules.</param>
    /// <returns>List of all registered modules.</returns>
    public static List<IFakeModule> GetAllModules(ITypedDataProvider dataProvider) {
        ArgumentNullException.ThrowIfNull( dataProvider );

        return new List<IFakeModule> {
            new BootlogModule( dataProvider ),
            // Future modules will be added here:
            // new CargoModule( dataProvider ),
            // new DockerModule( dataProvider ),
            // new TerraformModule( dataProvider ),
        };
    }

    /// <summary>
    ///     Gets a specific module by name.
    /// </summary>
    /// <param name="name">Module name (case-insensitive).</param>
    /// <param name="dataProvider">Data provider to inject into the module.</param>
    /// <returns>The requested module, or null if not found.</returns>
    public static IFakeModule? GetModuleByName(string name, ITypedDataProvider dataProvider) {
        ArgumentNullException.ThrowIfNull( name );
        ArgumentNullException.ThrowIfNull( dataProvider );

        var allModules = GetAllModules( dataProvider );
        return allModules.FirstOrDefault( m =>
            string.Equals( m.Name, name, StringComparison.OrdinalIgnoreCase )
        );
    }

    /// <summary>
    ///     Gets the names of all registered modules.
    /// </summary>
    /// <returns>Read-only list of module names.</returns>
    public static IReadOnlyList<string> GetModuleNames() {
        // Create a temporary data provider just to enumerate module names
        // This is cheap because we're not actually loading any data
        var tempProvider = new EmbeddedDataProvider();
        var modules = GetAllModules( tempProvider );
        return modules.Select( m => m.Name ).ToList().AsReadOnly();
    }
}
