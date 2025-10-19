namespace faketasks.Core.Helpers;

/// <summary>
///     Generates domain-specific names for various modules (crates, Docker images, resources, etc.).
/// </summary>
public static class NameGenerator {
    /// <summary>
    ///     Generates a Rust crate name by combining an adjective and noun.
    /// </summary>
    public static string GenerateCrateName(Random random, IReadOnlyList<string> adjectives, IReadOnlyList<string> nouns) {
        if ( random.NextBool( 0.3 ) ) // 30% chance of single word
        {
            return random.NextElement( nouns );
        }

        // 70% chance of compound name
        string adjective = random.NextElement( adjectives );
        string noun = random.NextElement( nouns );
        string separator = random.NextBool( 0.6 ) ? "-" : "_"; // Prefer hyphens

        return $"{adjective}{separator}{noun}";
    }

    /// <summary>
    ///     Generates a Docker image reference (registry/name:tag).
    /// </summary>
    public static string GenerateDockerImage(Random random, IReadOnlyList<string> baseImages) {
        string baseImage = random.NextElement( baseImages );

        // Sometimes add a registry prefix
        if ( random.NextBool( 0.2 ) ) {
            var registries = new[] { "docker.io", "ghcr.io", "quay.io", "gcr.io" };
            string registry = random.NextElement( registries );
            return $"{registry}/{baseImage}";
        }

        return baseImage;
    }

    /// <summary>
    ///     Generates a Terraform resource name.
    /// </summary>
    public static string GenerateTerraformResourceName(Random random, string resourceType) {
        var suffixes = new[] { "main", "primary", "secondary", "backup", "prod", "staging", "dev" };
        string numbers = random.NextBool( 0.4 ) ? $"-{random.Next( 1, 100 ):D2}" : "";

        string baseName = resourceType.Replace( "aws_", "" ).Replace( "azurerm_", "" ).Replace( "_", "-" );
        string suffix = random.NextElement( suffixes );

        return $"{baseName}-{suffix}{numbers}";
    }

    /// <summary>
    ///     Generates a Linux device name with optional partition number.
    /// </summary>
    public static string GenerateDeviceName(Random random, IReadOnlyList<string> devices) {
        string device = random.NextElement( devices );

        // Add partition number for block devices
        if ( device.StartsWith( "sd" ) || device.StartsWith( "nvme" ) ) {
            if ( random.NextBool() ) {
                int partition = random.Next( 1, 5 );
                return device.StartsWith( "nvme" ) ? $"{device}p{partition}" : $"{device}{partition}";
            }
        }

        return device;
    }

    /// <summary>
    ///     Generates a realistic hostname.
    /// </summary>
    public static string GenerateHostname(Random random, IReadOnlyList<string> adjectives, IReadOnlyList<string> nouns) {
        Func<string>[] patterns = new[] {
            () => $"{random.NextElement( nouns )}-{random.Next( 1, 100 ):D2}",
            () => $"{random.NextElement( adjectives )}-{random.NextElement( nouns )}",
            () => $"{random.NextElement( nouns )}{random.Next( 1, 1000 )}",
        };

        return random.NextElement( patterns )();
    }

    /// <summary>
    ///     Generates an AWS resource ARN (Amazon Resource Name).
    /// </summary>
    public static string GenerateAwsArn(Random random, string service, string resourceType, string resourceId) {
        var region = new[] { "us-east-1", "us-west-2", "eu-west-1" };
        string accountId = random.Next( 100000000, 999999999 ).ToString() + random.Next( 100, 999 );

        return $"arn:aws:{service}:{random.NextElement( region )}:{accountId}:{resourceType}/{resourceId}";
    }

    /// <summary>
    ///     Generates a Git commit SHA (short version).
    /// </summary>
    public static string GenerateCommitSha(Random random, bool shortForm = true)
        => random.NextHexString( shortForm ? 7 : 40 );

    /// <summary>
    ///     Generates a Kubernetes resource name (DNS-1123 compliant).
    /// </summary>
    public static string GenerateK8sResourceName(Random random, IReadOnlyList<string> nouns) {
        string noun = random.NextElement( nouns ).ToLowerInvariant();
        string suffix = random.NextHexString( 5 );

        return $"{noun}-{suffix}";
    }

    /// <summary>
    ///     Generates a database connection string parameter.
    /// </summary>
    public static string GenerateDatabaseName(Random random, IReadOnlyList<string> nouns) {
        string noun = random.NextElement( nouns );
        var suffixes = new[] { "_db", "_prod", "_staging", "_main", "" };

        return $"{noun}{random.NextElement( suffixes )}";
    }

    /// <summary>
    ///     Generates a realistic username.
    /// </summary>
    public static string GenerateUsername(Random random, IReadOnlyList<string> adjectives, IReadOnlyList<string> nouns) {
        Func<string>[] patterns = new[] {
            () => random.NextElement( nouns ) + random.Next( 10, 99 ),
            () => random.NextElement( adjectives ) + random.NextElement( nouns ),
            () => random.NextElement( nouns ) + random.NextElement( adjectives ),
        };

        return random.NextElement( patterns )().ToLowerInvariant();
    }
}