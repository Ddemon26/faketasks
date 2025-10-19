using System.Reflection;
using System.Text.Json;
using faketasks.Core.Data.Models;

namespace faketasks.Core.Data;

/// <summary>
/// Data provider that loads JSON resources embedded in the assembly.
/// Supports caching and provides strongly-typed access to data models.
/// </summary>
public sealed class EmbeddedDataProvider : IDataProvider
{
    private readonly DataProviderOptions _options;
    private readonly Assembly _assembly;
    private readonly Dictionary<string, object> _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    public EmbeddedDataProvider(DataProviderOptions? options = null)
    {
        _options = options ?? DataProviderOptions.Default;
        _assembly = typeof(EmbeddedDataProvider).Assembly;
        _cache = new Dictionary<string, object>();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
    }

    /// <summary>
    /// Loads lines from a named resource.
    /// For JSON resources, this returns a flattened array from the specified property.
    /// </summary>
    /// <param name="resourceName">Format: "filename.propertyPath" (e.g., "words.adjectives")</param>
    public async Task<IReadOnlyList<string>> GetLinesAsync(string resourceName)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
        {
            throw new ArgumentException("Resource name cannot be null or empty.", nameof(resourceName));
        }

        // Check cache first
        if (_options.EnableCaching && _cache.TryGetValue(resourceName, out var cached))
        {
            return (IReadOnlyList<string>)cached;
        }

        // Parse resource name (format: "filename.propertyPath")
        var parts = resourceName.Split('.', 2);
        if (parts.Length != 2)
        {
            throw new ArgumentException(
                $"Invalid resource name format. Expected 'filename.property', got '{resourceName}'",
                nameof(resourceName));
        }

        var filename = parts[0];
        var property = parts[1];

        // Load and parse JSON
        var resourcePath = $"faketasks.Core.Data.Resources.{filename}.json";
        var lines = await LoadPropertyFromJsonAsync(resourcePath, property).ConfigureAwait(false);

        // Cache if enabled
        if (_options.EnableCaching)
        {
            _cache[resourceName] = lines;
        }

        return lines;
    }

    /// <summary>
    /// Loads the WordsData model from embedded resources.
    /// </summary>
    public async Task<WordsData> GetWordsDataAsync()
    {
        return await LoadJsonResourceAsync<WordsData>("words").ConfigureAwait(false);
    }

    /// <summary>
    /// Loads the LinuxData model from embedded resources.
    /// </summary>
    public async Task<LinuxData> GetLinuxDataAsync()
    {
        return await LoadJsonResourceAsync<LinuxData>("linux").ConfigureAwait(false);
    }

    /// <summary>
    /// Loads the PackageData model from embedded resources.
    /// </summary>
    public async Task<PackageData> GetPackageDataAsync()
    {
        return await LoadJsonResourceAsync<PackageData>("packages").ConfigureAwait(false);
    }

    /// <summary>
    /// Loads the InfrastructureData model from embedded resources.
    /// </summary>
    public async Task<InfrastructureData> GetInfrastructureDataAsync()
    {
        return await LoadJsonResourceAsync<InfrastructureData>("infrastructure").ConfigureAwait(false);
    }

    /// <summary>
    /// Loads a typed model from an embedded JSON resource.
    /// </summary>
    private async Task<T> LoadJsonResourceAsync<T>(string filename) where T : class, new()
    {
        var cacheKey = $"model:{filename}";

        if (_options.EnableCaching && _cache.TryGetValue(cacheKey, out var cached))
        {
            return (T)cached;
        }

        var resourcePath = $"faketasks.Core.Data.Resources.{filename}.json";
        using var stream = _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            throw new InvalidOperationException($"Resource '{resourcePath}' not found in assembly.");
        }

        var model = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions).ConfigureAwait(false);

        if (model == null)
        {
            throw new InvalidOperationException($"Failed to deserialize resource '{resourcePath}'.");
        }

        if (_options.EnableCaching)
        {
            _cache[cacheKey] = model;
        }

        return model;
    }

    /// <summary>
    /// Loads a specific property array from a JSON resource.
    /// </summary>
    private async Task<IReadOnlyList<string>> LoadPropertyFromJsonAsync(string resourcePath, string propertyName)
    {
        using var stream = _assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
        {
            throw new InvalidOperationException($"Resource '{resourcePath}' not found in assembly.");
        }

        using var document = await JsonDocument.ParseAsync(stream, default).ConfigureAwait(false);
        var root = document.RootElement;

        if (!root.TryGetProperty(propertyName, out var property))
        {
            throw new ArgumentException(
                $"Property '{propertyName}' not found in resource '{resourcePath}'.",
                nameof(propertyName));
        }

        if (property.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException(
                $"Property '{propertyName}' in '{resourcePath}' is not an array.");
        }

        var result = new List<string>();
        foreach (var element in property.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                var value = element.GetString();
                if (!string.IsNullOrEmpty(value))
                {
                    result.Add(value);
                }
            }
        }

        return result.AsReadOnly();
    }
}
