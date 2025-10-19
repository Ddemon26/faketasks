using faketasks.Core.Data.Models;
namespace faketasks.Core.Data;

/// <summary>
///     Provides strongly-typed access to data models used by modules.
///     This interface extends IDataProvider with convenience methods for common data structures.
/// </summary>
public interface ITypedDataProvider : IDataProvider {
    /// <summary>
    ///     Loads the WordsData model containing adjectives, nouns, verbs, etc.
    /// </summary>
    Task<WordsData> GetWordsDataAsync();

    /// <summary>
    ///     Loads the LinuxData model containing kernel messages, devices, services, etc.
    /// </summary>
    Task<LinuxData> GetLinuxDataAsync();

    /// <summary>
    ///     Loads the PackageData model containing crate names, npm packages, features, etc.
    /// </summary>
    Task<PackageData> GetPackageDataAsync();

    /// <summary>
    ///     Loads the InfrastructureData model containing AWS/Azure resources, Docker images, etc.
    /// </summary>
    Task<InfrastructureData> GetInfrastructureDataAsync();
}