﻿using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Specifies a definition store, responsible for loading and storing entity definitions.
/// </summary>
public interface IDefinitionStore
{
	DataTypeBase GetDataType(Version clientVersion, XmlNode typeOrArgXmlNode);
	EntityDefinition GetEntityDefinitionByIndex(Version clientVersion, int index);
	EntityDefinition GetEntityDefinitionByName(Version clientVersion, string name);
	XmlDocument GetFileAsXml(Version clientVersion, string name, params string[] directoryNames);
}