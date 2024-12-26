using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.DataTypes;

internal class VectorDataType : DataTypeBase
{
	private readonly int _itemCount;

	protected VectorDataType(Version version, IDefinitionStore definitionStore, XmlNode xmlNode, int itemCount, Type clrType)
		: base(version, definitionStore, xmlNode, clrType)
	{
		_itemCount = itemCount;
	}

	/// <summary>
	/// Reads the python tuple
	/// Equivalent to tuple(struct.unpack())
	/// </summary>
	protected override object? GetValueInternal(BinaryReader reader, XmlNode? propertyOrArgumentNode, int headerSize)
	{
		// HACK: Check for nullable vector (NULLABLE_VECTOR{n})
		if (headerSize is 1 && propertyOrArgumentNode is { InnerText: { } type } && type.StartsWith("NULLABLE_VECTOR"))
		{
			return null;
		}
		
		// Size of a float value
		float[] result = new float[_itemCount];
		
		for (int i = 0; i < _itemCount; i++)
		{
		    result[i] = reader.ReadSingle();
		}
		
		return result.ToArray();
	}

	public override object? GetDefaultValue(XmlNode? propertyOrArgumentNode, bool forArray = false)
		=> propertyOrArgumentNode?.SelectSingleNodeText("Default")?.Split(' ').Select(Convert.ToSingle).ToArray();
}
