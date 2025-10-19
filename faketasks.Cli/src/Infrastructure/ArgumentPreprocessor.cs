namespace faketasks.Cli.Infrastructure;

/// <summary>
/// Preprocesses command-line arguments to support multiple access patterns:
/// - Subcommands: faketasks bootlog -t
/// - Module flags: faketasks -b -t or faketasks --bootlog -t
/// - Combined flags: faketasks -bt (bootlog + test)
/// </summary>
public static class ArgumentPreprocessor {
    /// <summary>
    /// Mapping of module flags to module names.
    /// </summary>
    static readonly Dictionary<string, string> ModuleFlags = new() {
        { "-b", "bootlog" },
        { "--bootlog", "bootlog" },
        { "--boot", "bootlog" }
        // Future modules:
        // { "-c", "cargo" },
        // { "--cargo", "cargo" },
        // { "-d", "docker" },
        // { "--docker", "docker" },
        // { "-tf", "terraform" },
        // { "--terraform", "terraform" }
    };

    /// <summary>
    /// Preprocesses arguments to transform module flags into subcommands.
    /// </summary>
    /// <param name="args">Original command-line arguments.</param>
    /// <returns>Tuple of (transformed arguments, detected module name or null).</returns>
    public static (string[] transformedArgs, string? moduleName) Preprocess(string[] args) {
        if (args.Length == 0) {
            return (args, null);
        }

        // Check for explicit module flags first (-b, --bootlog, etc.)
        foreach (var flag in ModuleFlags.Keys) {
            if (args.Contains( flag )) {
                var moduleName = ModuleFlags[flag];
                var transformed = TransformFlagToSubcommand( args, flag, moduleName );
                return (transformed, moduleName);
            }
        }

        // Check for combined short flags (-bt, -bs, etc.)
        var combinedFlag = args.FirstOrDefault( a =>
            a.StartsWith( "-" ) &&
            a.Length > 2 &&
            !a.StartsWith( "--" ) &&
            !a.Contains( '=' ) // Not a value like -s=2.0
        );

        if (combinedFlag != null) {
            var moduleChar = combinedFlag[1];
            var moduleShortFlag = $"-{moduleChar}";

            if (ModuleFlags.ContainsKey( moduleShortFlag )) {
                var moduleName = ModuleFlags[moduleShortFlag];
                var transformed = SplitCombinedFlags( args, combinedFlag, moduleName );
                return (transformed, moduleName);
            }
        }

        // No module flag detected, return args as-is
        return (args, null);
    }

    /// <summary>
    /// Transforms a module flag into a subcommand.
    /// Example: ["-b", "-t"] → ["bootlog", "-t"]
    /// </summary>
    static string[] TransformFlagToSubcommand(string[] args, string flag, string moduleName) {
        var result = new List<string> { moduleName };
        result.AddRange( args.Where( a => a != flag ) );
        return result.ToArray();
    }

    /// <summary>
    /// Splits combined short flags into separate flags.
    /// Example: ["-bt", "--speed", "2.0"] → ["bootlog", "-t", "--speed", "2.0"]
    /// </summary>
    static string[] SplitCombinedFlags(string[] args, string combinedFlag, string moduleName) {
        var result = new List<string> { moduleName };

        // Extract remaining characters after module flag
        var remainingChars = combinedFlag.Substring( 2 );

        // Add each remaining character as a separate flag
        foreach (var c in remainingChars) {
            result.Add( $"-{c}" );
        }

        // Add all other arguments except the combined flag
        result.AddRange( args.Where( a => a != combinedFlag ) );

        return result.ToArray();
    }

    /// <summary>
    /// Gets a formatted string showing all available module flags.
    /// </summary>
    public static string GetModuleFlagsHelp() {
        var flags = ModuleFlags
            .GroupBy( kv => kv.Value )
            .Select( g => $"  {g.Key}: {string.Join( ", ", g.Select( kv => kv.Key ) )}" );

        return string.Join( Environment.NewLine, flags );
    }
}
