using System;
using System.IO;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser.DataTypes;

public abstract class NumericType : DataType
{
	private StructType _structType;

	protected NumericType(int dataSize, StructType structType, int headerSize = 1)
		: base(dataSize, null, headerSize)
	{
		_structType = structType;
	}

	protected override object GetValueFromStream(Stream stream, int headerSize = 1)
	{
		byte[] array = new byte[DataSize];
		stream.Read(array);
		return _structType.Type switch
		{
			"b" => (sbyte)array[0],
			"h" => BitConverter.ToInt16(array),
			"i" => BitConverter.ToInt32(array),
			"q" => BitConverter.ToInt64(array),
			"B" => array[0],
			"H" => BitConverter.ToUInt16(array),
			"I" => BitConverter.ToUInt32(array),
			"Q" => BitConverter.ToUInt64(array),
			"f" => BitConverter.ToSingle(array),
			"d" => BitConverter.ToDouble(array),
			_ => throw new InvalidOperationException(),
		};
	}

	protected override object GetDefaultValueFromSection(XmlElement element)
	{
		string text = element.InnerText.Trim();
		return _structType.Type switch
		{
			"b" => sbyte.Parse(text),
			"h" => short.Parse(text),
			"i" => int.Parse(text),
			"q" => long.Parse(text),
			"B" => byte.Parse(text),
			"H" => ushort.Parse(text),
			"I" => uint.Parse(text),
			"Q" => ulong.Parse(text),
			"f" => float.Parse(text),
			"d" => double.Parse(text),
			_ => throw new InvalidOperationException(),
		};
	}
}

public class Int8 : NumericType
{
	public Int8() : base(1, new("b"))
	{
	}
}

public class Int16 : NumericType
{
	public Int16() : base(2, new("h"))
	{
	}
}

public class Int32 : NumericType
{
	public Int32() : base(4, new("i"))
	{
	}
}

public class Int64 : NumericType
{
	public Int64() : base(8, new("q"))
	{
	}
}

public class UInt8 : NumericType
{
	public UInt8() : base(1, new("B"))
	{
	}
}

public class UInt16 : NumericType
{
	public UInt16() : base(2, new("H"))
	{
	}
}

public class UInt32 : NumericType
{
	public UInt32() : base(4, new("I"))
	{
	}
}

public class UInt64 : NumericType
{
	public UInt64() : base(8, new("Q"))
	{
	}
}

public class Float32 : NumericType
{
	public Float32() : base(4, new("f"))
	{
	}
}

public class Float64 : NumericType
{
	public Float64() : base(8, new("d"))
	{
	}
}