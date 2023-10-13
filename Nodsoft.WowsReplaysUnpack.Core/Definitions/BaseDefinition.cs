using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Numerics;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Base class for all definitions (.def) files.
/// </summary>
public abstract record BaseDefinition
{
	/// <summary>
	/// Game client version of this definition file.
	/// </summary>
	protected Version ClientVersion { get; }

	/// <summary>
	/// Definition store for this definition.
	/// </summary>
	protected IDefinitionStore DefinitionStore { get; }

	/// <summary>
	/// Name of this definition file.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Represents all volatile properties in this definition file.
	/// </summary>
	public Dictionary<string, object> VolatileProperties { get; } = new();

	private readonly List<PropertyDefinition> _properties = new();
	private readonly string _folder;

	protected BaseDefinition(Version clientVersion, IDefinitionStore definitionStore, string name, string folder)
	{
		ClientVersion = clientVersion;
		DefinitionStore = definitionStore;
		Name = name;
		_folder = folder;

		ParseDefinitionFile(DefinitionStore.GetFileAsXml(ClientVersion, Name + ".def", _folder).DocumentElement!);
	}

	
	public PropertyDefinition[] GetPropertiesByFlags(EntityFlag entityFlag, bool orderBySize = false)
	{
		IEnumerable<PropertyDefinition> properties = _properties.Where(p => entityFlag.HasFlag(p.Flag));
		
		return !orderBySize 
			? properties.ToArray() 
			: properties.OrderBy(p => p.DataSize).ToArray();
	}

	protected virtual void ParseDefinitionFile(XmlElement xml)
	{
		ParseImplements(xml.SelectXmlNodes("Implements/Interface").Select(node => node.TrimmedText()).ToArray());
		ParseProperties(xml.SelectSingleNode("Properties"));
		ParseVolatile();
	}

	private void ParseImplements(IEnumerable<string> interfaces)
	{
		foreach (string @interface in interfaces)
		{
			ParseDefinitionFile(DefinitionStore.GetFileAsXml(ClientVersion, @interface + ".def", _folder, "interfaces").DocumentElement!);
		}
	}

	private void ParseProperties(XmlNode? propertiesNode)
	{
		if (propertiesNode is null)
		{
			return;
		}

		foreach (XmlNode propertyNode in propertiesNode.ChildNodes())
		{
			// when same-named properties are in interface and in definition, game client uses last one
			int propertyIndex = _properties.FindIndex(x => x.Name == propertyNode.Name);

			if (propertyIndex > -1)
			{
				_properties.RemoveAt(propertyIndex);
			}

			_properties.Add(new(ClientVersion, DefinitionStore, propertyNode));
		}
	}

	private void ParseVolatile()
	{
		foreach (PropertyDefinition? property in _properties)
		{
			if (property.Name is "position")
			{
				VolatileProperties[property.Name] = new Vector3(0f, 0f, 0f);
			}
			else if (property.Name is "yaw" or "pitch" or "roll")
			{
				VolatileProperties[property.Name] = 0.0f;
			}
		}
	}
}