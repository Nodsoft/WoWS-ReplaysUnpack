﻿using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Numerics;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public abstract class ABaseDefinition
{
	protected Version ClientVersion { get; }
	protected IDefinitionStore DefinitionStore { get; }
	protected List<PropertyDefinition> Properties { get; } = new();
	public string Name { get; }
	public string Folder { get; }
	public Dictionary<string, object> VolatileProperties { get; } = new();

	public ABaseDefinition(Version clientVersion, IDefinitionStore definitionStore, string name, string folder)
	{
		ClientVersion = clientVersion;
		DefinitionStore = definitionStore;
		Name = name;
		Folder = folder;
		ParseDefinitionFile(DefinitionStore.GetFileAsXml(ClientVersion, Name + ".def", Folder).DocumentElement!);
	}
	public PropertyDefinition[] GetPropertiesByFlags(EntityFlag entityFlag, bool orderBySize = false)
	{
		IEnumerable<PropertyDefinition> properties = Properties.Where(p => entityFlag.HasFlag(p.Flag));
		if (!orderBySize)
			return properties.ToArray();
		return properties.OrderBy(p => p.DataSize).ToArray();
	}
	protected virtual void ParseDefinitionFile(XmlElement xml)
	{
		ParseImplements(xml.SelectXmlNodes("Implements/Interface").Select(node => node.TrimmedText()).ToArray());
		ParseProperties(xml.SelectSingleNode("Properties"));
		ParseVolatile();
	}

	private void ParseImplements(string[] @interfaces)
	{
		foreach (string @interface in @interfaces)
			ParseDefinitionFile(DefinitionStore.GetFileAsXml(ClientVersion, @interface + ".def", Folder, "interfaces").DocumentElement!);
	}

	private void ParseProperties(XmlNode? propertiesNode)
	{
		if (propertiesNode is null)
			return;

		foreach (XmlNode propertyNode in propertiesNode.ChildNodes())
		{
			// when same-named properties are in interface and in definition, game client uses last one
			int propertyIndex = Properties.FindIndex(x => x.Name == propertyNode.Name);
			if (propertyIndex > -1)
				Properties.RemoveAt(propertyIndex);

			Properties.Add(new PropertyDefinition(ClientVersion, DefinitionStore, propertyNode));
		}
	}

	private void ParseVolatile()
	{
		string[] singleProps = new[] { "yaw", "pitch", "roll" };
		foreach (PropertyDefinition? property in Properties)
		{
			if (property.Name == "position")
				VolatileProperties[property.Name] = new Vector3(0f, 0f, 0f);
			else if (singleProps.Contains(property.Name))
				VolatileProperties[property.Name] = 0.0f;
		}
	}
}
