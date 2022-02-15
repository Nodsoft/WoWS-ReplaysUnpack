using System.Collections.Generic;
using System.Linq;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.DataTypes;

public class PyFixedDict : Dictionary<string, object>
{
	private List<KeyValuePair<string, DataType>> _attributes;
	
	public PyFixedDict(List<KeyValuePair<string, DataType>> attributes)
	{
		_attributes = attributes;
	}

	public string GetFieldNameForIndex(int index) => _attributes[index].Key;

	public DataType GetFieldTypeForIndex(int index) => _attributes[index].Value;
}

public class PyFixedList : List<object>
{
	private DataType _elementType;

	public PyFixedList(DataType elementType)
	{
		_elementType = elementType;
	}

	public int GetFieldNameForIndex(int index) => index;

	public DataType GetElementType() => _elementType;
}