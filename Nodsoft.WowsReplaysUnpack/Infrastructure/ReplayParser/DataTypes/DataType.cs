using System.IO;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.DataTypes;

public abstract class DataType
{
	private int _dataSize;
	private int _headerSize;
	private object? _defaultValue;

	protected DataType(int dataSize, object? defaultValue, int headerSize = 1)
	{
		_dataSize = dataSize;
		_headerSize = headerSize;
		_defaultValue = defaultValue;
	}

	public virtual int DataSize => _dataSize;

	public static T? FromSection<T>(string alias, object section, int headerSize) where T : DataType
	{
		return null;
	}

	public object? GetDefaultValue(XmlElement? element)
	{
		if (element is null)
		{
			return _defaultValue;
		}

		return GetDefaultValueFromSection(element);
	}

	public object CreateFromStream(Stream stream, int headerSize = 1) => GetValueFromStream(stream, headerSize);

	protected abstract object GetValueFromStream(Stream stream, int headerSize = 1);

	protected abstract object GetDefaultValueFromSection(XmlElement element);
}