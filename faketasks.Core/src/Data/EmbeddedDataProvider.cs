using System.Reflection;
using System.Text.Json;
namespace faketasks.Core.Data;

/// <summary>
///     Data provider that loads JSON resources embedded in the assembly.
///     Supports caching and provides generic access to data resources.
/// </summary>
public sealed class EmbeddedDataProvider : IDataProvider {
    readonly Assembly _assembly;
    readonly Dictionary<string, object> _cache;
    readonly JsonSerializerOptions _jsonOptions;
    readonly DataProviderOptions _options;

    public EmbeddedDataProvider(DataProviderOptions? options = null) {
        _options = options ?? DataProviderOptions.Default;
        _assembly = typeof(EmbeddedDataProvider).Assembly;
        _cache = new Dictionary<string, object>();
        _jsonOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };
    }

    /// <summary>
    ///     Loads lines from a named resource.
    ///     For JSON resources, this returns a flattened array from the specified property.
    ///     If no property specified, returns the entire file content as single lines.
    /// </summary>
    /// <param name="resourceName">Format: "filename.propertyPath" (e.g., "words.adjectives") or just "filename" for entire file</param>
    public async Task<IReadOnlyList<string>> GetLinesAsync(string resourceName) {
        if ( string.IsNullOrWhiteSpace( resourceName ) ) {
            throw new ArgumentException( "Resource name cannot be null or empty.", nameof(resourceName) );
        }

        // Check cache first
        if ( _options.EnableCaching && _cache.TryGetValue( resourceName, out object? cached ) ) {
            return (IReadOnlyList<string>)cached;
        }

        // Parse resource name (format: "filename.propertyPath" or just "filename")
        string[] parts = resourceName.Split( '.', 2 );
        string filename = parts[0];

        // Load and parse JSON
        var resourcePath = $"faketasks.Core.Data.Resources.{filename}.json";
        IReadOnlyList<string> lines;

        if ( parts.Length == 2 ) {
            // With property: "words.adjectives" -> return array from that property
            string property = parts[1];
            lines = await LoadPropertyFromJsonAsync( resourcePath, property ).ConfigureAwait( false );
        } else {
            // Without property: "linux" -> return entire file as lines
            lines = await LoadFileAsLinesAsync( resourcePath ).ConfigureAwait( false );
        }

        // Cache if enabled
        if ( _options.EnableCaching ) {
            _cache[resourceName] = lines;
        }

        return lines;
    }

  
    /// <summary>
    ///     Loads a typed model from an embedded JSON resource.
    /// </summary>
    async Task<T> LoadJsonResourceAsync<T>(string filename) where T : class, new() {
        var cacheKey = $"model:{filename}";

        if ( _options.EnableCaching && _cache.TryGetValue( cacheKey, out object? cached ) ) {
            return (T)cached;
        }

        var resourcePath = $"faketasks.Core.Data.Resources.{filename}.json";
        using var stream = _assembly.GetManifestResourceStream( resourcePath );

        if ( stream == null ) {
            throw new InvalidOperationException( $"Resource '{resourcePath}' not found in assembly." );
        }

        var model = await JsonSerializer.DeserializeAsync<T>( stream, _jsonOptions ).ConfigureAwait( false );

        if ( model == null ) {
            throw new InvalidOperationException( $"Failed to deserialize resource '{resourcePath}'." );
        }

        if ( _options.EnableCaching ) {
            _cache[cacheKey] = model;
        }

        return model;
    }

    /// <summary>
    ///     Loads an entire file as lines of text.
    /// </summary>
    async Task<IReadOnlyList<string>> LoadFileAsLinesAsync(string resourcePath) {
        using var stream = _assembly.GetManifestResourceStream( resourcePath );

        if ( stream == null ) {
            throw new InvalidOperationException( $"Resource '{resourcePath}' not found in assembly." );
        }

        using var reader = new StreamReader( stream );
        var lines = new List<string>();

        while ( await reader.ReadLineAsync().ConfigureAwait( false ) is { } line ) {
            lines.Add( line );
        }

        return lines.AsReadOnly();
    }

    /// <summary>
    ///     Loads a specific property array from a JSON resource.
    /// </summary>
    async Task<IReadOnlyList<string>> LoadPropertyFromJsonAsync(string resourcePath, string propertyName) {
        using var stream = _assembly.GetManifestResourceStream( resourcePath );

        if ( stream == null ) {
            throw new InvalidOperationException( $"Resource '{resourcePath}' not found in assembly." );
        }

        using var document = await JsonDocument.ParseAsync( stream ).ConfigureAwait( false );
        var root = document.RootElement;

        if ( !root.TryGetProperty( propertyName, out var property ) ) {
            throw new ArgumentException(
                $"Property '{propertyName}' not found in resource '{resourcePath}'.",
                nameof(propertyName)
            );
        }

        if ( property.ValueKind != JsonValueKind.Array ) {
            throw new InvalidOperationException(
                $"Property '{propertyName}' in '{resourcePath}' is not an array."
            );
        }

        List<string> result = new();
        foreach (var element in property.EnumerateArray()) {
            if ( element.ValueKind == JsonValueKind.String ) {
                string? value = element.GetString();
                if ( !string.IsNullOrEmpty( value ) ) {
                    result.Add( value );
                }
            }
        }

        return result.AsReadOnly();
    }
}