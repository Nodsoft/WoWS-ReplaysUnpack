using Microsoft.Extensions.Options;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.FileStore.Definitions;

/// <summary>
/// Provides a definition loader using the filesystem as a backing store.
/// </summary>
public class FileSystemDefinitionLoader : IDefinitionLoader
{
	private static readonly XmlReaderSettings _xmlReaderSettings = new()
	{
		IgnoreComments = true
	};
	
	private readonly IOptionsMonitor<FileSystemDefinitionLoaderOptions> _options;

	public FileSystemDefinitionLoader(IOptionsMonitor<FileSystemDefinitionLoaderOptions> options)
	{
		_options = options;
	}

	/// <inheritdoc/>
	public XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		string path = Path.Combine(_options.CurrentValue.RootDirectory, ToFilesystemString(clientVersion), "scripts", Path.Combine(directoryNames), name);
		
		if (!File.Exists(path))
		{
			throw new FileNotFoundException($"Could not find definition file {name} in {path}");
		}

		using Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		
		// Use an XmlReader to load the file, as it is more efficient than loading the entire file into memory.
		using XmlReader reader = XmlReader.Create(stream, _xmlReaderSettings);
		XmlDocument document = new();
		document.Load(reader);
		return document;
	}

	/// <inheritdoc/>
	public Version[] GetSupportedVersions()
	{
		// For a set directory, we're expecting the child directories to be the versions.
		// These will be structured by <major>_<minor>_<patch> (e.g. 0_11_0),
		// so we'll need to parse them accordingly into Version objects.
		Version[] versions = new DirectoryInfo(_options.CurrentValue.RootDirectory).GetDirectories()
			.Select(static dir => FromFilesystemString(dir.Name))
			.OrderByDescending(static version => version)
			.ToArray();
		
		return versions;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string ToFilesystemString(Version version)
	{
		return version.ToString().Replace('.', '_');
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Version FromFilesystemString(string version)
	{
		return new(version.Replace('_', '.'));
	}
}