using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public interface IDefinitionLoader
{
	/// <summary>
	/// Reads XML data from a .def file (seeking through directories as needed).
	/// </summary>
	/// <param name="clientVersion">Game client version</param>
	/// <param name="name">Name of the .def file to read</param>
	/// <param name="directoryNames">Directories where to search for the .def file</param>
	/// <returns>XML data</returns>
	XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames);

	/// <summary>
	/// Gets all game client versions supported by this definition loader.
	/// </summary>
	/// <returns>An array of all supported game client versions ordered newest -> oldest.</returns>
	Version[] GetSupportedVersions();
}