using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

/// <summary>
/// Defines an entity definition found in a .def file.
/// </summary>
public record EntityDefinition : BaseDefinition
{
	private const string ENTITY_DEFS = "entity_defs";
	
	private List<EntityMethodDefinition> CellMethods { get; set; } = new();
	// public List<EntityMethodDefinition> BaseMethods { get; set; } = new();

	/// <summary>
	/// Methods exposed by the game client.
	/// </summary>
	public List<EntityMethodDefinition> ClientMethods { get; private set; } = new();

	public EntityDefinition(Version clientVersion, IDefinitionStore definitionStore, string name) 
		: base(clientVersion, definitionStore, name, ENTITY_DEFS) { }

	/// <summary>
	/// Parses a .def file for the entity definition.
	/// </summary>
	/// <param name="xml">The XML document to parse.</param>
	protected override void ParseDefinitionFile(XmlElement xml)
	{
		base.ParseDefinitionFile(xml);
		ParseMethods(xml.SelectSingleNode("CellMethods"), CellMethods);
		//ParseMethods(xml.SelectSingleNode("BaseMethods"), BaseMethods);
		ParseMethods(xml.SelectSingleNode("ClientMethods"), ClientMethods);

		CellMethods = CellMethods.OrderBy(m => m.DataSize).ToList();
		ClientMethods = ClientMethods.OrderBy(m => m.DataSize).ToList();
	}

	private void ParseMethods(XmlNode? methodsNode, ICollection<EntityMethodDefinition> methods)
	{
		if (methodsNode is null)
		{
			return;
		}

		foreach (XmlNode node in methodsNode.ChildNodes())
		{
			methods.Add(new(ClientVersion, DefinitionStore, node));
		}
	}
}
