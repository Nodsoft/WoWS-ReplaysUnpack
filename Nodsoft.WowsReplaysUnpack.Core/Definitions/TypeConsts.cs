using Nodsoft.WowsReplaysUnpack.Core.DataTypes;
using System.Collections.Immutable;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public static class TypeConsts
{
	/// <summary>
	/// Provides common primitive types names, as read from the .def files, and their CLR types.
	/// </summary>
	public static IReadOnlyDictionary<string, Type> SimpleTypeMappings { get; } = new Dictionary<string, Type>
	{
		{ "BLOB", typeof(BlobDataType) },
		{ "STRING", typeof(StringDataType) },
		{ "UNICODE_STRING", typeof(StringDataType) },
		{ "FLOAT", typeof(Float32DataType) },
		{ "FLOAT32", typeof(Float32DataType) },
		{ "FLOAT64", typeof(Float64DataType) },
		{ "INT8", typeof(Int8DataType) },
		{ "INT16", typeof(Int16DataType) },
		{ "INT32", typeof(Int32DataType) },
		{ "INT64", typeof(Int64DataType) },
		{ "UINT8", typeof(UInt8DataType) },
		{ "UINT16", typeof(UInt16DataType) },
		{ "UINT32", typeof(UInt32DataType) },
		{ "UINT64", typeof(UInt64DataType) },
		{ "VECTOR2", typeof(Vector2DataType) },
		{ "VECTOR3", typeof(Vector3DataType) },
		{ "VECTOR4", typeof(Vector4DataType) },
		{ "MAILBOX", typeof(MailboxDataType) },
		{ "PYTHON", typeof(BlobDataType) },
		{ "FIXED_DICT", typeof(FixedDictDataType) },
		{ "ARRAY", typeof(ArrayDataType) },
		{ "TUPLE", typeof(ArrayDataType) },
		{ "USER_TYPE", typeof(ChildDataType) }
	};
}
