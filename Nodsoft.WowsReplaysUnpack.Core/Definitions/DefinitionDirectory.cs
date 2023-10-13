using Nodsoft.WowsReplaysUnpack.Core.Extensions;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Defines a directory containing definition files.
/// </summary>
public record DefinitionDirectory
{
	/// <summary>
	/// The name of the directory.
	/// </summary>
	public string Name { get; }
	
	/// <summary>
	/// The path of the directory.
	/// </summary>
	public string Path { get; }

	/// <summary>
	/// Files in the directory.
	/// </summary>
	public List<DefinitionFile> Files { get; } = new();
	
	/// <summary>
	/// Directories in the directory.
	/// </summary>
	public List<DefinitionDirectory> Directories { get; } = new();

	public DefinitionDirectory(string name, string path, string[] fileNames)
	{
		Name = name;
		Path = path;
		ParseChildrenRecursive(fileNames);
	}

	private void ParseChildrenRecursive(string[] fileNames)
	{
		fileNames = fileNames.Where(name => name.StartsWith(Path)).ToArray();

		foreach (string fileName in fileNames)
		{
			string actualFileName = fileName.GetStringAfterLength(Path + '.');

			if (actualFileName.Count(static c => c is '.') is 1)
			{
				Files.Add(new(actualFileName, fileName));
			}
			else
			{
				int dotPos = actualFileName.IndexOf('.');
				string directoryName = actualFileName[..dotPos];

				if (Directories.All(d => d.Name != directoryName))
				{
					string before = $".{actualFileName[(dotPos + 1)..]}";
					Directories.Add(new(actualFileName[..dotPos], fileName[..fileName.IndexOf(before, StringComparison.Ordinal)], fileNames));
				}
			}
		}
	}
}

/// <summary>
/// Defines a file containing definition data.
/// </summary>
/// <param name="Name">Name of the file.</param>
/// <param name="Path">Path of the file.</param>
public record DefinitionFile(string Name, string Path);