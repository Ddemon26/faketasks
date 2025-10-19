using System.Text;

namespace faketasks.Core.Helpers;

/// <summary>
/// Extension methods for Random to generate various realistic fake data.
/// </summary>
public static class RandomGenerators
{
    private const string HexChars = "0123456789abcdef";

    /// <summary>
    /// Generates a random IPv4 address.
    /// </summary>
    public static string NextIPv4(this Random random)
    {
        return $"{random.Next(1, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(1, 256)}";
    }

    /// <summary>
    /// Generates a random IPv6 address.
    /// </summary>
    public static string NextIPv6(this Random random)
    {
        var segments = new string[8];
        for (int i = 0; i < 8; i++)
        {
            segments[i] = random.Next(0, 65536).ToString("x4");
        }
        return string.Join(":", segments);
    }

    /// <summary>
    /// Generates a random SHA-256 hash string (64 hex characters).
    /// </summary>
    public static string NextSHA256Hash(this Random random)
    {
        return random.NextHexString(64);
    }

    /// <summary>
    /// Generates a random MD5 hash string (32 hex characters).
    /// </summary>
    public static string NextMD5Hash(this Random random)
    {
        return random.NextHexString(32);
    }

    /// <summary>
    /// Generates a random hexadecimal string of the specified length.
    /// </summary>
    public static string NextHexString(this Random random, int length)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            sb.Append(HexChars[random.Next(HexChars.Length)]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Generates a random byte array of the specified size.
    /// </summary>
    public static byte[] NextByteArray(this Random random, int size)
    {
        var buffer = new byte[size];
        random.NextBytes(buffer);
        return buffer;
    }

    /// <summary>
    /// Generates a random semantic version string (e.g., "1.23.5").
    /// </summary>
    public static string NextSemanticVersion(this Random random)
    {
        var major = random.Next(0, 10);
        var minor = random.Next(0, 100);
        var patch = random.Next(0, 100);
        return $"{major}.{minor}.{patch}";
    }

    /// <summary>
    /// Generates a realistic compilation duration (0.5s to 30s).
    /// </summary>
    public static TimeSpan NextCompileDuration(this Random random)
    {
        return TimeSpan.FromMilliseconds(random.Next(500, 30000));
    }

    /// <summary>
    /// Generates a realistic download duration (0.1s to 5s).
    /// </summary>
    public static TimeSpan NextDownloadDuration(this Random random)
    {
        return TimeSpan.FromMilliseconds(random.Next(100, 5000));
    }

    /// <summary>
    /// Generates a realistic short operation duration (10ms to 500ms).
    /// </summary>
    public static TimeSpan NextShortDuration(this Random random)
    {
        return TimeSpan.FromMilliseconds(random.Next(10, 500));
    }

    /// <summary>
    /// Generates a random Linux-style file path.
    /// </summary>
    public static string NextLinuxPath(this Random random)
    {
        var dirs = new[] { "/usr/lib", "/opt", "/var/cache", "/home/user/.local", "/tmp" };
        var files = new[] { "module.so", "lib.a", "config.toml", "data.db", "cache.bin" };

        return $"{random.NextElement(dirs)}/{random.NextElement(files)}";
    }

    /// <summary>
    /// Generates a random Windows-style file path.
    /// </summary>
    public static string NextWindowsPath(this Random random)
    {
        var drives = new[] { "C:", "D:" };
        var dirs = new[] { "Program Files", "Windows", "Users\\Public", "ProgramData" };
        var files = new[] { "library.dll", "config.xml", "data.db", "cache.dat" };

        return $"{random.NextElement(drives)}\\{random.NextElement(dirs)}\\{random.NextElement(files)}";
    }

    /// <summary>
    /// Generates a random file size in bytes (1 KB to 10 MB).
    /// </summary>
    public static long NextFileSize(this Random random)
    {
        return random.Next(1024, 10 * 1024 * 1024);
    }

    /// <summary>
    /// Generates a random port number (1024-65535).
    /// </summary>
    public static int NextPort(this Random random)
    {
        return random.Next(1024, 65536);
    }

    /// <summary>
    /// Selects a random element from a collection.
    /// </summary>
    public static T NextElement<T>(this Random random, IReadOnlyList<T> collection)
    {
        if (collection == null || collection.Count == 0)
        {
            throw new ArgumentException("Collection cannot be null or empty.", nameof(collection));
        }

        return collection[random.Next(collection.Count)];
    }

    /// <summary>
    /// Generates a random percentage (0-100).
    /// </summary>
    public static int NextPercentage(this Random random)
    {
        return random.Next(0, 101);
    }

    /// <summary>
    /// Generates a random boolean with optional probability.
    /// </summary>
    /// <param name="random">Random instance.</param>
    /// <param name="trueProbability">Probability of true (0.0 to 1.0). Default is 0.5.</param>
    public static bool NextBool(this Random random, double trueProbability = 0.5)
    {
        return random.NextDouble() < trueProbability;
    }

    /// <summary>
    /// Generates a random selection of items from a collection.
    /// </summary>
    public static IReadOnlyList<T> NextSelection<T>(this Random random, IReadOnlyList<T> collection, int count)
    {
        if (collection == null || collection.Count == 0)
        {
            throw new ArgumentException("Collection cannot be null or empty.", nameof(collection));
        }

        if (count <= 0 || count > collection.Count)
        {
            throw new ArgumentException($"Count must be between 1 and {collection.Count}.", nameof(count));
        }

        var indices = Enumerable.Range(0, collection.Count).OrderBy(_ => random.Next()).Take(count);
        return indices.Select(i => collection[i]).ToList();
    }
}
