using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

[UsedImplicitly]
[PublicAPI]
public class AssemblyDefinitionLoader : IDefinitionLoader
{
	private static readonly XmlReaderSettings _xmlReaderSettings = new() { IgnoreComments = true };
	
	/// <summary>
	/// Assembly of the Definition store (defaults to the implementation assembly).
	/// </summary>
	protected Assembly Assembly { get; set; } = typeof(DefaultDefinitionStore).Assembly;

	/// <summary>
	/// Version -> Definitions Directory
	/// </summary>
	protected Dictionary<string, DefinitionDirectory> DirectoryCache { get; set; } = new();
	
	/// <inheritdoc />
	public Version[] GetSupportedVersions()
	{
		string versionsDirectory = JoinPath(Assembly.FullName!.GetStringBeforeIndex(','), "Definitions", "Versions");
		return Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(versionsDirectory))
			.Select(name => name.GetStringAfterLength(versionsDirectory + '.').GetStringBeforeIndex('.')[1..])
			.Distinct()
			.Select(static version => version.Split('_').Select(int.Parse).ToArray())
			.Select(static arr => new Version(arr[0], arr[1], arr[2]))
			.OrderByDescending(static version => version)
			.ToArray();
	}

	/// <inheritdoc />
	public virtual XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames)
	{
		DefinitionDirectory directory = FindDirectory(clientVersion, directoryNames);

		if (directory.Files.SingleOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) is not { } file)
		{
			throw new InvalidOperationException("File could not be found");
		}
		
		XmlReader reader = XmlReader.Create(Assembly.GetManifestResourceStream(file.Path) ?? throw new InvalidOperationException("File not found"), _xmlReaderSettings);
		XmlDocument xmlDocument = new();
		xmlDocument.Load(reader);

		return xmlDocument;
	}

	/// <summary>
	/// Finds a definition directory by given names.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <param name="directoryNames">The names of the directories.</param>
	/// <returns>The definition directory.</returns>
	public virtual DefinitionDirectory FindDirectory(Version clientVersion, IEnumerable<string> directoryNames)
	{
		DefinitionDirectory folder = GetRootDirectory(clientVersion);

		foreach (string? folderName in directoryNames)
		{
			DefinitionDirectory? foundFolder = folder.Directories.SingleOrDefault(f => f.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase));

			if (foundFolder is null)
			{
				break;
			}

			folder = foundFolder;
		}

		return folder;
	}

	/// <summary>
	/// Gets the root definition directory for a given game client version.
	/// </summary>
	/// <param name="clientVersion">The game client version.</param>
	/// <returns>The root definition directory.</returns>
	public virtual DefinitionDirectory GetRootDirectory(Version clientVersion)
	{
		if (DirectoryCache.TryGetValue(clientVersion.ToString(), out DefinitionDirectory? rootDirectory))
		{
			return rootDirectory;
		}

		string scriptsDirectory = JoinPath(Assembly.FullName!.GetStringBeforeIndex(Consts.Comma),
			"Definitions", "Versions", $"{Consts.Underscore}{clientVersion.ToString().Replace(Consts.Dot, Consts.Underscore)}", "scripts"
		);

		string[] fileNames = Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(scriptsDirectory))
			.ToArray();

		rootDirectory = new("scripts", scriptsDirectory, fileNames);
		DirectoryCache.Add(clientVersion.ToString(), rootDirectory);

		return rootDirectory;
	}

	/// <summary>
	/// Joins parts of an XML path, separated by a dot.
	/// </summary>
	/// <param name="parts">Parts of the path.</param>
	/// <returns>The joined path.</returns>
	protected static string JoinPath(params string[] parts) => string.Join('.', parts);
}
