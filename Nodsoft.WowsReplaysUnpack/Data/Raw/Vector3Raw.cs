using System;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Data.Raw;

public sealed record Vector3Raw
{
	public Vector3Raw(Stream stream)
	{
		byte[] bX = new byte[4];
		byte[] bY = new byte[4];
		byte[] bZ = new byte[4];
		
		stream.Read(bX);
		stream.Read(bY);
		stream.Read(bZ);

		X = BitConverter.ToSingle(bX);
		Y = BitConverter.ToSingle(bY);
		Z = BitConverter.ToSingle(bZ);
	}
	public float X { get; }
	public float Y { get; }
	public float Z { get; }
}