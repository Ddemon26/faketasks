namespace faketasks.Core.Data.Models;

/// <summary>
///     Infrastructure and cloud-related data for terraform and docker modules.
/// </summary>
public sealed record InfrastructureData {
    /// <summary>
    ///     AWS Terraform resource types (e.g., "aws_instance", "aws_s3_bucket").
    /// </summary>
    public IReadOnlyList<string> AwsResources { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Docker image names with tags (e.g., "alpine:latest", "ubuntu:22.04").
    /// </summary>
    public IReadOnlyList<string> DockerImages { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Cloud regions (e.g., "us-east-1", "eu-west-1").
    /// </summary>
    public IReadOnlyList<string> Regions { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     Azure resource types (e.g., "azurerm_virtual_machine", "azurerm_storage_account").
    /// </summary>
    public IReadOnlyList<string> AzureResources { get; init; } = Array.Empty<string>();
}