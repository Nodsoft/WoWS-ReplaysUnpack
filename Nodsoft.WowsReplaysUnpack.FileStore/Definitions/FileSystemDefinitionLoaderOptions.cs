namespace Nodsoft.WowsReplaysUnpack.FileStore.Definitions;

/// <summary>
/// Defines settings for the <see cref="FileSystemDefinitionLoader"/>.
/// </summary>
public class FileSystemDefinitionLoaderOptions
{
	/// <summary>
	/// Gets or sets the path to the directory containing the definitions.
	/// </summary>
	public required string RootDirectory { get; set; }
	
	/// <summary>
	/// Gets or sets whether the loader should poll the filesystem for changes.
	/// </summary>
	public bool EnableChangePolling { get; set; }
}