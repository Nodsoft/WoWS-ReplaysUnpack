using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.DataTypes;

public abstract class MathDataType : DataType
{
	private readonly StructType _structType;
	
	protected MathDataType(int dataSize, DataType? defaultValue, StructType structType, int headerSize = 1)
		: base(dataSize, defaultValue, headerSize)
	{
		_structType = structType;
	}
	
	protected override object GetDefaultValueFromSection(XmlElement element) => throw new System.NotImplementedException();
}

public class Vector2 : MathDataType
{
	public Vector2() : base(8, null, new("ff"))
	{
	}

	protected override object GetValueFromStream(Stream stream, int headerSize = 1)
	{
		byte[] array = new byte[4];
		stream.Read(array);
		float value1 = BitConverter.ToSingle(array);
		stream.Read(array);
		float value2 = BitConverter.ToSingle(array);
		return (value1, value2);
	}

	protected override object GetDefaultValueFromSection(XmlElement element)
	{
		return element.InnerText.Trim().Split(' ').Select(float.Parse).ToList();
	}
}

public class Vector3 : MathDataType
{
	public Vector3() : base(12, null, new("fff"))
	{
	}

	protected override object GetValueFromStream(Stream stream, int headerSize = 1)
	{
		byte[] array = new byte[4];
		stream.Read(array);
		float value1 = BitConverter.ToSingle(array);
		stream.Read(array);
		float value2 = BitConverter.ToSingle(array);
		stream.Read(array);
		float value3 = BitConverter.ToSingle(array);
		return (value1, value2, value3);
	}
}

public class Vector4 : MathDataType
{
	public Vector4() : base(16, null, new("ffff"))
	{
	}

	protected override object GetValueFromStream(Stream stream, int headerSize = 1)
	{
		byte[] array = new byte[4];
		stream.Read(array);
		float value1 = BitConverter.ToSingle(array);
		stream.Read(array);
		float value2 = BitConverter.ToSingle(array);
		stream.Read(array);
		float value3 = BitConverter.ToSingle(array);
		stream.Read(array);
		float value4 = BitConverter.ToSingle(array);
		return (value1, value2, value3, value4);
	}
}